using BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz;
using BSN.Resa.Mci.CallCenter.Presentation.Dto.Rubika;
using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class HeadOfHouseholdViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NationalCode { get; set; }

        public string BirthDate { get; set; }

        public bool IsHeadOfHousehold { get; set; }

        public string MobilePhoneNumber { get; set; }

        public List<PersonInformation> MembersOfFamily { get; set; }

        public IEnumerable<SubsidyItem> SubsidyItems { get; set; }

        public IEnumerable<SubsidyItem> SubsidyRemain { get; set; }

        public IEnumerable<Order> LastOrders { get; set; }
    }
}