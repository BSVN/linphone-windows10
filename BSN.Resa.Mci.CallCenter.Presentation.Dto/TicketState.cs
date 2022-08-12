using BSN.Resa.Vns.Locale;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public enum TicketState
    {
        [Display(Name = "Created", ResourceType = typeof(Resources))]
        Created = 1,
        [Display(Name = "Processing", ResourceType = typeof(Resources))]
        Processing = 2,
        [Display(Name = "Closed", ResourceType = typeof(Resources))]
        Closed = 3
    }
}
