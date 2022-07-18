using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BSN.Resa.Mci.CallCenter.AgentApp.Data.DataModels
{
    internal class CallbackDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(reader.GetString()))
                return default;

            return DateTime.ParseExact(reader.GetString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime dateTimeValue, JsonSerializerOptions options)
        {
            writer.WriteStringValue(dateTimeValue.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
        }
    }
}   
