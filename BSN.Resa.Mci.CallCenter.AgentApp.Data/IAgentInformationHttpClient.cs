using BSN.Resa.Mci.CallCenter.Presentation.Dto;
using System.Threading.Tasks;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
    public interface IAgentInformationHttpClient
    {
        Task<OperatorsQueryServiceGetByExternalIdResponse> GetAgentInfoByUserIdAsync(string userId);

        Task<OperatorsQueryServiceGetBySoftPhoneNumberResponse> GetAgentInfoAsync(string sipUsername);

        Task<bool> UpdateAgentStatusAsync(string sipPhoneNumber, AgentStatus status);
    }
}
