using System;
using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    //TODO [Noei]: It seems file names are not matched with class names.
    public class OperatorsCommandServiceCreateAgentRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid WorkgroupId { get; set; }

        public IEnumerable<string> AccessGrants { get; set; }

        public SipProfileViewModel SipProfile { get; set; }
    }
}
