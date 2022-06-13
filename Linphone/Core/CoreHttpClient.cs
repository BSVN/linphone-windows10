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

        public async Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerNumber, string calleeNumber)
        {
            try
            {
                _logger.Information("Attempting to deliver incoming call initation from {CallerNumber} to {CalleeNumber}.", callerNumber, calleeNumber);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/Calls/InitiateIncoming?CustomerPhoneNumber={callerNumber}&OperatorSoftPhoneNumber={calleeNumber}");

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

        /// <summary>
        /// Fire and forget method for updating call's state to accepted.
        /// </summary>
        /// <remarks>
        /// Should be called after incoming call initiation with a proper callid
        /// This method call is a fire and forget method.
        /// Timing is not a premise as long as we doubted on application crashing reason.
        /// </remarks>
        /// <param name="callId">Call id</param>
        public async Task AcceptIncomingCallAsync(Guid callId)
        {
            try
            {
                _logger.Information("Attemting to deliver incoming call accepted event, for call id: {CallId}.", callId);
                
                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/Calls/AcceptIncoming/{callId}");

                CallsCommandServiceInitiateIncomingResponse response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateIncomingResponse>();
                
                _logger.Information("Successfully delivered incoming call accepted event with response payload: {Payload}.", response.SerializeToJson());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to deliver incoming call accepted message.");
            }
        }

        /// <summary>
        /// Fire and forget method to submit a missed call.
        /// </summary>
        /// <param name="callerNumber">Caller PhoneNumber.</param>
        /// <param name="calleeNumber">Callee PhoneNumber</param>
        public async Task SubmitMissedCallAsync(string callerNumber, string calleeNumber)
        {
            try
            {
                _logger.Information("Attempting to deliver a missed call report form {CallerNumber} to {CalleeNumber}.", callerNumber, calleeNumber);

                HttpResponseMessage response = await _httpClient.GetAsync($"/api/Calls/MissedIncoming?CustomerPhoneNumber={callerNumber}&OperatorSoftPhoneNumber={calleeNumber}");

                CallsCommandServiceSubmitMissedIncomingResponse result = await response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceSubmitMissedIncomingResponse>();

                _logger.Information("Successfully reported the missed call with response payload {Payload}.", result.SerializeToJson());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send request for submission of a missed call.");
            }
        }

        public async Task<CallsCommandServiceTerminateIncomingResponse> TerminateCallAsync(Guid callId)
        {            
            try
            {
                _logger.Information("Attemting to deliver call termination event, for call id: {CallId}.", callId);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/Calls/TerminateIncoming/{callId}");
                
                CallsCommandServiceTerminateIncomingResponse deserializedResponse = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceTerminateIncomingResponse>();

                _logger.Information("Successfully delivered call termination event with payload: {Payload}.", deserializedResponse.SerializeToJson());

                return deserializedResponse;
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
