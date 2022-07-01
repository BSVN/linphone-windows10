namespace BSN.Resa.Vns.Commons.Requests
{
    public class AdvancedSearchRequest
    {
        public string Filters { get; set; }

        public string Sorts { get; set; }

        public uint PageNumber { get; set; }

        public uint PageSize { get; set; }
    }
}
