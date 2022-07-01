using System;

namespace BSN.Resa.Vns.Commons.Utilities
{
    public static class SieveUtilities
    {
        public static (DateTime start, DateTime end) ParseDateTimeRange(string value)
        {
            string[] datetimes = value.Split(RANGE_SEPARATOR);

            if (datetimes.Length != 2)
                throw new ArgumentException(PARAMETER_FORMAT_ERROR, nameof(value));

            if (DateTime.TryParse(datetimes[0], out DateTime start) && DateTime.TryParse(datetimes[1], out DateTime end))
            {
                if (end <= start)
                    throw new ArgumentException(PARAMETER_PRECEDENCE_ERROR);

                return (start, end);
            }

            throw new ArgumentException("Lower or upper bounds are not valid DateTime values.");
        }

        public static (DateTime? start, DateTime? end) ParseDateTimeOpenRange(string value)
        {
            string[] datetimes = value.Split(RANGE_SEPARATOR);
            DateTime? start = null, end = null;

            if (datetimes.Length != 2)
                throw new ArgumentException(PARAMETER_FORMAT_ERROR, nameof(value));

            if (!string.IsNullOrEmpty(datetimes[0]))
            {
                if (DateTime.TryParse(datetimes[0], out DateTime dateTime))
                    start = dateTime;
                else
                    throw new ArgumentException("Lower or upper bounds are not valid DateTime values.");
            }

            if (!string.IsNullOrEmpty(datetimes[1]))
            {
                if (DateTime.TryParse(datetimes[1], out DateTime dateTime))
                    end = dateTime;
                else
                    throw new ArgumentException("Lower or upper bounds are not valid DateTime values.");
            }

            if (start.HasValue && end.HasValue && end <= start)
                throw new ArgumentException(PARAMETER_PRECEDENCE_ERROR);

            return (start, end);
        }

        public static (uint start, uint end) ParseUnsignedIntegerRange(string value)
        {
            string[] integers = value.Split(RANGE_SEPARATOR);

            if (integers.Length != 2)
                throw new ArgumentException(PARAMETER_FORMAT_ERROR, nameof(value));

            if (uint.TryParse(integers[0], out uint start) && uint.TryParse(integers[1], out uint end))
            {
                if (end <= start)
                    throw new ArgumentException(PARAMETER_PRECEDENCE_ERROR);

                return (start, end);
            }

            throw new ArgumentException("Lower or upper bounds are not valid uint values.");
        }

        public static (uint? start, uint? end) ParseUnsignedIntegerOpenRange(string value)
        {
            string[] integers = value.Split(RANGE_SEPARATOR);
            uint? start = null, end = null;

            if (integers.Length != 2)
                throw new ArgumentException(PARAMETER_FORMAT_ERROR, nameof(value));

            if (!string.IsNullOrEmpty(integers[0]))
            {
                if (uint.TryParse(integers[0], out uint integer))
                    start = integer;
                else
                    throw new ArgumentException("Lower or upper bounds are not valid uint values.");
            }

            if (!string.IsNullOrEmpty(integers[1]))
            {
                if (uint.TryParse(integers[1], out uint integer))
                    end = integer;
                else
                    throw new ArgumentException("Lower or upper bounds are not valid uint values.");
            }

            if (start.HasValue && end.HasValue && end <= start)
                throw new ArgumentException(PARAMETER_PRECEDENCE_ERROR);

            return (start, end);
        }

        public static (TimeSpan start, TimeSpan end) ParseTimeSpanRange(string value)
        {
            string[] integers = value.Split(RANGE_SEPARATOR);

            if (integers.Length != 2)
                throw new ArgumentException(PARAMETER_FORMAT_ERROR, nameof(value));

            if (TimeSpan.TryParse(integers[0], out TimeSpan start) && TimeSpan.TryParse(integers[1], out TimeSpan end))
            {
                if (end <= start)
                    throw new ArgumentException(PARAMETER_PRECEDENCE_ERROR);

                return (start, end);
            }

            throw new ArgumentException("Lower or upper bounds are not valid TimeSpan values.");
        }

        public static (TimeSpan? start, TimeSpan? end) ParseTimeSpanOpenRange(string value)
        {
            string[] timeSpans = value.Split(RANGE_SEPARATOR);
            TimeSpan? start = null, end = null;

            if (timeSpans.Length != 2)
                throw new ArgumentException(PARAMETER_FORMAT_ERROR, nameof(value));

            if (!string.IsNullOrEmpty(timeSpans[0]))
            {
                if (TimeSpan.TryParse(timeSpans[0], out TimeSpan timeSpan))
                    start = timeSpan;
                else
                    throw new ArgumentException("Lower or upper bounds are not valid TimeSpan values.");
            }

            if (!string.IsNullOrEmpty(timeSpans[1]))
            {
                if (TimeSpan.TryParse(timeSpans[1], out TimeSpan timeSpan))
                    end = timeSpan;
                else
                    throw new ArgumentException("Lower or upper bounds are not valid TimeSpan values.");
            }

            if (start.HasValue && end.HasValue && end <= start)
                throw new ArgumentException(PARAMETER_PRECEDENCE_ERROR);

            return (start, end);
        }


        private static char[] RANGE_SEPARATOR = { '.', '.', '.' };

        private const string PARAMETER_FORMAT_ERROR = "The parameter is not well-formed and could not be matched to a range pattern.";
        private const string PARAMETER_PRECEDENCE_ERROR = "Upper bound should be greater than the lower bound.";

    }
}
