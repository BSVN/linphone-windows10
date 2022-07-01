using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BelledonneCommunications.Linphone.Dialogs
{
	public sealed partial class ContinueCallbackAnsweringDialog : ContentDialog
	{
		public ContinueCallbackAnsweringDialog()
		{
			this.InitializeComponent();

			time = TimeSpan.FromSeconds(10);
			timer = new DispatcherTimer()
			{
				Interval = new TimeSpan(0, 0, 1)
			};

			timer.Tick += (object sender, object e) =>
			{
				this.PrimaryButtonText = $"بلی ({time.ToString("%s")}s)";
				if (time == TimeSpan.Zero)
				{
					timer.Stop();
					this.ContentDialog_OkButtonClick(this, null);
					this.Hide();
				}
				time = time.Add(TimeSpan.FromSeconds(-1));
			};

			taskCompletionSource = new TaskCompletionSource<CancellationToken>(TaskCreationOptions.RunContinuationsAsynchronously);
		}

		public Task<CancellationToken> ResultAsync
		{
			get => taskCompletionSource.Task;
		}

		private void ContentDialog_OkButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			taskCompletionSource.SetResult(new CancellationToken(false));
		}

		private void ContentDialog_NoButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			taskCompletionSource.SetResult(new CancellationToken(true));
		}

		private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
		{
			timer.Start();
		}

		private TimeSpan time;
		private readonly DispatcherTimer timer;
		private readonly TaskCompletionSource<CancellationToken> taskCompletionSource;

	}
}
