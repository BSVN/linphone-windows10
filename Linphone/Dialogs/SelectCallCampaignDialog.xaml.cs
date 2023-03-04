using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BelledonneCommunications.Linphone.Dialogs
{
    public sealed partial class SelectCallCampaignDialog : ContentDialog
    {
        private readonly Dictionary<string, string> CampaignTitleAndIdPair;

        public SelectCallCampaignDialog()
        {
            this.InitializeComponent();
        }

        public SelectCallCampaignDialog(Dictionary<string, string> campaignsTitleWithIdPair)
        {
            this.InitializeComponent();

            foreach (var item in campaignsTitleWithIdPair.Keys)
            {
                AgentActiveCallCampaigns.Items.Add(item);
            }

            this.CampaignTitleAndIdPair = campaignsTitleWithIdPair;
        }

        public string SelectedCallCampaign { get; set; } = null;

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (AgentActiveCallCampaigns.SelectedValue != null)
                SelectedCallCampaign = CampaignTitleAndIdPair[AgentActiveCallCampaigns.SelectedValue.ToString()];
        }
    }
}
