using BSN.Resa.Vns.Locale;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public enum AgentStatus
    {
        [Display(Name = "Ready", ResourceType = typeof(Resources))]
        Ready = 1,
        [Display(Name = "Break", ResourceType = typeof(Resources))]
        Break = 2,
        [Display(Name = "Offline", ResourceType = typeof(Resources))]
        Offline = 3,
    }
}
