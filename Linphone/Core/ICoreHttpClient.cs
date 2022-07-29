using BelledonneCommunications.Linphone.Presentation.Dto;
using System;
using System.Threading.Tasks;

namespace BelledonneCommunications.Linphone.Core
{
    public interface ICoreHttpClient
    {
        Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber, string inboundService);

        Task<CallsCommandServiceInitiateOutgoingResponse> InitiateOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber, string inboundService);

        Task CallEstablishedAsync(Guid callId);

        Task SubmitMissedIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber, string inboundservice);

        Task SubmitMissedOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber, string inboundService);

        Task<CallsCommandServiceTerminateResponse> TerminateCallAsync(Guid callId);

        Task<OperatorsQueryServiceGetByExternalIdResponse> GetAgentInfoByUserIdAsync(string userId);

        Task<OperatorsQueryServiceGetBySoftPhoneNumberResponse> GetAgentInfoAsync(string sipUsername);

        Task<bool> UpdateAgentStatusAsync(string sipPhoneNumber, AgentStatus status);
    }
}
