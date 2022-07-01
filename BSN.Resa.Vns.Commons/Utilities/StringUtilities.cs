using System;
using System.Collections.Generic;
using System.Linq;

namespace BSN.Resa.Vns.Commons.Utilities
{
    public static class StringUtilities
    {
        public static string GenerateRandomAlphaNumericString(uint length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random random = new Random(DateTime.Now.GetHashCode());
            var result = new string(Enumerable.Repeat(chars, (int)length).Select(s => s[random.Next(s.Length)]).ToArray());

            return result;
        }

        public static string GenerateRandomNumericString(uint length)
        {
            const string chars = "0123456789";

            Random random = new Random(DateTime.Now.GetHashCode());
            var result = new string(Enumerable.Repeat(chars, (int)length).Select(s => s[random.Next(s.Length)]).ToArray());

            return result;
        }

        public static string ConvertPersianNumbersToEnglish(string value)
        {
            var conversionMap = new Dictionary<char, char>
            {
                ['۰'] = '0',
                ['۱'] = '1',
                ['۲'] = '2',
                ['۳'] = '3',
                ['۴'] = '4',
                ['۵'] = '5',
                ['۶'] = '6',
                ['۷'] = '7',
                ['۸'] = '8',
                ['۹'] = '9'
            };

            foreach (KeyValuePair<char, char> conversion in conversionMap)
                value = value.Replace(conversion.Key, conversion.Value);

            return value;
        }
    }
}
