using BSN.Resa.Mci.CallCenter.Presentation.Dto;
using BSN.Resa.Vns.Commons.Extensions;
using BSN.Resa.Vns.Commons.Responses;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
    public class AgentInformationHttpClient: IAgentInformationHttpClient
    {
        public AgentInformationHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _logger = Log.Logger.ForContext("SourceContext", nameof(CallEventsReportHttpClient));
        }

        public async Task<OperatorsQueryServiceGetByExternalIdResponse> GetAgentInfoByUserIdAsync(string userId)
        {
            try
            {
                _logger.Information("Attemting to query sip settings for agent using user id: {UserId}.", userId);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"/api/Operators/UserId/{userId}");

                OperatorsQueryServiceGetByExternalIdResponse deserializedResponse = await responseMessage.Content.ReadAsAsyncCaseInsensitive<OperatorsQueryServiceGetByExternalIdResponse>();

                _logger.Information("Attemting to retrieved sip settings with payload: {Payload}.", deserializedResponse.SerializeToJson());

                return deserializedResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while getting sip settings from server.");
                throw ex;
            }
        }

        public async Task<OperatorsQueryServiceGetBySoftPhoneNumberResponse> GetAgentInfoAsync(string sipUsername)
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
