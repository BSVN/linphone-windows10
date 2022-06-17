using Sieve.Exceptions;
using System;

namespace BSN.Resa.Vns.Commons.Extensions
{
    public static class SieveExtensions
    {
        public static string ExtractMessage(this SieveException ex)
        {
            string message = ex.InnerException?.InnerException?.Message;

            message = message ?? ex.InnerException?.Message;
            message = message ?? ex.Message;

            return message;
        }
    }
}
