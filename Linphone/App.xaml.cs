/*
App.xaml.cs
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
using Linphone.Model;
using Microsoft.Win32;
using PCLAppConfig;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BSN.Resa.Mci.CallCenter.AgentApp.Data;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using BelledonneCommunications.Linphone.ViewModels;
using BSN.Commons.Infrastructure;
using BSN.Resa.Mci.CallCenter.Presentation.Dto;

namespace Linphone
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application, CallControllerListener
    {
        Frame rootFrame;
        bool acceptCall;
        String sipAddress;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
        {
            this.InitializeComponent();
            this.UnhandledException += App_UnhandledException;
            this.Suspending += OnSuspending;

            ConfigurationManager.Initialise(PCLAppConfig.FileSystemStream.PortableStream.Current);

            SettingsManager.InstallConfigFile();
            applicationSettingsManager = new ApplicationSettingsManager();
            applicationSettingsManager.Load();

			Log.Logger.Error("Here is the usage.");

            Logger.ConfigureLogger();

            _logger = Log.Logger.ForContext("SourceContext", nameof(App));

            databaseFactory = new DatabaseFactory(applicationSettingsManager.RedisConnectionString);
            databaseFactory.OnConnectionEstablished += P => Ioc.Default.GetRequiredService<CallFlowControl>().AgentProfile.CallbackQueueConnectionEstablished = true;

            // TODO: Use this code check current version of WebView.
            // This line of code might be counted as deprecated as soon as we use fixed runtime instaed.
            IsWebView2Installed();

            acceptCall = false;
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _logger.Error(e.Exception, "Application unhandled exception.");

            e.Handled = true;
        }

        private void Back_requested(object sender, BackRequestedEventArgs e)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }

        public async void CallEnded(Call call)
        {
            bool wasAnOutgoingCall = _callFlowControl.CallContext.Direction == CallDirection.Outgoing;
            if (_callFlowControl.CallContext.Direction == CallDirection.Command)
            {
                _logger.Information("A command call has been ended.");

                if (_closeApp)
                {
                    DisableRegisteration();
                    Current.Exit();
                }

                _callFlowControl.CallContext.Direction = CallDirection.Incoming;
                return;
            }
            else
            {
                await _callFlowControl.TerminateCall();
            }

            if (wasAnOutgoingCall)
            {
                if (_callFlowControl.AgentProfile.Status == BSN.Resa.Mci.CallCenter.Presentation.Dto.AgentStatus.Ready)
                    _callFlowControl.JoinIntoIncomingCallQueue();
            }

            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
            else
            {
                // Launch the Dialer and remove the incall view from the backstack
                rootFrame.Navigate(typeof(Views.Dialer), null);
                if (rootFrame.BackStack.Count > 0)
                {
                    rootFrame.BackStack.Clear();
                }
            }
        }

        public void CallUpdatedByRemote(Call call, bool isVideoAdded)
        {
            //throw new NotImplementedException();
        }

        public void MuteStateChanged(bool isMicMuted)
        {
            //throw new NotImplementedException();
        }

        public void NewCallStarted(string callerNumber)
        {
            if (_callFlowControl.CallContext.Direction == CallDirection.Command)
            {
                return;
            }
            else if (_callFlowControl.CallContext.Direction == CallDirection.Outgoing)
            {
                _callFlowControl.CallEstablished();
            }

            Debug.WriteLine("[CallListener] NewCallStarted " + callerNumber);
            List<String> parameters = new List<String>();
            parameters.Add(callerNumber);
            rootFrame.Navigate(typeof(Views.InCall), parameters);
        }

        public void PauseStateChanged(Call call, bool isCallPaused, bool isCallPausedByRemote)
        {
            Debug.WriteLine("Pausestatechanged");
            // if (this.PauseListener != null)
            //     this.PauseListener.PauseStateChanged(call, isCallPaused, isCallPausedByRemote);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Initialize(e, null);
        }

		private void Initialize(IActivatedEventArgs e, String args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            //Start linphone
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            LinphoneManager.Instance.InitLinphoneCore();
            LinphoneManager.Instance.CallListener = this;
            LinphoneManager.Instance.CoreDispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().CoreWindow.Dispatcher;

            rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += Back_requested;

            RegisterTypes(rootFrame);

            if (rootFrame.Content == null)
            {
                if (args != null)
                {
                    if (args.StartsWith("chat"))
                    {
                        var sipAddr = args.Split('=')[1];
                        // TODO: Please replace with Prism equiavalent
                        rootFrame.Navigate(typeof(Views.Chat), sipAddr);
                    }
                    else
                    {
                        if (args.StartsWith("answer"))
                        {
                            acceptCall = true;
                            sipAddress = args.Split('=')[1];
                        }
                        rootFrame.Navigate(typeof(Views.Dialer), null);
                    }
                }
                else
                {
                    rootFrame.Navigate(typeof(Views.Dialer), args);
                }
            }

            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App_CloseRequested;
            //Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(WindowVisibilityChanged);
            Window.Current.Activate();

            DisableRegisteration();
        }

		private void RegisterTypes(Frame rootFrame)
		{
            var serviceCollection = new ServiceCollection()
                .AddSingleton<INavigationService>(new NavigationService(rootFrame))
                .AddSingleton<AgentProfile>()
                .AddSingleton<CallContext>()
                .AddSingleton<CallFlowControl>()
				.AddSingleton<IDatabaseFactory>(databaseFactory)
				.AddTransient<DialerViewModel>();

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["InHomeTesting"]))
			{
				serviceCollection
					.AddSingleton<ICallbackQueue>(new CallbackQueueStub());
			}
			else
			{
                serviceCollection
                    .AddSingleton<ICallbackQueue>(new CallbackQueue(databaseFactory));
			}

            serviceCollection.AddHttpClient<ICallEventsReportHttpClient, CallEventsReportHttpClient >(P => P.BaseAddress = new Uri(applicationSettingsManager.PanelAddress));
            serviceCollection.AddHttpClient<ICallEventsReportHttpClient, CallEventsReportHttpClient >(P => P.BaseAddress = new Uri(applicationSettingsManager.PanelAddress));

            Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());

            _callFlowControl = Ioc.Default.GetRequiredService<CallFlowControl>();
		}

		protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.ToastNotification)
            {
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                var arguments = toastArgs.Argument;
                Initialize(args, arguments);
            }
        }

        private async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            if (_closeApp) return;

            _logger.Information("App_CloseRequested raised omg !!");

            var differal = e.GetDeferral();

            e.Handled = true;

            _closeApp = true;

            if (_callFlowControl.AgentProfile.IsLoggedIn)
                await _callFlowControl.UpdateAgentStatusAsync(AgentStatus.Offline);

            if (_callFlowControl.CallContext.Direction != CallDirection.Command)
            {
                DisableRegisteration();

                differal.Complete();

                Current.Exit();
            }
            else
            {
                differal.Complete();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
		{
            var deferral = e.SuspendingOperation.GetDeferral();

            //TODO: Save application state and stop any background activity
            DisableRegisteration();

            _logger.Debug("OnSuspending.");
            deferral.Complete();
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

                //Wait for unregister to complete
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

        public async void CallIncoming(Call call)
        {
            if (acceptCall)
            {
                // Related arguments should be set for routing incoming call to this section. 
                if (sipAddress != "")
                {
                    Address addr = LinphoneManager.Instance.Core.InterpretUrl(sipAddress);
                    if (addr != null && addr.AsStringUriOnly().Equals(call.RemoteAddress.AsStringUriOnly()))
                    {
                        call.Accept();
                        List<string> parameters = new List<string>();
                        parameters.Add(call.RemoteAddress.AsString());
                        rootFrame.Navigate(typeof(Views.InCall), parameters);
                        acceptCall = false;
                    }
                }
            }
            else
            {
                // HotPoint #0
                Address address = LinphoneManager.Instance.Core.InterpretUrl(call.RemoteAddress.AsString());
                await _callFlowControl.InitiateIncomingCallAsync(address.GetCanonicalPhoneNumber(), call.RemoteAddress.DisplayName);

                rootFrame.Navigate(typeof(Views.IncomingCall), call.RemoteAddress.AsString());
            }
        }

        private bool IsWebView2Installed()
        {
            // Todo: Working with registry is forbidden for some devices, we should acquire grants needed for this task.
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("pv");
                        if (value != null)
                        {
                            _logger.Information($"WebView2 Version: {value}");
                            return true;
                        }
                    }
                }

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("pv");
                        if (value != null)
                        {
                            _logger.Information($"WebView2 Version: {value}");
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Internal error while checking webview2 installation.");
                return false;
            }
        }

        private bool _closeApp = false;
		
        private readonly ApplicationSettingsManager applicationSettingsManager;
        private CallFlowControl _callFlowControl;

        private readonly ILogger _logger;
        private readonly DatabaseFactory databaseFactory;
	}
}
