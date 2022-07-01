using System.Collections.Generic;
using System.Linq;

namespace BSN.Resa.Vns.Commons
{
    public class FilterModel
    {
        public FilterModel()
        {
            Parameters = new List<FilterParameter>();
        }

        public void AddParameter(string key, string value, string @operator = FilterOperator.Contains)
        {
            Parameters.Add(new FilterParameter()
            {
                Key = key,
                Value = value,
                Operator = @operator
            });
        }

        public override string ToString()
        {
            return string.Join(",", Parameters.Select(P => $"{P.Key}{P.Operator}{P.Value}"));
        }

        private List<FilterParameter> Parameters { get; set; }
    }

    public class FilterParameter
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Operator { get; set; }
    }

    /// <summary>
    /// Sieve library defauly operators.
    /// <see cref="https://github.com/Biarity/Sieve"/>
    /// </summary>
    public class FilterOperator
    {
        public const string Equals = "==";

        public const string NotEquals = "!=";

        public const string GreaterThan = ">";

        public const string LessThan = "<";

        public const string GreaterThanOrEqualTo = ">=";

        public const string LessThanOrEqualTo = "<=";

        public const string Contains = "@=";
    }
}
