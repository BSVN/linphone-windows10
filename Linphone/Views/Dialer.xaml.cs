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

using BelledonneCommunications.Linphone.Commons;
using BelledonneCommunications.Linphone.Core;
using BelledonneCommunications.Linphone.Dialogs;
using BelledonneCommunications.Linphone.Presentation.Dto;
using Linphone.Model;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Windows.Storage;
using System.Net.Http;
using BelledonneCommunications.Linphone.Presentation.Dto;
using System.Diagnostics;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;
using BSN.Resa.Mci.CallCenter.AgentApp.Data;
using StackExchange.Redis;
using BelledonneCommunications.Linphone.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{

    public sealed partial class Dialer : Page
    {
        public static string CallerId = null;

        public static string CalleeId = null;

        public static bool IsLoggedIn = false;


        private DatabaseFactory databaseFactory;
        private readonly HttpClient httpClient;
        private readonly ApplicationSettingsManager _settings = new ApplicationSettingsManager();

		public Dialer()
        {
            this.InitializeComponent();
            httpClient = new HttpClient();

			DataContext = Ioc.Default.GetRequiredService<DialerViewModel>();
            ViewModel.RefreshCommand = status.RefreshCommand;

            _logger = Log.Logger.ForContext("SourceContext", nameof(Dialer));

            ContactsManager contactsManager = ContactsManager.Instance;

            addressBox.KeyDown += (sender, args) =>
            {
                if (args.Key == Windows.System.VirtualKey.Enter)
                {
                    ViewModel.CallCommand.Execute(null);
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
        }

        public DialerViewModel ViewModel => (DialerViewModel)DataContext;

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);
            ViewModel.OnNavigatingFrom(e);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

            // TODO: Please remove it
            if (!await Utility.IsMicrophoneAvailable())
            {
                var micrphonePermissionDialog = new MicrophonePermissionRequestDialog();
                await micrphonePermissionDialog.ShowAsync();
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"));
            }

            ViewModel.OnNavigatedTo(e);
		}

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
            var passwordDialog = new SettingsPasswordDialog();
            await passwordDialog.ShowAsync();
            if (passwordDialog.Password == "Noei@Sip#")
            {
                Frame.Navigate(typeof(Views.Settings), null);
            }
            else
            {
                _logger.Information("Unsuccessful attempt to enter settings password with: {Password}", passwordDialog.Password);
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

        private void Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(String.Empty), new DecoderExceptionFallback()), Encoding.UTF8.GetBytes(inputValue)));
        }

        private async void Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _logger.Information("Browser path: {Path}.", sender.Source.AbsolutePath);

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
            else if (sender.Source.AbsolutePath.Contains("Dashboard") && CallFlowControl.Instance.AgentProfile.IsLoggedIn == true)
            {
                if (string.IsNullOrWhiteSpace(CallFlowControl.Instance.AgentProfile.SipPhoneNumber))
                {
                    _logger.Information("Backup solution for loading sip settings.");

                    string html = await Browser.CoreWebView2.ExecuteScriptAsync("document.body.outerHTML");
                    string pattern = "\\\\u003Cinput\\s*name=\\\\\"Username\\\\\"\\s*type=\\\\\"hidden\\\\\"\\s*value=\\\\\"(\\w|-)*\\\\\">";

                    Regex regex = new Regex(pattern);
                    MatchCollection matches = regex.Matches(html);
                    Match match = matches.FirstOrDefault();
                    string a = ViewModel.StripUnicodeCharactersFromString(html);

                    if (match != null)
                    {
                        var matchedValue = "";
                        try
                        {
                            matchedValue = match.Value;
                            matchedValue = matchedValue.Replace(" ", "").Replace("\\u003Cinput name=\\\"Username\\\" type=\\\"hidden\\\" value=\\\"".Replace(" ", ""), "");
                            matchedValue = matchedValue.Replace("\\\">", "");
                            matchedValue = matchedValue.Trim();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Backup solution for loading sip setting is not working at all !.");
                        }

                        var userSettings = await CallFlowControl.Instance.GetAgentSettingByUserId(matchedValue);
                        CallFlowControl.Instance.AgentProfile.SipPhoneNumber = userSettings.Data.SipProfile.Username;

                        LoadSipSettings(userSettings.Data.SipProfile);

                        AgentStatus.IsEnabled = true;

                        await AgentStatus.Dispatcher.RunIdleAsync(P =>
                        {
                            AgentStatus.SelectedIndex = 0;
                        });
                    }
                    else
                    {
                        _logger.Error("Our backup solution has been fucked up !!!.");
                    }
                }
            }
            // TODO: It will never works on first try in production !!! 
            else if (sender.Source.AbsolutePath.Contains("/api/Operators/UserInfo"))
            {
                // Literally کثافتکاری
                // Todo: Interactions with Browser should be revised.
                // This try and error may not works in some ways. i placed another backup solution for loading sip settings on Dashboard navigation completion.
                try
                {
                    if (UserInfoRetryLimit == 0)
                    {
                        Browser.CoreWebView2.Navigate($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}");
                        return;
                    }

                    string html = "";
                    string sipPhoneNumber = "";

                    try
                    {
                        html = await Browser.CoreWebView2.ExecuteScriptAsync("document.body.outerHTML");
                        string content = html.Substring(html.IndexOf("sipProfile"));
                        content = content.Substring(content.IndexOf("username"));
                        content = content.Substring(content.IndexOf(":") + 1);
                        sipPhoneNumber = content.Substring(0, content.IndexOf(",")).Replace("\"", "").Replace("\\", "");
                        _logger.Information("Successfully catched the sip number.");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "UserInfo parsing problem.");
                        _logger.Error("Loaded Html content: {Html}", html);

                        UserInfoRetryLimit -= 1;
                        Browser.CoreWebView2.Navigate($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}/api/Operators/UserInfo");
                        return;
                    }

                    CallFlowControl.Instance.AgentProfile.SipPhoneNumber = sipPhoneNumber;

                    LoadSipSettings();

                    AgentStatus.IsEnabled = true;

                    await AgentStatus.Dispatcher.RunIdleAsync(P =>
                    {
                        AgentStatus.SelectedIndex = 0;
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while reading sip phonenumber.");
                }

                Browser.CoreWebView2.Navigate($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}");
            }
        }

        private async void LoadSipSettings()
        {
            await LinphoneManager.Instance.CoreDispatcher.RunIdleAsync((args) =>
            {
                UpdateSettings();
            });
        }

        private async void LoadSipSettings(SipProfileViewModel sipProfileViewModel)
        {
            await LinphoneManager.Instance.CoreDispatcher.RunIdleAsync((args) =>
            {
                UpdateSettings(sipProfileViewModel);
            });
        }

        private async void UpdateSettings(SipProfileViewModel sipProfileViewModel)
        {
            try
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    SIPAccountSettingsManager settings = new SIPAccountSettingsManager();
                    settings.Update(sipProfileViewModel);
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while updating agent settings.");
            }
        }

        private async void UpdateSettings()
        {
            try
            {
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    OperatorsQueryServiceGetBySoftPhoneNumberResponse agentSettings = await CallFlowControl.Instance.GetAgentSettings();
                    SIPAccountSettingsManager settings = new SIPAccountSettingsManager();

                    settings.Load();

                    settings.Username = string.IsNullOrWhiteSpace(agentSettings.Data.SipProfile.Username) ? "" : agentSettings.Data.SipProfile.Username;
                    settings.UserId = string.IsNullOrWhiteSpace(agentSettings.Data.SipProfile.UserId) ? "" : agentSettings.Data.SipProfile.UserId;
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

            /*
            if (Database == null)
                return;

            while (true)
            {
                SortedSetEntry? callback = Database.SortedSetPop(QUEUE_NAME);
                if (callback == null || !callback.HasValue)
                    break;

                Database.SortedSetAdd(QUEUE_NAME, callback.Value.Element, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

				// Add 0 as a prefix to ensure number starts with 00 (for PSTN call it is mandatory)
				if (_settings.OutgoingCallEnabled)
				{
					string number = "0" + callback.Value.Element;
					LinphoneManager.Instance.NewOutgoingCall(number);
				}
                else
                {
					string number = callback.Value.Element;
					LinphoneManager.Instance.NewOutgoingCall(number);
				}	
                Call call = LinphoneManager.Instance.GetCurrentCall();
                if (call == null)
                    break;
                if (call.State == CallState.End)
                    Debug.WriteLine($"Call to {call.ToAddress} is ended");
                break;
			}
            */
		}

        private async void AgentStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AgentStatus.SelectedIndex == 0)
                {
                    await CallFlowControl.Instance.UpdateAgentStatusAsync(BelledonneCommunications.Linphone.Presentation.Dto.AgentStatus.Ready);
                }
                else
                {
                    await CallFlowControl.Instance.UpdateAgentStatusAsync(BelledonneCommunications.Linphone.Presentation.Dto.AgentStatus.Break);
                }

                Browser.CoreWebView2.Navigate($"{CallFlowControl.Instance.AgentProfile.PanelBaseUrl}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Internal error while updating agent status.");
            }
        }


        private const string HEAD_OF_HOUSEHOLD_SERVICE = "99970";
        private const string SELLERS_SERVICE_PHONENUMBER = "99971";
        private const int EXTRA_ZERO_CORRECTION_INDEX = 1;
        private static int UserInfoRetryLimit = 4;

        private readonly ILogger _logger;
        private readonly ApplicationSettingsManager _settings = new ApplicationSettingsManager();
    }
}