﻿using BelledonneCommunications.Linphone.Presentation.Dto;
using Linphone.Model;
using Linphone.Views;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.Storage;

namespace BelledonneCommunications.Linphone.Core
{
    internal class CallFlowControl
    {
        private static CallFlowControl _instance = new CallFlowControl();
        public static CallFlowControl Instance
        {
            get
            {
                return _instance;
            }
        }

        private CallFlowControl()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            object settingValue = localSettings.Values["PanelUrl"];
            string panelBaseUrl = settingValue == null ? "http://10.19.82.133:9011" : settingValue as string;

            _logger = Log.Logger.ForContext("SourceContext", nameof(CallFlowControl));
            _coreClient = new CoreHttpClient(panelBaseUrl);

            CallContext = new CallContext();
            AgentProfile = new PhoneProfile(panelBaseUrl);
        }

        public void SystemRegisteration()
        {

        }

        public async Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerNumber)
        {
            try
            {
                _logger.Information("Initiation incoming call from: {CallerNumber}.", callerNumber);

                CallContext.CallState = CallState.Ringing;
                CallContext.CallerNumber = callerNumber;
                CallContext.CalleeNumber = AgentProfile.SipPhoneNumber;
                CallContext.Direction = CallDirection.Incoming;
                CallContext.CallId = default; 

                CallsCommandServiceInitiateIncomingResponse response =
                    await _coreClient.InitiateIncomingCallAsync(callerNumber: callerNumber,
                                                                calleeNumber: AgentProfile.SipPhoneNumber);
                if (response != null)
                {
                    CallContext.CallId = response.Data.Id;
                }

                _logger.Information("Incoming call initiation successfully done with call id: {CallId}.", CallContext.CallId.ToString());

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error during call initation.");
                return null;
            }
        }

        public void IncomingCallAccepted()
        {
            try
            {
                // Todo: Call state transition check should be implemented.
                CallContext.CallState = CallState.Established;

                if (CallContext.CallId == default)
                {
                    _logger.Information("Skip delivering call establishing event, because the Call did not initiated properly.");
                    return;
                }

                _coreClient.AcceptIncomingCallAsync(CallContext.CallId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while accepting the incoming call.");
            }
        }

        public void IncomingCallDeclined()
        {
            _logger.Information("Incoming call declined by agent.");
            
            CallContext.CallState = CallState.DeclinedByAgent;
        }

        public async Task TerminateCall()
        {
            // Call missed by caller departure.
            if (CallContext.CallState == CallState.Ringing && CallContext.Direction == CallDirection.Incoming)
            {
                _logger.Information("Missed call caused by caller hangup in ringing phase.");

                _coreClient.SubmitMissedCallAsync(CallContext.CallerNumber,
                                                  CallContext.CalleeNumber);
                CallContext.Reset();
            }
            // Call missed by agent decline.
            else if (CallContext.CallState == CallState.DeclinedByAgent && CallContext.Direction == CallDirection.Incoming)
            {
                _logger.Information("Missed call caused by agent declining in ringing phase.");

                _coreClient.SubmitMissedCallAsync(CallContext.CallerNumber,
                                                  CallContext.CalleeNumber);
                CallContext.Reset();
            }
            // Call terminated either by caller hang up or agent hang up during an established call.
            else if (CallContext.Direction == CallDirection.Incoming)
            {
                try
                {
                    _logger.Information("Call terminated either by agent hangup or caller hangup during a call.");

                    if (CallContext.CallId == default)
                        return;

                    CallsCommandServiceTerminateIncomingResponse callTerminationResponse = await _coreClient.TerminateCallAsync(CallContext.CallId);
                    if (!callTerminationResponse.Data.CallReason.HasValue && !callTerminationResponse.Data.TicketId.HasValue)
                    {
                        var redirectUrl = HttpUtility.UrlEncode($"{AgentProfile.PanelBaseUrl}/CallRespondingAgents/Dashboard?customerPhoneNumber={CallContext.CallerNumber}&IsIncomingCall=true&CallId={CallContext.CallId}");
                        var inCallUri = $"{AgentProfile.PanelBaseUrl}/CallRespondingAgents/Dashboard?customerPhoneNumber={CallContext.CallerNumber}&IsIncomingCall=true&CallId={CallContext.CallId}&RedirectUrl={redirectUrl}";
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to terminate the call.");
                }
            }
            else
            {
                _logger.Information("Unexpected call termination situation, CallContext: {CallContext}.", CallContext.SerializeToJson());
            }
        }

        public void HangUpByAgent()
        {
            _logger.Information("Agent has ended the call.");

            //CallContext.CallState = CallState.AgentHangUp;
        }

        public Uri BuildInCallUri()
        {
            if (CallContext.Direction == CallDirection.Incoming)
            {
                var redirectUrl = HttpUtility.UrlEncode($"{AgentProfile.PanelBaseUrl}/CallRespondingAgents/Dashboard?customerPhoneNumber={CallContext.CallerNumber}&IsIncomingCall=true&CallId={CallContext.CallId}");
                var inCallUri = $"{AgentProfile.PanelBaseUrl}/CallRespondingAgents/Dashboard?customerPhoneNumber={CallContext.CallerNumber}&IsIncomingCall=true&CallId={CallContext.CallId}&RedirectUrl={redirectUrl}";
                return new Uri(inCallUri);
            }
            else
            {
                // TODO: Complete this.
                return new Uri("");
            }
        }

        public async Task<OperatorsQueryServiceGetBySoftPhoneNumberResponse> GetAgentSettings()
        {
            try
            {
                return await _coreClient.GetAgentInfo(AgentProfile.SipPhoneNumber);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Error while getting settings for the agent.");
                return null;
            }
        }

        public async Task<bool> UpdateAgentStatusAsync(AgentStatus status)
        {
            try
            {
                _logger.Information("Attempting to update agent status.");

                switch (status)
                {
                    case AgentStatus.Ready:
                        if (AgentProfile.Status == AgentStatus.Ready) return true;
                        break;
                    case AgentStatus.Break:
                        if (AgentProfile.Status == AgentStatus.Break) return true;
                        break;
                    case AgentStatus.Offline:
                        if (AgentProfile.Status == AgentStatus.Offline) return true;
                        break;
                }

                bool response = await _coreClient.UpdateAgentStatusAsync(AgentProfile.SipPhoneNumber, status);

                if (response)
                {
                    AgentProfile.Status = status;
                }

                switch (status)
                {
                    case AgentStatus.Ready:
                        CallContext.Direction = CallDirection.Command;
                        LinphoneManager.Instance.NewOutgoingCall("agent-login");
                        break;
                    case AgentStatus.Break:
                        CallContext.Direction = CallDirection.Command;
                        LinphoneManager.Instance.NewOutgoingCall("agent-on-break");
                        break;
                    case AgentStatus.Offline:
                        CallContext.Direction = CallDirection.Command;
                        LinphoneManager.Instance.NewOutgoingCall("agent-logoff");
                        break;
                }

                _logger.Information("Successfully updated agent status.");
                
                return response;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Internal error while updating agent status.");
                return false;
            }
        }


        public PhoneProfile AgentProfile { get; private set; }
        public CallContext CallContext { get; private set; }
        private CoreHttpClient _coreClient;
        private readonly ILogger _logger;
    }

    internal class PhoneProfile
    {
        public PhoneProfile(string panelBaseUrl)
        {
            PanelBaseUrl = panelBaseUrl;
            Status = AgentStatus.Offline;
        }

        public string PanelBaseUrl { get; private set; }

        public string SipPhoneNumber { get; set; }

        public string BrowsingHistory { get; set; }

        public bool IsLoggedIn { get; set; }

        public AgentStatus Status { get; set; } = AgentStatus.Offline;
    }

    internal class CallContext
    {
        public string CallerNumber { get; set; }

        public string CalleeNumber { get; set; }

        public Guid CallId { get; set; }

        public CallState CallState { get; set; }

        public CallDirection Direction { get; set; }

        internal void Reset()
        {
            CallerNumber = string.Empty;
            CalleeNumber = string.Empty;
            CallId = default;
            CallState = CallState.Ready;
        }
    }

    internal enum CallDirection
    {
        Incoming = 1,
        Outgoing = 2,
        Command = 3
    }

    internal enum CallState
    {
        Ready = 1,
        Ringing = 2,
        Established = 3,
        DeclinedByAgent = 4,
        InCall = 5
    }
}
