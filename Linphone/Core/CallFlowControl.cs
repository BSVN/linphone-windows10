using BelledonneCommunications.Linphone.Presentation.Dto;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            _coreClient = new CoreHttpClient();
            CallContext = new CallContext();
            AgentProfile = new PhoneProfile(panelBaseUrl);
        }

        public void SystemRegisteration()
        {

        }

        public void InitiateIncomingCall(string callerNumber)
        {
            try
            {
                CallsCommandServiceInitiateIncomingResponse response =
                    _coreClient.InitiateIncomingCallAsync(callerNumber: callerNumber,
                                                          calleeNumber: AgentProfile.SipPhoneNumber).Result;
                if (response != null)
                {
                    CallContext.CallerNumber = callerNumber;
                    CallContext.CallId = response.Data.Id;
                }

                Log.Information("Initiation incoming call from: {CallerNumber}.", callerNumber);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal error during call initation.");
            }
        }

        public void TerminateCall()
        {
            //if (Dialer.IsIncomingCall && Dialer.CallId != default && Dialer.IsIncomingCallAnswered && Dialer.IsCallTerminatedByAgent)
            //{
            //    bool StayThere = false;
            //    try
            //    {
            //        var response = await httpClient.GetAsync($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}/api/Calls/TerminateIncoming/{CallFlowControl.Instance.CallContext.CallId}");
            //        var result = response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceTerminateIncomingResponse>();
            //        if (!result.Result.Data.CallReason.HasValue && !result.Result.Data.TicketId.HasValue)
            //        {
            //            var dialerNewSource = $"/CallRespondingAgents/Dashboard?customerPhoneNumber={CallFlowControl.Instance.CallContext.CallId}&IsIncomingCall=true&CallId={CallFlowControl.Instance.AgentProfile.PanelBaseUrl}{Dialer.BrowserCurrentUrlOffset}";
            //            Dialer.BrowserCurrentUrlOffset = dialerNewSource;
            //            StayThere = true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error(ex, "Exception during call termination at the end.");
            //    }

            //    if (!StayThere)
            //    {
            //        Dialer.CallId = default;
            //        Dialer.IsIncomingCallAnswered = false;
            //        Dialer.CallerId = default;
            //        Dialer.CalleeId = default;
            //        Dialer.IsCallTerminatedByAgent = false;
            //        Dialer.BrowserCurrentUrlOffset = "";
            //    }
            //}
            //else if (Dialer.IsIncomingCall && Dialer.CallId != default && Dialer.IsIncomingCallAnswered && !Dialer.IsCallTerminatedByAgent)
            //{
            //    bool StayThere = false;

            //    try
            //    {
            //        var response = await httpClient.GetAsync($"{Dialer.BrowserBaseUrl}/api/Calls/TerminateIncoming/{Dialer.CallId}");
            //        var result = response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceTerminateIncomingResponse>();
            //        if (!result.Result.Data.CallReason.HasValue && !result.Result.Data.TicketId.HasValue)
            //        {
            //            var dialerNewSource = $"/CallRespondingAgents/Dashboard?customerPhoneNumber={Dialer.CallerId}&IsIncomingCall=true&CallId={Dialer.CallId}&RedirectUrl={Dialer.BrowserBaseUrl}{Dialer.BrowserCurrentUrlOffset}";
            //            Dialer.BrowserCurrentUrlOffset = dialerNewSource;
            //            StayThere = true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error(ex, "Exception during call termination at the end.");
            //    }

            //    if (!StayThere)
            //    {
            //        Dialer.CallId = default;
            //        Dialer.IsIncomingCallAnswered = false;
            //        Dialer.CallerId = default;
            //        Dialer.CalleeId = default;
            //        Dialer.IsCallTerminatedByAgent = false;
            //        Dialer.BrowserCurrentUrlOffset = "";
            //    }
            //}
            //else if (Dialer.IsIncomingCall && Dialer.CallId != default && !Dialer.IsIncomingCallAnswered)
            //{
            //    try
            //    {
            //        Task<HttpResponseMessage> task = httpClient.GetAsync($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}/api/Calls/TerminateIncoming/{Dialer.CallId}");
            //        task.ContinueWith(P =>
            //        {
            //            if (task.Exception != null)
            //            {
            //                Log.Error(task.Exception, "Exception during submitting call termination by customer.");
            //            }
            //            else
            //            {
            //                var result = task.Result.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceTerminateIncomingResponse>();
            //                Log.Information("Call termination by customer submited {Payload}.", result.SerializeToJson());
            //            }
            //        });
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error(ex, "Exception during call termination by customer.");
            //    }

            //    Dialer.CallId = default;
            //    Dialer.IsIncomingCallAnswered = false;
            //    Dialer.CallerId = default;
            //    Dialer.CalleeId = default;
            //    Dialer.IsCallTerminatedByAgent = false;
            //}
        }

        public void AgentAcceptedIncomingCall()
        {
            _coreClient.AcceptIncomingCallAsync(CallContext.CallId);
        }

        public void IncomingCallReceived()
        {

        }

        public void CallAccepted()
        {

        }

        public void HangUpByAgent()
        {

        }

        public void HangUp()
        {

        }

        public Uri BuildInCallUri()
        {
            if (CallContext.Direction == CallDirection.Incoming)
            {
                var inCallUri = $"{AgentProfile.PanelBaseUrl}/CallRespondingAgents/Dashboard?customerPhoneNumber={CallContext.CallerNumber}&IsIncomingCall=true&CallId={CallContext.CallId}&RedirectUrl={AgentProfile.PanelBaseUrl}/CallRespondingAgents/Dashboard?customerPhoneNumber={AgentProfile.PanelBaseUrl}&IsIncomingCall=true&CallId={CallContext.CallId}";
                return new Uri(inCallUri);
            }
            else
            {
                // TODO: Complete this.
                return new Uri("");
            }
        }

        public PhoneProfile AgentProfile { get; private set; }

        public CallContext CallContext { get; private set; }

        private CoreHttpClient _coreClient;
    }

    internal class PhoneProfile
    {
        public PhoneProfile(string panelBaseUrl)
        {
            PanelBaseUrl = panelBaseUrl;
        }

        public string PanelBaseUrl { get; private set; }

        public string SipPhoneNumber { get; set; }

        public string BrowsingHistory { get; set; }
    }

    internal class CallContext
    {
        public string CallerNumber { get; set; }

        public string CalleeNumber { get; set; }

        public Guid CallId { get; set; }

        public CallState CallState { get; set; }

        public CallDirection Direction { get; set; }
    }

    internal enum CallDirection
    {
        Incoming = 1,
        Outgoing = 2
    }

    internal enum CallState
    {
        Ready = 1,
        Ringing = 2,
        AgentAcceptedTheCall = 3,
        InCall = 4,
        AgentHangUp = 5,
        HangUp = 6
    }
}
