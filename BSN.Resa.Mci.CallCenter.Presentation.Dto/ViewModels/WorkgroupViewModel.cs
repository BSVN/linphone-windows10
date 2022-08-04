using System;
using System.Collections.Generic;

namespace BSN.Resa.Mci.CallCenter.Presentation.Dto
{
    public class WorkgroupViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<OperatorViewModel> Users { get; set; }
    }
}
