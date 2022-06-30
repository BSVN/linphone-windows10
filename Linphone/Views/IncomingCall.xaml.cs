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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Core;
using System.Collections.Generic;
using BelledonneCommunications.Linphone.Core;
using BelledonneCommunications.Linphone;
using BelledonneCommunications.Linphone.Dialogs;
using BelledonneCommunications.Linphone.Commons;

namespace Linphone.Views
{
    public partial class IncomingCall : Page
    {
        private const string PHONE_ADDRESS_PREFIX = "sip:";
        private string _callerNumber;

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

            if (!await Utility.IsMicrophoneAvailable())
            {
                var micrphonePermissionDialog = new MicrophonePermissionRequestDialog();
                await micrphonePermissionDialog.ShowAsync();
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"));
            }

            if ((nee.Parameter as string).Contains(PHONE_ADDRESS_PREFIX))
            {
                _callerNumber = (nee.Parameter as string);
                if (_callerNumber.StartsWith(PHONE_ADDRESS_PREFIX))
                {
                    _callerNumber = _callerNumber.Substring(PHONE_ADDRESS_PREFIX.Length);
                }
                
                // While we dunno if the number matches a contact one, we consider it won't and we display the phone number as username
                Address address = LinphoneManager.Instance.Core.InterpretUrl(_callerNumber);
                Contact.Text = address.GetCanonicalPhoneNumber();

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
                // HotPoint #1
                CallFlowControl.Instance.CallEstablished();

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
            CallFlowControl.Instance.IncomingCallDeclined();
            LinphoneManager.Instance.EndCurrentCall();
            
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}