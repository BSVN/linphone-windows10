using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BelledonneCommunications.Linphone.Messages
{
	internal class ContinueCallbackAnsweringRequestMessage : RequestMessage<Task<CancellationToken>>
	{
	}
}
