using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BelledonneCommunications.Linphone.Dialogs
{
    public sealed partial class SettingsPasswordDialog : ContentDialog
    {
        public string Password { get; set; }

        public SettingsPasswordDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Password = AdminPassword.Password;
        }
    }
}
