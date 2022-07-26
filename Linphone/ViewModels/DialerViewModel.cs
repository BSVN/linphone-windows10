using BelledonneCommunications.Linphone.Commons;
using BelledonneCommunications.Linphone.Core;
using BelledonneCommunications.Linphone.Messages;
using BelledonneCommunications.Linphone.Presentation.Dto;
using BSN.Resa.Mci.CallCenter.AgentApp.Data;
using BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Linphone;
using Linphone.Model;
using Linphone.Views;
using PCLAppConfig;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        public DialerViewModel(INavigationService navigationService, ICallbackQueue callbackQueue)
        {
            this.navigationService = navigationService;
            this.callbackQueue = callbackQueue;
            CallCommand = new RelayCommand(CallClick);
            CallbackCommand = new RelayCommand(CallbackClick);
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

        private int outgoingChannelSelectedIndex;
		public int OutgoingChannelSelectedIndex
        {
            get => outgoingChannelSelectedIndex;
            set => SetProperty(ref this.outgoingChannelSelectedIndex, value);
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
        public ICommand CallbackCommand { get; }
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

            LinphoneManager.Instance.RegistrationChanged -= RegistrationChanged;
            LinphoneManager.Instance.MessageReceived -= MessageReceived;
            LinphoneManager.Instance.CallStateChangedEvent -= CallStateChanged;

            try
            {
                if (SourceUri.OriginalString.Length > CallFlowControl.Instance.AgentProfile.PanelBaseUrl.Length)
                    CallFlowControl.Instance.AgentProfile.BrowsingHistory = SourceUri.OriginalString.Substring(CallFlowControl.Instance.AgentProfile.PanelBaseUrl.Length);
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

        private async void CallbackClick()
		{
            bool joinedCallQueue = CallFlowControl.Instance.AgentProfile.JoinedIntoIncomingCallQueue;
            if (await PreparingOutgoingCall() == false)
                return;

            while (true)
            {
                CallbackDto callback = callbackQueue.Pop();
                if (callback is null)
                    break;

                CallFlowControl.Instance.CallContext.CallbackRequest = callback;

                string normalizedAddress = callback.caller_number.GetCanonicalPhoneNumber();

                await CallFlowControl.Instance.InitiateCallbackAsync(calleePhoneNumber: normalizedAddress, inboundService: callback.callee_number, requestedAt: callback.RequestedAt);

                BSN.LinphoneSDK.Call outgoingCall = await LinphoneManager.Instance.NewOutgoingCall($"{callback.callee_number}*0{normalizedAddress}");
                
                await outgoingCall.WhenEnded();

                // TODO: It is mandatory for backing to Dialer from InCall, but it is very bugous and must fix it
				await Task.Delay(500);
                Task<CancellationToken> cancellationTokenTask = WeakReferenceMessenger.Default.Send<ContinueCallbackAnsweringRequestMessage>();
                CancellationToken cancellationToken = await cancellationTokenTask;
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            if (joinedCallQueue)
                CallFlowControl.Instance.JoinIntoIncomingCallQueue();
        }

        private async void CallClick()
        {
            if (await PreparingOutgoingCall() == false)
                return;

            if (AddressBoxText.Length > 0)
            {
                string inboundService;
                if (OutgoingChannelSelectedIndex == 0)
                {
                    inboundService = HEAD_OF_HOUSEHOLD_SERVICE;
                }
                else
                {
                    inboundService = SELLERS_SERVICE_PHONENUMBER;
                }

                string normalizedAddres = AddressBoxText;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if prepared for ougoing call, otherwise return false</returns>
        private async Task<bool> PreparingOutgoingCall()
		{
            bool isInHomeTesting = Convert.ToBoolean(ConfigurationManager.AppSettings["InHomeTesting"]);

            if (!isInHomeTesting && !CallFlowControl.Instance.AgentProfile.IsLoggedIn) return false;

            if (CallFlowControl.Instance.CallContext.CallType == CallType.Command)
            {
                _logger.Information("Cant start a call because of a running command.");
                return false;
            }

            if (!isInHomeTesting && CallFlowControl.Instance.AgentProfile.JoinedIntoIncomingCallQueue)
            {
                CallFlowControl.Instance.LeaveIncomingCallQueue();
                TimeSpan timeout = TimeSpan.FromMilliseconds(5000);
                while (true)
                {
                    timeout = timeout - TimeSpan.FromMilliseconds(500);
                    if (timeout.TotalMilliseconds <= 0 || CallFlowControl.Instance.CallContext.CallType != CallType.Command)
                    {
                        break;
                    }

                    await Task.Delay(500);
                }
            }

            if (CallFlowControl.Instance.CallContext.CallType == CallType.Command)
            {
                _logger.Information("Cant start a call because of a still running command.");
                _logger.Information("Critical Situation.");

                CallFlowControl.Instance.JoinIntoIncomingCallQueue();

                return false;
            }

            if (CallFlowControl.Instance.CallContext.CallState != BelledonneCommunications.Linphone.Core.CallState.Ready)
            {
                _logger.Information("Cant start a call, phone is in {State} state.", CallFlowControl.Instance.CallContext.CallState.ToString("g"));
                return false;
            }

            return true;
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
		private readonly ICallbackQueue callbackQueue;
		private readonly HttpClient httpClient;
        private readonly ILogger _logger;

        private const string HEAD_OF_HOUSEHOLD_SERVICE = "99970";
        private const string SELLERS_SERVICE_PHONENUMBER = "99971";
        private const int EXTRA_ZERO_CORRECTION_INDEX = 1;
	}
}
