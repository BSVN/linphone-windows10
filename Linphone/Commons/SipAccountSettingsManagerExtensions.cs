using BelledonneCommunications.Linphone.Presentation.Dto;
using Linphone.Model;

namespace BelledonneCommunications.Linphone.Commons
{
    public static class SIPAccountSettingsManagerExtensions
    {
        public static void Update(this SIPAccountSettingsManager settings, SipProfileViewModel sipProfileViewModel)
        {
            settings.Load();

            settings.Username = string.IsNullOrWhiteSpace(sipProfileViewModel.Username) ? "" : sipProfileViewModel.Username;
            settings.UserId = string.IsNullOrWhiteSpace(sipProfileViewModel.UserId) ? "" : sipProfileViewModel.UserId;
            settings.Password = string.IsNullOrWhiteSpace(sipProfileViewModel.Password) ? "" : sipProfileViewModel.Password;
            settings.Domain = string.IsNullOrWhiteSpace(sipProfileViewModel.Domain) ? "10.19.82.3" : sipProfileViewModel.Domain;
            settings.Proxy = string.IsNullOrWhiteSpace(settings.Proxy) ? "" : settings.Proxy;
            settings.OutboundProxy = settings.OutboundProxy;
            settings.DisplayName = string.IsNullOrWhiteSpace(sipProfileViewModel.Username) ? "" : sipProfileViewModel.Username;
            settings.Transports = (sipProfileViewModel.Protocol == 0) ? "TCP" : sipProfileViewModel.Protocol.ToString("g");
            settings.Expires = string.IsNullOrWhiteSpace(settings.Expires) ? "500" : settings.Expires;
            settings.AVPF = settings.AVPF;
            settings.ICE = settings.ICE;

            settings.Save();
        }

        public static void Update(this SIPAccountSettingsManager settings, string username, string userId, string password, string domain, string proxy, bool? outboundProxy, string displayName, string transports, string expires, bool? aVPF, bool? iCE)
        {
            settings.Load();

            settings.Username = username;
            settings.UserId = userId;
            settings.Password = password;
            settings.Domain = domain;
            settings.Proxy = proxy;
            settings.OutboundProxy = outboundProxy;
            settings.DisplayName = displayName;
            settings.Transports = transports;
            settings.Expires = expires;
            settings.AVPF = aVPF;
            settings.ICE = iCE;

            settings.Save();
        }
    }
}
