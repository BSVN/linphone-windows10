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
using Linphone;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Windows.Storage;
using System.Net.Http;
using BelledonneCommunications.Linphone.Presentation.Dto;
using System.Diagnostics;
using Windows.UI.Popups;
using BSN.Resa.Mci.CallCenter.AgentApp.Data;
using StackExchange.Redis;
using BelledonneCommunications.Linphone.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;

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

            ContactsManager contactsManager = ContactsManager.Instance;
            addressBox.KeyDown += (sender, args) =>
            {
                if (args.Key == Windows.System.VirtualKey.Enter)
                {
                    ViewModel.CallCommand.Execute(null);
                }
            };

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
            var result = await AdminPasswordDialog.ShowAsync();
            if (AdminPassword.Password == "Noei@Sip#")
            {
                Frame.Navigate(typeof(Views.Settings), null);
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
            if (IsLoggedIn)
                LinphoneManager.Instance.Core.RefreshRegisters();
        }

        private void Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            if (sender.Source.AbsolutePath == "/Account/Login")
            {
                IsLoggedIn = false;
                DisableRegisteration();
            }
            else if (sender.Source.AbsolutePath.Contains("Dashboard"))
            {
                IsLoggedIn = true;
                EnableRegister(true);
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
                    System.Threading.Tasks.Task.Delay(100);
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

		private DatabaseFactory DatabaseFactory
		{
			get
			{
				if (databaseFactory == null)
				{
					_settings.Load();
					try
					{
						databaseFactory = new DatabaseFactory((_settings.RedisConnectionString ?? throw new ArgumentNullException("Redis connection string is null")) == "" ? "localhost" : _settings.RedisConnectionString);
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
				return databaseFactory;
			}
		}
	}
}