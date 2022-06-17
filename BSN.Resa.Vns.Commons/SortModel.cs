using System.Collections.Generic;
using System.Linq;

namespace BSN.Resa.Vns.Commons
{
    public class SortModel
    {
        public SortModel()
        {
            Parameters = new List<SortParameter>();
        }

        public void AddParameter(string key)
        {
            Parameters.Add(new SortParameter()
            { 
                Key = key.Replace("-", string.Empty),
                Direction = key.Contains("-") ? SortDirection.Descending : SortDirection.Ascending
            });
        }

        public override string ToString()
        {
            return string.Join(",", Parameters.Select(P => $"{(P.Direction == SortDirection.Descending ? "-" : string.Empty)}{P.Key}"));
        }


        private List<SortParameter> Parameters { get; set; }
    }


    public class SortParameter
    {
        public string Key { get; set; }

        public SortDirection Direction { get; set; }

        public override string ToString()
        {
            string sortOrder = Direction == SortDirection.Ascending ? "ASC" : "DESC";
            return $"{Key}:{sortOrder}";
        }
    }


    public enum SortDirection
    {
        Ascending = 1,
        Descending = 2
    }
}
