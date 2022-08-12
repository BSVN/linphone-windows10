using BSN.Resa.Vns.Locale;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public enum CallReason
    {
        [Display(Name = "Callback", ResourceType = typeof(Resources))]
        Callback = 1,
        [Display(Name = "Question", ResourceType = typeof(Resources))]
        Question = 2,
        [Display(Name = "Objection", ResourceType = typeof(Resources))]
        Objection = 3,
        [Display(Name = "TicketFollowUp", ResourceType = typeof(Resources))]
        TicketFollowUp = 4,
        [Display(Name = "Other", ResourceType = typeof(Resources))]
        Other = 5,
        [Display(Name = "Silence", ResourceType = typeof(Resources))]
        Silence = 6
    }
}
