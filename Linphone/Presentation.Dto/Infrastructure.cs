/*
GenericResponseBase.cs
Copyright (C) 2022 Resaa Corporation.
Copyright (C) 2016 Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BelledonneCommunications.Linphone.Presentation.Dto
{
    public class GenericResponseBase<T> : Response where T : class
    {
        public T Data { get; set; }

        public GenericResponseBase() { }
    }

    public interface IResponse<ValidationResultType>
    {
        IList<ValidationResultType> InvalidItems { get; set; }

        bool IsSuccess { get; }

        string Message { get; set; }

        ResponseStatusCode StatusCode { get; set; }
    }

    public enum ResponseStatusCode
    {
        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        Ambiguous = 300,
        MultipleChoices = 300,
        Found = 302,
        NotModified = 304,
        Unused = 306,
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        RequestUriTooLong = 414,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504
    }

    public class JsonForceDefaultConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value);
        }
    }

    /// <summary>
    /// During serialization and deserialization operations, enumeration values are always converted to and from strings.
    /// This is possible through registering the 'JsonStringEnumConverter' in our DI infrastructure. There is once exception,
    /// namely the 'StatusCode' property of the 'ResponseBase' class which should keep it's default numeral value when being converted.
    /// </summary>
    public class Response : ResponseBase
    {
        public new bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode <= 299;

        [JsonConverter(typeof(JsonForceDefaultConverter<ResponseStatusCode>))]
        public new ResponseStatusCode StatusCode { get; set; }
    }

    public class ResponseBase : IResponse<ValidationResult>
    {
        public bool IsSuccess
        {
            get
            {
                if (StatusCode >= ResponseStatusCode.OK)
                {
                    return StatusCode <= (ResponseStatusCode)299;
                }

                return false;
            }
        }

        public string Message { get; set; }

        public ResponseStatusCode StatusCode { get; set; }

        public IList<ValidationResult> InvalidItems { get; set; }
    }
}
