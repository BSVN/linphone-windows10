using Linphone;
using System;

namespace BelledonneCommunications.Linphone.Commons
{
    public static class AddressExtentions
    {
        public static string GetCanonicalPhoneNumber(this Address address)
        {
            string phoneNumber = address?.Username ?? throw new ArgumentNullException(nameof(address));
            return "0" + phoneNumber.Substring(phoneNumber.Length - 10);
        }

        public static string GetCanonicalPhoneNumber(this string phoneNumber)
        {
            return "0" + phoneNumber.Substring(phoneNumber.Length - 10);
        }

        public static string GetUsernameFromAddress(this Address Address)
        {
            if (Address.DisplayName != null && Address.DisplayName.Length > 0)
            {
                return Address.DisplayName;
            }
            else
            {
                if (Address.Username != null && Address.Username.Length > 0)
                {
                    return Address.Username;
                }
                else
                {
                    return Address.AsStringUriOnly();
                }
            }
        }
    }
}
