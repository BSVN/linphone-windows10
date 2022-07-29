using BelledonneCommunications.Linphone.Commons;
using BelledonneCommunications.Linphone.Presentation.Dto;
using Linphone.Model;
using PCLAppConfig;
using Serilog;
using System;
using System.Threading.Tasks;

namespace BelledonneCommunications.Linphone.Core
{
    // Mention: Sip login on shayan side, means that "I am ready for accept incoming calls" and does nothing to outgoing calls.
    // Todo: Draw and double check state transition on the phone events to prevent unpredicted situations.
    public class CallFlowControl
    {
        public CallFlowControl(ICoreHttpClient coreHttpClient, 
                               CallContext callContext, 
                               AgentProfile agentProfile)
        {
            CallContext = callContext;
            AgentProfile = agentProfile;

            _coreClient = coreHttpClient;
            _logger = Log.Logger.ForContext("SourceContext", nameof(CallFlowControl));
        }

        public AgentProfile AgentProfile { get; private set; }
        
        public CallContext CallContext { get; private set; }

        /// <summary>
        /// Initiate an incoming call by submitting a record.
        /// </summary>
        /// <param name="callerPhoneNumber">Either a customer's phonenumber or another operator's phonenumber (in inter-callcenter call scenario).</param>
        /// <param name="inboundService">It's the service phonenumber we want to introduce ourself as it's operator (e.g. 99970, 99971, ...).</param>
        /// <returns></returns>
        public async Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerPhoneNumber, string inboundService)
        {
            try
            {
                _logger.Information("Initiation incoming call from: {CallerNumber}.", callerPhoneNumber);

                CallContext.CallState = CallState.Ringing;
                CallContext.CallerNumber = callerPhoneNumber;
                CallContext.InboundService = inboundService;
                CallContext.CalleeNumber = AgentProfile.SipPhoneNumber;
                CallContext.Direction = CallDirection.Incoming;
                CallContext.CallId = default;

                CallsCommandServiceInitiateIncomingResponse response =
                    await _coreClient.InitiateIncomingCallAsync(callerPhoneNumber: callerPhoneNumber,
                                                                agentPhoneNumber: AgentProfile.SipPhoneNumber,
                                                                inboundService: inboundService);
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

        /// <summary>
        /// Initiate an outgoing call by submitting a record.
        /// </summary>
        /// <param name="calleePhoneNumber">Either a customer phonenumber or inter-callcenter phonenumber.</param>
        /// <param name="inboundService">It's the service phonenumber we want to introduce ourself as it's operator (e.g. 99970, 99971, ...).</param>
        /// <returns></returns>
        public async Task<CallsCommandServiceInitiateOutgoingResponse> InitiateOutgoingCallAsync(string calleePhoneNumber, string inboundService)
        {
            // Todo: We should prevent to submit 2 call record for missed calls. Please check it.
            try
            {
                _logger.Information("Initiation outgoing call to: {CallePhoneNumber}.", calleePhoneNumber);

                CallContext.CallState = CallState.Ringing;
                CallContext.CallerNumber = AgentProfile.SipPhoneNumber;
                CallContext.CalleeNumber = calleePhoneNumber;
                CallContext.Direction = CallDirection.Outgoing;
                CallContext.CallId = default;

                CallsCommandServiceInitiateOutgoingResponse response =
                    await _coreClient.InitiateOutgoingCallAsync(agentPhoneNumber: AgentProfile.SipPhoneNumber,
                                                                calleePhoneNumber: calleePhoneNumber,
                                                                inboundService: inboundService);
                if (response != null)
                {
                    CallContext.CallId = response.Data.Id;
                }

                _logger.Information("Outgoing call initiation successfully done with call id: {CallId}.", CallContext.CallId.ToString());

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error during call initation.");
                return null;
            }
        }

        /// <summary>
        /// Send established message of the current call.
        /// </summary>
        public void CallEstablished()
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

                _coreClient.CallEstablishedAsync(CallContext.CallId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while accepting the incoming call.");
            }
        }

        /// <summary>
        /// Update state of the call on declining.
        /// </summary>
        public void IncomingCallDeclined()
        {
            _logger.Information("Incoming call declined by agent.");

            CallContext.CallState = CallState.DeclinedByAgent;
        }

        /// <summary>
        /// Do post-termination actions for the current call.
        /// </summary>
        /// <returns>Task of async actions.</returns>
        public async Task TerminateCall()
        {
            // Call missed by caller departure.
            if (CallContext.CallState == CallState.Ringing && CallContext.Direction == CallDirection.Incoming)
            {
                _logger.Information("Missed call caused by caller hangup in ringing phase.");

                _coreClient.SubmitMissedIncomingCallAsync(CallContext.CallerNumber,
                                                          CallContext.CalleeNumber,
                                                          CallContext.InboundService);
                CallContext.Reset();
            }
            // Call missed by agent decline.
            else if (CallContext.CallState == CallState.DeclinedByAgent && CallContext.Direction == CallDirection.Incoming)
            {
                _logger.Information("Missed call caused by agent declining in ringing phase.");

                _coreClient.SubmitMissedIncomingCallAsync(CallContext.CallerNumber,
                                                          CallContext.CalleeNumber,
                                                          CallContext.InboundService);
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

                    CallsCommandServiceTerminateResponse callTerminationResponse = await _coreClient.TerminateCallAsync(CallContext.CallId);
                    if (!callTerminationResponse.Data.CallReason.HasValue && !callTerminationResponse.Data.TicketId.HasValue)
                    {
                        AgentProfile.BrowsingHistory = $"/CallRespondingAgents/Dashboard?CallId={CallContext.CallId}";
                    }

                    CallContext.Reset();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to terminate the call.");
                }
            }
            else if (CallContext.Direction == CallDirection.Outgoing &&
                     CallContext.CallState == CallState.DeclinedByAgent)
            {
                _logger.Information("Agent canceled the call before customer answers the call.");
                try
                {
                    // Todo: Call should be ignored in this situation.
                    _coreClient.TerminateCallAsync(CallContext.CallId);

                    CallContext.Reset();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Internal error while delivering call termination report.");
                }
            }
            else if (CallContext.Direction == CallDirection.Outgoing &&
                     CallContext.CallState == CallState.Ringing)
            {
                _logger.Information("Callee canceled the call before call being established.");
                try
                {
                    // Todo: Call should be ignored in this situation.
                    _coreClient.SubmitMissedOutgoingCallAsync(AgentProfile.SipPhoneNumber,
                                                              CallContext.CalleeNumber,
                                                              CallContext.InboundService);

                    AgentProfile.BrowsingHistory = "";

                    CallContext.Reset();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Internal error while delivering missed outgoing call report.");
                }
            }
            else if (CallContext.Direction == CallDirection.Outgoing)
            {
                _logger.Information("Callee canceled the call before call being established.");
                try
                {
                    // Todo: Call should be ignored in this situation.
                    _coreClient.TerminateCallAsync(CallContext.CallId);

                    CallContext.Reset();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Internal error while delivering missed outgoing call report.");
                }
            }
            else
            {
                _logger.Information("Unexpected call termination situation, CallContext: {CallContext}.", CallContext.SerializeToJson());
                CallContext.Reset();
            }
        }

        /// <summary>
        /// Get sip settings of the current agent.
        /// </summary>
        /// <returns>Agent's sip settings.</returns>
        public async Task<OperatorsQueryServiceGetBySoftPhoneNumberResponse> GetAgentSettings()
        {
            try
            {
                return await _coreClient.GetAgentInfoAsync(AgentProfile.SipPhoneNumber);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while getting settings for the agent.");
                return null;
            }
        }

        /// <summary>
        /// Notify agent status change to the telephony system and the phone's back-end service.
        /// </summary>
        /// <param name="status">New state of the agent.</param>
        /// <returns>Determines whether the update was successful or not.</returns>
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
                        JoinIntoIncomingCallQueue();
                        break;
                    case AgentStatus.Break:
                        LeaveIncomingCallQueue();
                        break;
                    case AgentStatus.Offline:
                        CallContext.Direction = CallDirection.Command;
                        LinphoneManager.Instance.NewOutgoingCall("agent-logoff");
                        AgentProfile.JoinedIntoIncomingCallQueue = false;
                        break;
                }

                _logger.Information("Successfully updated agent status.");

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while updating agent status.");
                return false;
            }
        }

        public void LeaveIncomingCallQueue()
        {
            CallContext.Direction = CallDirection.Command;
            LinphoneManager.Instance.NewOutgoingCall("agent-on-break");
            AgentProfile.JoinedIntoIncomingCallQueue = false;
        }

        public void JoinIntoIncomingCallQueue()
        {
            CallContext.Direction = CallDirection.Command;
            LinphoneManager.Instance.NewOutgoingCall("agent-login");
            AgentProfile.JoinedIntoIncomingCallQueue = true;
        }

        public async Task<OperatorsQueryServiceGetByExternalIdResponse> GetAgentSettingByUserId(string userId)
        {
            try
            {
                return await _coreClient.GetAgentInfoByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while getting settings for the agent by id.");
                return null;
            }
        }

        private readonly ICoreHttpClient _coreClient;
        private readonly ILogger _logger;
    }

    public class AgentProfile
    {
        public AgentProfile()
        {
            JoinedIntoIncomingCallQueue = false;
        }

        public string SipPhoneNumber { get; set; }

        public string BrowsingHistory { get; set; }

        public bool JoinedIntoIncomingCallQueue { get; set; }

        public bool IsLoggedIn { get; set; }

        public AgentStatus Status { get; set; }
    }

    public class CallContext
    {
        public string CallerNumber { get; set; }

        public string CalleeNumber { get; set; }

        public string InboundService { get; set; }

        public Guid CallId { get; set; }

        public CallState CallState { get; set; }

        public CallDirection Direction { get; set; }

        public CallContext()
        {
            CallState = CallState.Ready;
        }

        internal void Reset()
        {
            CallerNumber = string.Empty;
            CalleeNumber = string.Empty;
            CallId = default;
            CallState = CallState.Ready;
        }
    }

    public enum CallDirection
    {
        Incoming = 1,
        Outgoing = 2,
        Command = 3
    }

    public enum CallState
    {
        Ready = 1,
        Ringing = 2,
        Established = 3,
        DeclinedByAgent = 4,
        InCall = 5
    }
}
