using System.Linq;
using System.Collections.Generic;

namespace BSN.Resa.Vns.Commons.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool HasDuplicates<TSource>(this IEnumerable<TSource> source)
        {
            return source.Count() != source.Distinct().Count();
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }
    }
}
