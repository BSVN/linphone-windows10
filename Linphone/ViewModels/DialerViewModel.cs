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
            if (SourceUri.OriginalString.Length > BrowserBaseUrl.Length)
                BrowserCurrentUrlOffset = SourceUri.OriginalString.Substring(BrowserBaseUrl.Length);
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
            if (HasUnfinishedCall)
            {
                var response = await httpClient.GetAsync($"{BrowserBaseUrl}/api/Calls/{CallId}");
                var result = response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceGetByIdResponse>();
                if (!result.Result.Data.CallReason.HasValue && !result.Result.Data.TicketId.HasValue)
                    return;
                else
                    HasUnfinishedCall = false;
            }

            if (AddressBoxText.Length > 0)
            {
                IsIncomingCall = false;
                LinphoneManager.Instance.NewOutgoingCall(AddressBoxText);
            }
            else
            {
                string lastDialedNumber = LinphoneManager.Instance.GetLastCalledNumber();
                AddressBoxText = lastDialedNumber == null ? "" : lastDialedNumber;
            }
        }

        private void OnLoadedBrowser()
		{
            if (BrowserCurrentUrlOffset != null && !BrowserCurrentUrlOffset.StartsWith("/Account/Login"))
            {
                object settingValue = localSettings.Values["PanelUrl"];
                BrowserBaseUrl = settingValue == null ? "http://localhost:9011" : settingValue as string;
                SourceUri = new Uri($"{BrowserBaseUrl}{BrowserCurrentUrlOffset}");
            }
            else
            {
                object settingValue = localSettings.Values["PanelUrl"];
                string loadingUrl = settingValue == null ? "http://localhost:9011" : settingValue as string;
                SourceUri = new Uri(loadingUrl);
                BrowserBaseUrl = loadingUrl;
            }
		}

        private readonly INavigationService navigationService;
        private readonly HttpClient httpClient;
	}
}
