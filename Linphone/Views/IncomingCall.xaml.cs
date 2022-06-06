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
using BelledonneCommunications.Linphone.Core;

namespace Linphone.Views
{
    public partial class IncomingCall : Page
    {
        private string _callerNumber;
        private string _canonicalCallerPhoneNumber;
        private string _sipPhoneUsername;
        
        public IncomingCall()
        {
            this.InitializeComponent();

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
                _callerNumber = (nee.Parameter as String);
                Address address = LinphoneManager.Instance.Core.InterpretUrl(_callerNumber);
                _canonicalCallerPhoneNumber = address.GetCanonicalPhoneNumber();

                if (_callerNumber.StartsWith("sip:"))
                {
                    _callerNumber = _callerNumber.Substring(4);
                }

                // While we dunno if the number matches a contact one, we consider it won't and we display the phone number as username
                Contact.Text = _callerNumber;

                if (_callerNumber != null && _callerNumber.Length > 0)
                {
                    //ContactManager cm = ContactManager.Instance;
                    //cm.ContactFound += cm_ContactFound;
                    //cm.FindContact(_callingNumber);
                }
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
                        CoreHttpClient.Instance.AcceptIncomingCall(Dialer.CallId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failure in accepting the incoming call.");
                    }
                }

                List<string> parameters = new List<string>();
                parameters.Add(_callerNumber);
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