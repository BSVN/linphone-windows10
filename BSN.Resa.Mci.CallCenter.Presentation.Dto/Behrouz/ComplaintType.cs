using BSN.Resa.Vns.Locale;
using System.ComponentModel.DataAnnotations;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz
{
    public enum ComplaintType
    {
        [Display(Name = "Extortion", ResourceType = typeof(Resources))]
        Extortion = 1, // گرانفروشی

        [Display(Name = "Hoarding", ResourceType = typeof(Resources))]
        Hoarding = 2, // احتکار

        [Display(Name = "LackOfHygiene", ResourceType = typeof(Resources))]
        LackOfHygiene = 3, // عدم رعایت بهداشت

        [Display(Name = "Underselling", ResourceType = typeof(Resources))]
        Underselling = 4 // کم فروشی
    }
}
