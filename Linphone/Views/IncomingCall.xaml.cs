/*
IncomingCall.xaml.cs
Copyright (C) 2016  Belledonne Communications, Grenoble, France
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

using System;
using Linphone.Model;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Core;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using BelledonneCommunications.Linphone.Presentation.Dto;
using Serilog;
using System.Threading.Tasks;

namespace Linphone.Views
{

    public partial class IncomingCall : Page
    {
        private String _callingNumber;
        private HttpClient httpClient;
        private string UserId;

        public IncomingCall()
        {
            this.InitializeComponent();

            SIPAccountSettingsManager _settings = new SIPAccountSettingsManager();
            _settings.Load();
            UserId = _settings.UserId;

            httpClient = new HttpClient();

            SystemNavigationManager.GetForCurrentView().BackRequested += Back_requested;
            if (!LinphoneManager.Instance.Core.VideoSupported() || !LinphoneManager.Instance.Core.VideoCaptureEnabled)
            {
                //AnswerVideo.Visibility = Visibility.Collapsed;
            }
        }

        private void Back_requested(object sender, BackRequestedEventArgs e)
        {
            LinphoneManager.Instance.EndCurrentCall();
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        /// <summary>
        /// Remove this entry from the back stack to ensure the user won't navigate to it with the back button
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //if (!e.IsNavigationInitiator)
            //{
            //if we leave the application, we consider it as a call rejection
            // LinphoneManager.Instance.EndCurrentCall();
            //}

            base.OnNavigatedFrom(e);
            if (Frame.BackStack.Count > 0)
            {
                Frame.BackStack.Clear();
            }
        }

        /// <summary>
        /// Method called when the page is displayed.
        /// Searches for a matching contact using the current call address or number and display information if found.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);

            if ((nee.Parameter as String).Contains("sip"))
            {
                _callingNumber = (nee.Parameter as String);

                Address address = LinphoneManager.Instance.Core.InterpretUrl(_callingNumber);
                Dialer.CallId = default;
                Dialer.CallerId = address.Username;
                Dialer.CalleeId = UserId;
                Dialer.IsIncomingCallAnswered = false;

                if (_callingNumber.StartsWith("sip:"))
                {
                    _callingNumber = _callingNumber.Substring(4);
                }

                // While we dunno if the number matches a contact one, we consider it won't and we display the phone number as username
                Contact.Text = _callingNumber;

                if (_callingNumber != null && _callingNumber.Length > 0)
                {
                    //ContactManager cm = ContactManager.Instance;
                    //cm.ContactFound += cm_ContactFound;
                    //cm.FindContact(_callingNumber);
                }

                Log.Debug("Initiate a new call.");
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"{Dialer.BrowserBaseUrl}/api/Calls/InitiateIncoming?CustomerPhoneNumber={address.Username}&OperatorSoftPhoneNumber={UserId}");
                    var result = response.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateIncomingResponse>();
                    if (result != null)
                    {
                        Dialer.IsIncomingCall = true;
                        Dialer.CallId = result.Result.Data.Id;
                    }
                }
                catch
                {
                    Log.Debug("Exception in iniating a new call.");
                }

                Log.Debug("Call Initiation finished.");
            }
        }

        /// <summary>
        /// Callback called when the search on a phone number for a contact has a match
        /// </summary>
      /*  private void cm_ContactFound(object sender, ContactFoundEventArgs e)
        {
            if (e.ContactFound != null)
            {
                Contact.Text = e.ContactFound.DisplayName;
                if (e.PhoneLabel != null)
                {
                    Number.Text = e.PhoneLabel + " : " + e.PhoneNumber;
                }
                else
                {
                    Number.Text = e.PhoneNumber;
                }
            }
        }*/

        private async void Answer_Click(object sender, RoutedEventArgs e)
        {
            if (LinphoneManager.Instance.Core.CurrentCall != null)
            {
                Dialer.IsIncomingCallAnswered = true;

                if (Dialer.CallId != default)
                {
                    try
                    {
                        Address address = LinphoneManager.Instance.Core.InterpretUrl(_callingNumber);
                        Task<HttpResponseMessage> task = httpClient.GetAsync($"{Dialer.BrowserBaseUrl}/api/Calls/AcceptIncoming/{Dialer.CallId}");
                        task.ContinueWith(P =>
                        {
                            if (task.Exception != null)
                            {
                                Log.Error(task.Exception, "Exception during Accepting an incoming call.");
                            }
                            else
                            {
                                var result = task.Result.Content.ReadAsAsyncCaseInsensitive<CallsCommandServiceInitiateIncomingResponse>();
                                Log.Information("Accept incoming call response payload {Payload}.", result.SerializeToJson());
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Answer click event exception.");
                    }
                }

                List<string> parameters = new List<string>();
                parameters.Add(_callingNumber);
                parameters.Add("incomingCall");
                Frame.Navigate(typeof(Views.InCall), parameters);
            }
        }

        private void AnswerVideo_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void Decline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Address address = LinphoneManager.Instance.Core.InterpretUrl(_callingNumber);
                Task<HttpResponseMessage> task = httpClient.GetAsync($"{Dialer.BrowserBaseUrl}/api/Calls/MissedIncoming?CustomerPhoneNumber={address.Username}&OperatorSoftPhoneNumber={UserId}");
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

                Dialer.IsIncomingCall = false;
                Dialer.CallId = default;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception during declining a call.");
            }

            LinphoneManager.Instance.EndCurrentCall();
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}