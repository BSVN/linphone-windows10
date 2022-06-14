/*
Dialer.xaml.cs
Copyright (C) 2022 Resaa Corporation.
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using Windows.UI.Xaml.Controls;
using Linphone.Model;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using StackExchange.Redis;
using Windows.UI.Popups;
using BelledonneCommunications.Linphone.Core;
using Serilog;
using System.Threading.Tasks;
using BelledonneCommunications.Linphone.Presentation.Dto;

namespace Linphone.Views
{

    public sealed partial class Dialer : Page, INotifyPropertyChanged
    {
        public Dialer()
        {
            this.InitializeComponent();
            DataContext = this;
            
            _logger = Log.Logger.ForContext("SourceContext", nameof(Dialer));

            ContactsManager contactsManager = ContactsManager.Instance;

            addressBox.KeyDown += (sender, args) =>
            {
                if (args.Key == Windows.System.VirtualKey.Enter)
                {
                    call_Click(null, null);
                }
            };


            if (CallFlowControl.Instance.AgentProfile.IsLoggedIn)
            {
                AgentStatus.SelectionChanged -= AgentStatus_SelectionChanged;
                AgentStatus.IsEnabled = true;
                if (CallFlowControl.Instance.AgentProfile.Status == BelledonneCommunications.Linphone.Presentation.Dto.AgentStatus.Ready)
                {
                    AgentStatus.SelectedValue = OnlineAgentComboItem;
                }
                else if (CallFlowControl.Instance.AgentProfile.Status == BelledonneCommunications.Linphone.Presentation.Dto.AgentStatus.Break)
                {
                    AgentStatus.SelectedValue = OnBreakAgentComboItem;
                }

                AgentStatus.SelectionChanged += AgentStatus_SelectionChanged;
            }

            // TODO: WebView FixedRuntime Approach make installation easier.
            //CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions();
            //StorageFolder localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            //string path = Path.Combine(localFolder.Path, "Assets\\FixedRuntime\\102.0.1245.33_x64");
            //CoreWebView2Environment env = CoreWebView2Environment.CreateWithOptionsAsync(path, "", options).GetResults();

            //Browser.EnsureCoreWebView2Async().GetResults();
        }

        /// <summary>
        /// Raises right after page unloading 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
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

            base.OnNavigatingFrom(e);
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
                unreadMessageCount = value;
                if (unreadMessageCount > 0)
                {
                    unreadMessageText.Visibility = Visibility.Visible;
                }
                else
                {
                    unreadMessageText.Visibility = Visibility.Collapsed;
                }

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("UnreadMessageCount"));
                }
            }
        }

        private ConnectionMultiplexer RedisConnectionMultiplexer
        {
            get
            {
                if (connectionMultiplexer == null)
                {
                    _settings.Load();
                    try
                    {
                        connectionMultiplexer = ConnectionMultiplexer.Connect((_settings.RedisConnectionString ?? throw new ArgumentNullException("Redis connection string is null")) == "" ? "localhost" : _settings.RedisConnectionString);
                    }
                    catch (RedisConnectionException e)
                    {
                        Debug.WriteLine(e.Message);
                        var messageDialog = new MessageDialog("ارتباط با پایگاه داده ردیس برقرار نگردید، لطفا تنظیمات را مجددا بررسی کنید و یا از ارتباط شبکه مطمئن گردید");
                        messageDialog.Commands.Add(new UICommand("باشه"));
                        messageDialog.DefaultCommandIndex = 0;
                        messageDialog.CancelCommandIndex = 0;
                        messageDialog.ShowAsync();
                    }
                }

                return connectionMultiplexer;
            }
        }

        private IDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = RedisConnectionMultiplexer?.GetDatabase();
                }

                return database;
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
                missedCallCount = value;
                if (missedCallCount > 0)
                {
                    MissedCallText.Visibility = Visibility.Visible;
                }
                else
                {
                    MissedCallText.Visibility = Visibility.Collapsed;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MissedCallCount"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void LogUploadProgressIndication(int offset, int total)
        {
            /* base.UIDispatcher.BeginInvoke(() =>
             {
                 BugReportUploadProgressBar.Maximum = total;
                 if (offset <= total)
                 {
                     BugReportUploadProgressBar.Value = offset;
                 }
             });*/
        }

        private void RegistrationChanged(ProxyConfig config, RegistrationState state, string message)
        {
            status.RefreshStatus();
        }

        private void MessageReceived(ChatRoom room, ChatMessage message)
        {
            UnreadMessageCount = LinphoneManager.Instance.GetUnreadMessageCount();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LinphoneManager.Instance.CoreDispatcher = Dispatcher;
            LinphoneManager.Instance.RegistrationChanged += RegistrationChanged;
            LinphoneManager.Instance.MessageReceived += MessageReceived;
            LinphoneManager.Instance.CallStateChangedEvent += CallStateChanged;
            status.RefreshStatus();
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
                if (call != null)
                {
                    List<String> parameters = new List<String>();
                    parameters.Add(call.RemoteAddress.AsStringUriOnly());
                    Frame.Navigate(typeof(Views.InCall), parameters);
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
                addressBox.Text = arguments;
                try
                {
                    Address address = LinphoneManager.Instance.Core.InterpretUrl(e.Parameter as String);
                    String sipAddressToCall = address.AsStringUriOnly();
                    addressBox.Text = sipAddressToCall;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Linphone raised exception.");
                }
            }
        }

        private void CallStateChanged(Call call, CallState state)
        {
            MissedCallCount = LinphoneManager.Instance.Core.MissedCallsCount;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);
            // LinphoneManager.Instance.LogUploadProgressIndicationEH -= LogUploadProgressIndication;
            // BugReportUploadPopup.Visibility = Visibility.Collapsed;
        }

        private async void call_Click(object sender, RoutedEventArgs e)
        {
            if (CallFlowControl.Instance.CallContext.Direction == CallDirection.Command)
            {
                _logger.Information("Cant start a call because of a running command.");
                return;
            }

            //if (CallFlowControl.Instance.CallContext.CallState != BelledonneCommunications.Linphone.Core.CallState.Ready)
            //{
            //    return;
            //}
            //else
            //{
            //    // HotPoint #6
            //    // TODO: Prepare for outgoing call.
            //}

            if (addressBox.Text.Length > 0)
            {
                LinphoneManager.Instance.NewOutgoingCall(addressBox.Text);
            }
            else
            {
                string lastDialedNumber = LinphoneManager.Instance.GetLastCalledNumber();
                addressBox.Text = lastDialedNumber == null ? "" : lastDialedNumber;
            }
        }

        private void numpad_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            String tag = button.Tag as String;
            LinphoneManager.Instance.Core.PlayDtmf(Convert.ToSByte(tag.ToCharArray()[0]), 1000);

            addressBox.Text += tag;
        }

        private void VoicemailClick(object sender, RoutedEventArgs e)
        {

        }

        private void zero_Hold(object sender, RoutedEventArgs e)
        {
            if (addressBox.Text.Length > 0)
                addressBox.Text = addressBox.Text.Substring(0, addressBox.Text.Length - 1);
            addressBox.Text += "+";
        }

        private void chat_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.Chats), null);
        }

        private void history_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.History), null);
        }

        private void contacts_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.ContactList), null);
        }

        private async void settings_Click(object sender, RoutedEventArgs e)
        {
            var result = await AdminPasswordDialog.ShowAsync();
            if (AdminPassword.Password == "Noei@Sip#")
            {
                Frame.Navigate(typeof(Views.Settings), null);
            }
            else
            {
                _logger.Information("Unsuccessful attempt to enter settings password with: {Password}", AdminPassword.Password);
            }
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.About), null);
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            EnableRegister(false);
        }

        private void connect_Click(object sender, EventArgs e)
        {
            EnableRegister(true);
        }

        private void EnableRegister(bool enable)
        {
            Core lc = LinphoneManager.Instance.Core;
            ProxyConfig cfg = lc.DefaultProxyConfig;
            if (cfg != null)
            {
                cfg.Edit();
                cfg.RegisterEnabled = enable;
                cfg.Done();
            }
        }

        private void status_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (CallFlowControl.Instance.AgentProfile.IsLoggedIn)
                LinphoneManager.Instance.Core.RefreshRegisters();
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            // HotPoint #4
            if (!string.IsNullOrWhiteSpace(CallFlowControl.Instance.AgentProfile.BrowsingHistory)
                && !CallFlowControl.Instance.AgentProfile.BrowsingHistory.StartsWith("/Account/Login"))
            {
                Browser.Source = new Uri($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}{CallFlowControl.Instance.AgentProfile.BrowsingHistory}");                               
            }
            else
            {
                Browser.Source = new Uri(CallFlowControl.Instance.AgentProfile.PanelBaseUrl);
            }
        }

        private async void Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            // HotPoint #5
            if (sender.Source.AbsolutePath == "/Account/Login")
            {
                CallFlowControl.Instance.AgentProfile.IsLoggedIn = false;
                DisableRegisteration();
            }
            else if (sender.Source.AbsolutePath.Contains("Dashboard") && CallFlowControl.Instance.AgentProfile.IsLoggedIn == false)
            {
                CallFlowControl.Instance.AgentProfile.IsLoggedIn = true;

                EnableRegister(true);

                Browser.CoreWebView2.Navigate($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}/api/Operators/UserInfo");
            }
            else if (sender.Source.AbsolutePath.Contains("/api/Operators/UserInfo"))
            {
                // Literally کثافتکاری                
                try
                {
                    var html = await Browser.CoreWebView2.ExecuteScriptAsync("document.body.outerHTML");
                    string content = html.Substring(html.IndexOf("sipProfile"));
                    content = content.Substring(content.IndexOf("username"));
                    content = content.Substring(content.IndexOf(":") + 1);

                    CallFlowControl.Instance.AgentProfile.SipPhoneNumber = content.Substring(0, content.IndexOf(",")).Replace("\"", "").Replace("\\", "");

                    AgentStatus.IsEnabled = true;

                    await AgentStatus.Dispatcher.RunIdleAsync(P =>
                    {
                        AgentStatus.SelectedIndex = 0;
                    });

                    LoadSipSettings();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while reading sip phonenumber.");
                }

                Browser.CoreWebView2.Navigate($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}");
            }
        }

        private void LoadSipSettings()
        {
            LinphoneManager.Instance.CoreDispatcher.RunIdleAsync((args) =>
            {
                UpdateSettings();
            });
        }

        private async Task UpdateSettings()
        {
            try
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                  {
                      OperatorsQueryServiceGetBySoftPhoneNumberResponse agentSettings = await CallFlowControl.Instance.GetAgentSettings();
                      SIPAccountSettingsManager settings = new SIPAccountSettingsManager();

                      settings.Load();

                      settings.Username = string.IsNullOrWhiteSpace(agentSettings.Data.SipProfile.Username) ? "" : agentSettings.Data.SipProfile.Username;
                      settings.UserId = string.IsNullOrWhiteSpace(agentSettings.Data.SipProfile.UserId) ? "": agentSettings.Data.SipProfile.UserId;
                      settings.Password = string.IsNullOrWhiteSpace(agentSettings.Data.SipProfile.Password) ? "" : agentSettings.Data.SipProfile.Password;
                      settings.Domain = string.IsNullOrWhiteSpace(agentSettings.Data.SipProfile.Domain) ? "10.19.82.3" : agentSettings.Data.SipProfile.Domain;
                      settings.Proxy = string.IsNullOrWhiteSpace(settings.Proxy) ? "" : settings.Proxy;
                      settings.OutboundProxy = settings.OutboundProxy;
                      settings.DisplayName = string.IsNullOrWhiteSpace(agentSettings.Data.SipProfile.Username) ? "" : agentSettings.Data.SipProfile.Username;
                      settings.Transports = (agentSettings.Data.SipProfile.Protocol == 0) ? "TCP" : agentSettings.Data.SipProfile.Protocol.ToString("g");
                      settings.Expires = string.IsNullOrWhiteSpace(settings.Expires) ? "500" : settings.Expires;
                      settings.AVPF = settings.AVPF;
                      settings.ICE = settings.ICE;

                      settings.Save();
                  });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while updating agent settings.");
            }
        }

        private static void DisableRegisteration()
        {
            Core lc = LinphoneManager.Instance.Core;
            ProxyConfig cfg = lc.DefaultProxyConfig;
            if (cfg != null)
            {
                cfg.Edit();
                cfg.RegisterEnabled = false;
                cfg.Done();

                // Wait for unregister to complete
                int timeout = 2000;
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (true)
                {
                    if (stopwatch.ElapsedMilliseconds >= timeout || cfg.State == RegistrationState.Cleared || cfg.State == RegistrationState.None)
                    {
                        break;
                    }
                    LinphoneManager.Instance.Core.Iterate();
                    Task.Delay(100);
                }
            }
        }

        private void Callback_Click(object sender, RoutedEventArgs e)
        {
            const string QUEUE_NAME = "cbq";

            if (Database == null)
                return;

            while (true)
            {
                SortedSetEntry? callback = Database.SortedSetPop(QUEUE_NAME);
                if (callback == null || !callback.HasValue)
                    break;

                Database.SortedSetAdd(QUEUE_NAME, callback.Value.Element, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                // Add 0 as a prefix to ensure number starts with 00 (for PSTN call it is mandatory)
                LinphoneManager.Instance.NewOutgoingCall("0" + callback.Value.Element);

                break;
            }
        }

        private async void AgentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AgentStatus.SelectedIndex == 0)
                {
                    await CallFlowControl.Instance.UpdateAgentStatusAsync(BelledonneCommunications.Linphone.Presentation.Dto.AgentStatus.Ready);
                    Browser.CoreWebView2.Reload();
                }
                else
                {
                    await CallFlowControl.Instance.UpdateAgentStatusAsync(BelledonneCommunications.Linphone.Presentation.Dto.AgentStatus.Break);
                    Browser.CoreWebView2.Reload();
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Internal error while updating agent status.");
            }
        }

        private readonly ILogger _logger;
        private ConnectionMultiplexer connectionMultiplexer;
        private IDatabase database;
        private readonly ApplicationSettingsManager _settings = new ApplicationSettingsManager();
    }
}