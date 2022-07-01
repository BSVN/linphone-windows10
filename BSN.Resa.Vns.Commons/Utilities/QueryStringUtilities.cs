using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSN.Resa.Vns.Commons.Utilities
{
    public class QueryStringUtilities
    {
        public static string Append(string uri, IDictionary<string, string> queryString)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (queryString == null)
                throw new ArgumentNullException(nameof(queryString));

            queryString = queryString.Where(P => !string.IsNullOrWhiteSpace(P.Value))
                                     .ToDictionary(P => P.Key, P => P.Value);

            if (queryString.Count == 0)
                return uri;

            return QueryHelpers.AddQueryString(uri, queryString);
        }

        public static string Append(string uri, string name, string value)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Append(uri, new Dictionary<string, string> { { name, value } });
        }
    }
}
