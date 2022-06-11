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
        internal CoreHttpClient()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callerNumber"></param>
        /// <param name="calleeNumber"></param>
        /// <returns></returns>
        public async Task<CallsCommandServiceInitiateIncomingResponse> InitiateIncomingCallAsync(string callerNumber, string calleeNumber)
        {
            try
            {
                Log.Information("Send incoming call initation from {CallerNumber} to {CalleeNumber}.", callerNumber, calleeNumber);

                HttpResponseMessage responseMessage = await _httpClient.GetAsync($"{Dialer.BrowserBaseUrl}/api/Calls/InitiateIncoming?CustomerPhoneNumber={callerNumber}&OperatorSoftPhoneNumber={calleeNumber}");
                CallsCommandServiceInitiateIncomingResponse response = await responseMessage.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateIncomingResponse>();

                Log.Information("Call initiation successfully done with call id: {CallId}.", response.Data?.Id);

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Internal error while initiating an incoming call.");
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
        public void AcceptIncomingCallAsync(Guid callId)
        {
            try
            {
                Log.Information("Send a fire and forget call to accept the call with id: {CallId}.", callId);

                Task<HttpResponseMessage> task = _httpClient.GetAsync($"{Dialer.BrowserBaseUrl}/api/Calls/AcceptIncoming/{callId}");

                task.ContinueWith(P =>
                {
                    if (task.Exception != null)
                    {
                        Log.Error(task.Exception, "Exception during Accepting an incoming call.");
                    }
                    else
                    {
                        CallsCommandServiceInitiateIncomingResponse response =
                            task.Result.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateIncomingResponse>().Result;
                        Log.Information("Accept incoming call response payload {Payload}.", response.SerializeToJson());
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send accept call request.");
            }
        }

        /// <summary>
        /// Fire and forget method to submit a missed call.
        /// </summary>
        /// <param name="callerNumber">Caller PhoneNumber.</param>
        /// <param name="calleeNumber">Callee PhoneNumber</param>
        public void SubmitMissedCallAsync(string callerNumber, string calleeNumber)
        {
            try
            {
                Log.Information("Initiate a fire and forget call to submit a missed call form {CallerNumber} to {CalleeNumber}.", callerNumber, calleeNumber);

                Task<HttpResponseMessage> task = _httpClient.GetAsync($"{Dialer.BrowserBaseUrl}/api/Calls/MissedIncoming?CustomerPhoneNumber={callerNumber}&OperatorSoftPhoneNumber={calleeNumber}");

                task.ContinueWith(P =>
                {
                    if (task.Exception != null)
                    {
                        Log.Error(task.Exception, "Exception during submit a missed incoming call.");
                    }
                    else
                    {
                        var result = task.Result.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceSubmitMissedIncomingResponse>();
                        Log.Information("Accept missed incoming call response payload {Payload}.", result.SerializeToJson());
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send request for submission of a missed call.");
            }
        }



        private readonly HttpClient _httpClient;
    }
}
