using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto.Behrouz
{
    public class GetFamilyInfoResponse
    {
        public PersonInformation HouseHoldHead { get; set; }

        public List<PersonInformation> MembersOfFamily { get; set; }

        public GetFamilyInfoResponse()
        {
            MembersOfFamily = new List<PersonInformation>();
        }
    }
}
