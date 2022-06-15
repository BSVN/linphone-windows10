using BelledonneCommunications.Linphone.Presentation.Dto;
using Linphone.Views;
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

        public async Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber)
        {
            try
            {
                _logger.Information("Attempting to deliver incoming call initation from {CallerNumber} to {CalleeNumber}.", callerPhoneNumber, agentPhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceInitiateIncomingRequest
                { 
                    AgentPhoneNumber = agentPhoneNumber,
                    CallerPhoneNumber = callerPhoneNumber,
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/Calls/Incoming/Initiate", content);

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

        public async Task<CallsCommandServiceInitiateOutgoingResponse> InitiateOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber)
        {
            try
            {
                _logger.Information("Attempting to deliver outgoing call initation from {CallerNumber} to {CalleeNumber}.", agentPhoneNumber, calleePhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceInitiateOutgoingRequest
                {
                    AgentPhoneNumber = agentPhoneNumber,
                    CalleePhoneNumber = calleePhoneNumber
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/Calls/Outgoing/Initiate", content);

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
                
                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/Calls/Established/{callId}", null);

                CallsCommandServiceEstablishResponse response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceEstablishResponse>();
                
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
        public async Task SubmitMissedIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber)
        {
            try
            {
                _logger.Information("Attempting to deliver a missed call report form {CallerNumber} to {CalleeNumber}.", callerPhoneNumber, agentPhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceSubmitMissedIncomingRequest
                {
                    CallerPhoneNumber = callerPhoneNumber,
                    AgentPhoneNumber = agentPhoneNumber
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"/api/Calls/Incoming/Missed", content);

                CallsCommandServiceSubmitMissedIncomingResponse result = await response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceSubmitMissedIncomingResponse>();

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
        public async Task SubmitMissedOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber)
        {
            try
            {
                _logger.Information("Attempting to deliver a missed outgoing call report form {CallerNumber} to {CalleeNumber}.", agentPhoneNumber, calleePhoneNumber);

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new CallsCommandServiceSubmitMissedOutgoingRequest
                {
                    AgentSoftPhoneNumber = agentPhoneNumber,
                    CalleePhoneNumber = calleePhoneNumber
                }), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"/api/Calls/Outgoing/Missed", content);

                CallsCommandServiceSubmitMissedOutgoingResponse result = await response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceSubmitMissedOutgoingResponse>();

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

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/Calls/Terminate/{callId}", null);

                CallsCommandServiceTerminateResponse response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceTerminateResponse>();

                _logger.Information("Successfully delivered call termination event with payload: {Payload}.", response.SerializeToJson());

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while delivering call termination event.");
                throw ex;
            }
        }

        public async Task<OperatorsQueryServiceGetBySoftPhoneNumberResponse> GetAgentInfo(string sipUsername)
        {
            try
            {
                _logger.Information("Attemting to query sip settings for agent: {SipPhoneNumber}.", sipUsername);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/Operators/SoftPhoneNumber/{sipUsername}");

                OperatorsQueryServiceGetBySoftPhoneNumberResponse deserializedResponse = await responseMessage.Content.ReadAsAsyncCaseInsensitive<OperatorsQueryServiceGetBySoftPhoneNumberResponse>();

                _logger.Information("Attemting to retrieved sip settings with payload: {Payload}.", deserializedResponse.SerializeToJson());

                return deserializedResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while getting sip settings from server.");
                throw ex;
            }
        }

        public async Task<bool> UpdateAgentStatusAsync(string sipPhoneNumber, AgentStatus status)
        {
            try
            {
                _logger.Information("Attemting to update agent status: {SipPhoneNumber} to {TargetState}.", sipPhoneNumber, status.ToString("g"));

                var request = new AgentsCommandServiceChangeStatusRequest()
                {
                    Status = status
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await _httpClient.PostAsync($"/api/CallRespondingAgents/{sipPhoneNumber}/Status", content);

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
