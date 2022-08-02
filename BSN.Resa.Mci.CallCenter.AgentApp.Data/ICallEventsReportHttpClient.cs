using BSN.Resa.Mci.CallCenter.Presentation.Dto;
using System;
using System.Threading.Tasks;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data
{
    public interface ICallEventsReportHttpClient
    {
        Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber, string inboundService);

        Task<CallsCommandServiceInitiateOutgoingResponse> InitiateOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber, string inboundService);

        Task CallEstablishedAsync(Guid callId);

        Task SubmitMissedIncomingCallAsync(string callerPhoneNumber, string agentPhoneNumber, string inboundservice);

        Task SubmitMissedOutgoingCallAsync(string agentPhoneNumber, string calleePhoneNumber, string inboundService);

        Task<CallsCommandServiceTerminateResponse> TerminateCallAsync(Guid callId);
    }
}
