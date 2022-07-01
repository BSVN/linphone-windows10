using Linphone;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSN.LinphoneSDK
{
	/// <summary>
	/// Class for representing call in system
	/// </summary>
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public class Call
	{
		/// <summary>
		/// Construct call from linphone call
		/// </summary>
		/// <param name="rawCall"></param>
		public Call(Linphone.Call rawCall)
		{
			RawCall = rawCall;
			callEndedTaskCompletion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
		}

		/// <summary>
		/// Raw object of call in linphone SDK
		/// </summary>
		public Linphone.Call RawCall { get; set; }

		/// <summary>
		/// The Id of call, that does not unique in all time.
		/// </summary>
		public int Id
		{
			get => GenerateId(RawCall);
		}

		/// <summary>
		/// Genereate call id from raw linphone call object
		/// </summary>
		/// <param name="call"></param>
		/// <returns></returns>
		public static int GenerateId(Linphone.Call call)
		{
			return call.CallLog.CallId.GetHashCode();
		}

		/// <summary>
		/// This call has been ended
		/// </summary>
		public void Ended()
		{
			callEndedTaskCompletion.SetResult(null);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <returns></returns>
		public Task WhenEnded()
		{
			return callEndedTaskCompletion.Task;
		}

		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		// TODO: Change to generic version when update this project to .NET 5
		private TaskCompletionSource<object> callEndedTaskCompletion;
	}
}
