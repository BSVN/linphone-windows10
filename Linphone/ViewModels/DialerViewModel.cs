using BelledonneCommunications.Linphone.Presentation.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Linphone;
using Linphone.Model;
using Linphone.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace BelledonneCommunications.Linphone.ViewModels
{
	public class DialerViewModel : ObservableObject
	{
        public DialerViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            CallCommand = new RelayCommand(CallClick);
            BrowserLoadedCommand = new RelayCommand(OnLoadedBrowser);
            httpClient = new HttpClient();
            _logger = Log.Logger.ForContext("SourceContext", nameof(Dialer));
        }

        private Uri sourceUri;
		public Uri SourceUri
        {
            get => sourceUri;
            set => SetProperty(ref this.sourceUri, value);
        }

        // TODO: Please remove it, and use _settings
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

		// TODO: Please remove all static variable
        public static string BrowserCurrentUrlOffset = null;
        public static string BrowserBaseUrl = null;
        public static bool HasUnfinishedCall = false;
        public static Guid CallId = default;

        public static bool IsIncomingCall { get; set; } = false;

        public ICommand CallCommand { get; }
        public ICommand BrowserLoadedCommand { get; }

        // FIXME
        public ICommand RefreshCommand
		{
            get;
            set;
		}

        private int unreadMessageCount;
        public int UnreadMessageCount
        {
            get
            {
                return unreadMessageCount;
            }

            set
            {
                // FIXME
                /*
                if (unreadMessageCount > 0)
                {
                    unreadMessageText.Visibility = Visibility.Visible;
                }
                else
                {
                    unreadMessageText.Visibility = Visibility.Collapsed;
                }
                */

                SetProperty(ref unreadMessageCount, value);
            }
        }

		private int missedCallCount;
		public int MissedCallCount
		{
			get
			{
				return missedCallCount;
            }

            set
            {
                // FIXME
                /*
                if (missedCallCount > 0)
                {
                    MissedCallText.Visibility = Visibility.Visible;
                }
                else
                {
                    MissedCallText.Visibility = Visibility.Collapsed;
                }
                */

                SetProperty(ref missedCallCount, value);
            }
        }

        private string addressBoxText;
		public string AddressBoxText
        {
            get { return addressBoxText; }
            set
            {
                SetProperty(ref addressBoxText, value);
            }
        }


		public void OnNavigatedTo(NavigationEventArgs e)
		{
			LinphoneManager.Instance.CoreDispatcher = CoreApplication.GetCurrentView().CoreWindow.Dispatcher;
            LinphoneManager.Instance.RegistrationChanged += RegistrationChanged;
            LinphoneManager.Instance.MessageReceived += MessageReceived;
            LinphoneManager.Instance.CallStateChangedEvent += CallStateChanged;
            RefreshCommand.Execute(this);
            /*    if (e.NavigationMode == NavigationMode.New)
                {
                    if (BugCollector.HasExceptionToReport())
                    {
                        // Allow to report exceptions before the creation of the core in case the problem is in there
                        CustomMessageBox reportIssueDialog = new CustomMessageBox()
                        {
                            Caption = AppResources.ReportCrashDialogCaption,
                            Message = AppResources.ReportCrashDialogMessage,
                            LeftButtonContent = AppResources.ReportCrash,
                            RightButtonContent = AppResources.Close
                        };

                        reportIssueDialog.Dismissed += (s, ev) =>
                        {
                            switch (ev.Result)
                            {
                                case CustomMessageBoxResult.LeftButton:
                                    BugReportUploadProgressBar.Minimum = 0;
                                    BugReportUploadProgressBar.Maximum = 100;
                                    BugReportUploadPopup.Visibility = Visibility.Visible;
                                    LinphoneManager.Instance.LogUploadProgressIndicationEH += LogUploadProgressIndication;
                                    LinphoneManager.Instance.LinphoneCore.UploadLogCollection();
                                    break;
                                case CustomMessageBoxResult.RightButton:
                                    BugCollector.DeleteFile();
                                    break;
                            }
                        };

                        reportIssueDialog.Show();
                    }
                    else
                    {
                        BugReportUploadPopup.Visibility = Visibility.Collapsed;
                    }
                }*/

            if (LinphoneManager.Instance.Core.CallsNb > 0)
            {
                Call call = LinphoneManager.Instance.Core.CurrentCall;
                if( call != null){
                    List<String> parameters = new List<String>();
                    parameters.Add(call.RemoteAddress.AsStringUriOnly());
                    navigationService.Navigate<global::Linphone.Views.InCall>(parameters);
                }
            }

            if (LinphoneManager.Instance.GetUnreadMessageCount() > 0)
            {
                UnreadMessageCount = LinphoneManager.Instance.GetUnreadMessageCount();
            }

            if (LinphoneManager.Instance.Core.MissedCallsCount > 0)
            {
                MissedCallCount = LinphoneManager.Instance.Core.MissedCallsCount;
            }

            if (e.Parameter is String && (e.Parameter as String)?.Length > 0 && e.NavigationMode != NavigationMode.Back)
            {
                String arguments = e.Parameter as String;
                AddressBoxText = arguments;
                try
                {
                    Address address = LinphoneManager.Instance.Core.InterpretUrl(e.Parameter as String);
                    String sipAddressToCall = address.AsStringUriOnly();
                    AddressBoxText = sipAddressToCall;
                }
                catch (Exception exception)
                {
                }
            }

		}

		/// <summary>
		/// Raises right after page unloading
		/// </summary>
		/// <param name="e"></param>
		public void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
            _logger.Debug("OnNavigatingFrom");

            try
            {
                if (Browser.Source.OriginalString.Length > CallFlowControl.Instance.AgentProfile.PanelBaseUrl.Length)
                    CallFlowControl.Instance.AgentProfile.BrowsingHistory = Browser.Source.OriginalString.Substring(CallFlowControl.Instance.AgentProfile.PanelBaseUrl.Length);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while updating dialer browser's history.");
                CallFlowControl.Instance.AgentProfile.BrowsingHistory = "";
            }
		}

        private void RegistrationChanged(ProxyConfig config, RegistrationState state, string message)
        {
            RefreshCommand.Execute(null);
        }

        private void MessageReceived(ChatRoom room, ChatMessage message)
        {
            UnreadMessageCount = LinphoneManager.Instance.GetUnreadMessageCount();
        }

        private void CallStateChanged(Call call, global::Linphone.CallState state)
        {
            MissedCallCount = LinphoneManager.Instance.Core.MissedCallsCount;
        }

        private async void CallClick()
        {
            if (!CallFlowControl.Instance.AgentProfile.IsLoggedIn) return;

            if (CallFlowControl.Instance.CallContext.Direction == CallDirection.Command)
            {
                _logger.Information("Cant start a call because of a running command.");
                return;
            }

            if (CallFlowControl.Instance.AgentProfile.JoinedIntoIncomingCallQueue)
            {
                CallFlowControl.Instance.LeaveIncomingCallQueue();
                TimeSpan timeout = TimeSpan.FromMilliseconds(5000);
                while (true)
                {
                    timeout = timeout - TimeSpan.FromMilliseconds(500);
                    if (timeout.TotalMilliseconds <= 0 || CallFlowControl.Instance.CallContext.Direction != CallDirection.Command)
                    {
                        break;
                    }

                    await Task.Delay(500);
                }
            }

            if (CallFlowControl.Instance.CallContext.Direction == CallDirection.Command)
            {
                _logger.Information("Cant start a call because of a still running command.");
                _logger.Information("Critical Situation.");

                CallFlowControl.Instance.JoinIntoIncomingCallQueue();

                return;
            }

            if (CallFlowControl.Instance.CallContext.CallState != BelledonneCommunications.Linphone.Core.CallState.Ready)
            {
                _logger.Information("Cant start a call, phone is in {State} state.", CallFlowControl.Instance.CallContext.CallState.ToString("g"));
                return;
            }

            if (addressBox.Text.Length > 0)
            {
                string inboundService;
                if (OutgoingChannel.SelectedIndex == 0)
                {
                    inboundService = HEAD_OF_HOUSEHOLD_SERVICE;
                }
                else
                {
                    inboundService = SELLERS_SERVICE_PHONENUMBER;
                }

                string normalizedAddres = addressBox.Text;
                if (!normalizedAddres.StartsWith("00"))
                {
                    if (normalizedAddres.StartsWith('0'))
                        normalizedAddres = "0" + normalizedAddres;
                    else
                        normalizedAddres = "00" + normalizedAddres;
                }

                await CallFlowControl.Instance.InitiateOutgoingCallAsync(normalizedAddres.Substring(EXTRA_ZERO_CORRECTION_INDEX), inboundService);

                LinphoneManager.Instance.NewOutgoingCall($"{inboundService}*{normalizedAddres}");
            }
        }

        private void OnLoadedBrowser()
		{
            // HotPoint #4
            if (!string.IsNullOrWhiteSpace(CallFlowControl.Instance.AgentProfile.BrowsingHistory)
                && !CallFlowControl.Instance.AgentProfile.BrowsingHistory.StartsWith("/Account/Login"))
            {
                SourceUri = new Uri($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}{CallFlowControl.Instance.AgentProfile.BrowsingHistory}");
            }
            else
            {
                SourceUri = new Uri(CallFlowControl.Instance.AgentProfile.PanelBaseUrl);
            }
		}

        public static String StripUnicodeCharactersFromString(string inputValue)
        {
            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(String.Empty), new DecoderExceptionFallback()), Encoding.UTF8.GetBytes(inputValue)));
        }

        private readonly INavigationService navigationService;
        private readonly HttpClient httpClient;
        private readonly ILogger _logger;
	}
}
