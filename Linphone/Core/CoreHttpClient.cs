using BelledonneCommunications.Linphone.Commons;
using BelledonneCommunications.Linphone.Presentation.Dto;
using BSN.Resa.Vns.Commons.Responses;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BelledonneCommunications.Linphone.Core
{
    internal class CoreHttpClient
    {
        internal CoreHttpClient(string baseUrl)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };

            _logger = Log.Logger.ForContext("SourceContext", nameof(CoreHttpClient));
        }

        public async Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber, string inboundService)
        {
            try
            {
                _logger.Information("Attempting to deliver incoming call initation from {CallerNumber} to {CalleeNumber}.", callerPhoneNumber, agentPhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceInitiateIncomingRequest
                {
                    AgentPhoneNumber = agentPhoneNumber,
                    CallerPhoneNumber = callerPhoneNumber,
                    InboundService = inboundService
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/v2.0/Calls/Incoming", content);

                CallsCommandServiceInitiateIncomingResponse response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateIncomingResponse>();

                _logger.Information("Successfully delivered call initiation message with call id: {CallId}.", response.Data?.Id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while delivering incoming call initiation message.");
                throw ex;
            }
        }

        public async Task<CallsCommandServiceInitiateOutgoingResponse> InitiateOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber, string inboundService)
        {
            try
            {
                _logger.Information("Attempting to deliver outgoing call initation from {CallerNumber} to {CalleeNumber}.", agentPhoneNumber, calleePhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceInitiateOutgoingRequest
                {
                    AgentPhoneNumber = agentPhoneNumber,
                    CalleePhoneNumber = calleePhoneNumber,
                    InboundService = inboundService
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/v2.0/Calls/Outgoing", content);

                CallsCommandServiceInitiateOutgoingResponse response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateOutgoingResponse>();

                _logger.Information("Successfully delivered call initiation message with call id: {CallId}.", response.Data?.Id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while delivering outgoing call initiation message.");
                throw ex;
            }
        }

        public async Task<CallsCommandServiceInitiateCallbackResponse> InitiateCallbackAsync(string agentPhoneNumber, string calleePhoneNumber, string inboundService, DateTime requestedAt)
        {
            try
            {
                _logger.Information("Attempting to deliver callback initation from {CallerNumber} to {CalleeNumber}, request at {RequestedAt}.", agentPhoneNumber, calleePhoneNumber, requestedAt);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceInitiateCallbackRequest
                {
                    AgentPhoneNumber = agentPhoneNumber,
                    CalleePhoneNumber = calleePhoneNumber,
                    InboundService = inboundService,
                    RequestAt = requestedAt,
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/v2.0/Calls/Callback", content);

                CallsCommandServiceInitiateCallbackResponse response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateCallbackResponse>();

                _logger.Information("Successfully delivered call initiation message with call id: {CallId}.", response.Data?.Id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while delivering callback initiation message.");
                throw ex;
            }
        }

        public async Task<CallsCommandServiceInitiateOutgoingResponse> InitiateCampaignCallAsync(string agentPhoneNumber,
                                                                                                 string calleePhoneNumber,
                                                                                                 string callCampaignId,
                                                                                                 string inboundService)
        {
            try
            {
                _logger.Information("Attempting to deliver outgoing call initation from {CallerNumber} to {CalleeNumber}.", agentPhoneNumber, calleePhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceInitiateCampaignCallRequest
                {
                    CallCampaignId = callCampaignId,
                    AgentPhoneNumber = agentPhoneNumber,
                    CalleePhoneNumber = calleePhoneNumber,
                    InboundService = inboundService
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/v2.0/Calls/Campaign", content);

                var response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateOutgoingResponse>();

                _logger.Information("Successfully delivered call initiation message with call id: {CallId}.", response.Data?.Id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while delivering outgoing call initiation message.");
                throw ex;
            }
        }

        public async Task<CallCampaignsQueryServiceGetListByUserIdResponse> GetCallCampaignsAsync(string sipPhoneNumber)
        {
            try
            {
                _logger.Information("Attempting to retrieve agents call campaign list, Agent sipPhoneNumber: {UserId}.", sipPhoneNumber);


                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/v2.0/DesktopApplicationAgents/SipPhoneNumber/{sipPhoneNumber}/CallCampaigns");

                var response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallCampaignsQueryServiceGetListByUserIdResponse>();

                _logger.Information("Successfully retrieved call campaigns for the agent.");

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while retriving the list of call campaigns.");
                throw ex;
            }
        }

        /// <summary>
        /// Fire and forget method for updating call's state to accepted.
        /// </summary>
        /// <remarks>
        /// Should be called after incoming call initiation with a proper callid
        /// This method call is a fire and forget method.
        /// Timing is not a premise as long as we doubted on application crashing reason.
        /// </remarks>
        /// <param name="callId">Call id</param>
        public async Task CallEstablishedAsync(Guid callId)
        {
            try
            {
                _logger.Information("Attemting to deliver call established event, for call id: {CallId}.", callId);

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/v2.0/Calls/{callId}/Establish", null);

                var response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceEstablishResponse>();

                _logger.Information("Successfully delivered call established event with response payload: {Payload}.", response.SerializeToJson());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deliver call established event message.");
            }
        }

        /// <summary>
        /// Fire and forget method to submit a missed call.
        /// </summary>
        /// <param name="callerPhoneNumber">Caller PhoneNumber.</param>
        /// <param name="agentPhoneNumber">Callee PhoneNumber</param>
        public async Task SubmitMissedIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber, string inboundservice)
        {
            try
            {
                _logger.Information("Attempting to deliver a missed call report form {CallerNumber} to {CalleeNumber}.", callerPhoneNumber, agentPhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceSubmitMissedIncomingRequest
                {
                    CallerPhoneNumber = callerPhoneNumber,
                    AgentPhoneNumber = agentPhoneNumber,
                    InboundService = inboundservice
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"/api/v2.0/Calls/Incoming/Missed", content);

                var result = await response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceSubmitMissedIncomingResponse>();

                _logger.Information("Successfully reported the missed call with response payload {Payload}.", result.SerializeToJson());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send request for submission of a missed call.");
            }
        }

        /// <summary>
        /// Fire and forget method to submit a missed call.
        /// </summary>
        /// <param name="callerPhoneNumber">Caller PhoneNumber.</param>
        /// <param name="agentPhoneNumber">Callee PhoneNumber</param>
        public async Task SubmitMissedCallByIdAsync(Guid id)
        {
            try
            {
                _logger.Information("Attempting to deliver a missed call report using call id: {CallId}.", id.ToString());

                HttpResponseMessage response = await _httpClient.PostAsync($"/api/v2.0/Calls/Missed/{id}", null);

                var result = await response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceSubmitMissedByIdResponse>();

                _logger.Information("Successfully reported the missed call with response payload {Payload}.", result.SerializeToJson());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send request for submission of a missed call.");
            }
        }

        /// <summary>
        /// Fire and forget method to submit a missed call.
        /// </summary>
        /// <param name="callerPhoneNumber">Caller PhoneNumber.</param>
        /// <param name="agentPhoneNumber">Callee PhoneNumber</param>
        public async Task SubmitMissedOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber, string inboundService)
        {
            try
            {
                _logger.Information("Attempting to deliver a missed outgoing call report form {CallerNumber} to {CalleeNumber}.", agentPhoneNumber, calleePhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceSubmitMissedOutgoingRequest
                {
                    AgentSoftPhoneNumber = agentPhoneNumber,
                    CalleePhoneNumber = calleePhoneNumber,
                    InboundService = inboundService
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"/api/v2.0/Calls/Outgoing/Missed", content);

                var result = await response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceSubmitMissedOutgoingResponse>();

                _logger.Information("Successfully reported the missed outgoing call with response payload {Payload}.", result.SerializeToJson());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send request for submission of a missed call.");
            }
        }

        public async Task<CallsCommandServiceTerminateResponse> TerminateCallAsync(Guid callId)
        {
            try
            {
                _logger.Information("Attemting to deliver call termination event, for call id: {CallId}.", callId);

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/v2.0/Calls/{callId}/Terminate", null);

                var response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceTerminateResponse>();

                _logger.Information("Successfully delivered call termination event with payload: {Payload}.", response.SerializeToJson());

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while delivering call termination event.");
                throw ex;
            }
        }

        public async Task<DesktopApplicationAgentsQueryServiceGetByUserIdResponse> GetAgentInfoByUserId(string userId)
        {
            try
            {
                _logger.Information("Attemting to query sip settings for agent using user id: {UserId}.", userId);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/v2.0/DesktopApplicationAgents/UserId/{userId}");

                var deserializedResponse = await responseMessage.Content.ReadAsAsyncCaseInsensitive<DesktopApplicationAgentsQueryServiceGetByUserIdResponse>();

                _logger.Information("Attemting to retrieved sip settings with payload: {Payload}.", deserializedResponse.SerializeToJson());

                return deserializedResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while getting sip settings from server.");
                throw ex;
            }
        }

        public async Task<DesktopApplicationAgentsQueryServiceGetBySipPhoneNumberResponse> GetAgentInfo(string sipUsername)
        {
            try
            {
                _logger.Information("Attemting to query sip settings for agent: {SipPhoneNumber}.", sipUsername);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/v2.0/DesktopApplicationAgents/SipPhoneNumber/{sipUsername}");

                var deserializedResponse = await responseMessage.Content.ReadAsAsyncCaseInsensitive<DesktopApplicationAgentsQueryServiceGetBySipPhoneNumberResponse>();

                _logger.Information("Attemting to retrieved sip settings with payload: {Payload}.", deserializedResponse.SerializeToJson());

                return deserializedResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while getting sip settings from server.");
                throw ex;
            }
        }

        public async Task<bool> UpdateAgentStatusAsync(string sipPhoneNumber, DesktopApplicationAgentStatus status)
        {
            try
            {
                _logger.Information("Attemting to update agent status: {SipPhoneNumber} to {TargetState}.", sipPhoneNumber, status.ToString("g"));

                var request = new DesktopApplicationAgentsCommandServiceUpdateStatusRequest()
                {
                    Status = status
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/v2.0/DesktopApplicationAgents/{sipPhoneNumber}/Status", content);

                Response response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<Response>();

                if (!response.IsSuccess)
                {
                    _logger.Error("Failed to update agent status with response payload: {Payload}.", response.SerializeToJson());
                    return false;
                }

                _logger.Information("Successfully updated agent status wit response payload: {Payload}.", response.SerializeToJson());
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while updating agent status.");
                throw ex;
            }
        }


        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
    }
}
