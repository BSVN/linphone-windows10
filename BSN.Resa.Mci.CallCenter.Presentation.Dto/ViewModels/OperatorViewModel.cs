using System;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class OperatorViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public UserRole Role { get; set; }

        public AgentStatus? Status { get; set; }

        public string UserId { get; set; }

        public double AverageCallDuration { get; set; }

        public SipProfileViewModel SipProfile { get; set; }

        public WorkgroupViewModel Workgroup { get; set; }
    }
}
