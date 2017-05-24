using System.Net;

namespace EventStore.Core.Data
{
    public class GossipAdvertiseInfo
    {
        public IPEndPoint InternalTcp { get; set; }
        public IPEndPoint InternalSecureTcp { get; set; }
        public IPEndPoint ExternalTcp { get; set; }
        public IPEndPoint ExternalSecureTcp { get; set; }
        public IPEndPoint InternalHttp { get; set; }
        public IPEndPoint ExternalHttp { get; set; }
        public IPAddress AdvertiseInternalIPAs { get; set; }
        public IPAddress AdvertiseExternalIPAs { get; set; }
        public int AdvertiseInternalHttpPortAs { get; set; }
        public int AdvertiseExternalHttpPortAs { get; set; }
        public GossipAdvertiseInfo(IPEndPoint internalTcp, IPEndPoint internalSecureTcp,
                                   IPEndPoint externalTcp, IPEndPoint externalSecureTcp,
                                   IPEndPoint internalHttp, IPEndPoint externalHttp,
                                   IPAddress advertiseInternalIPAs, IPAddress advertiseExternalIPAs,
                                   int advertiseInternalHttpPortAs, int advertiseExternalHttpPortAs)
        {
            InternalTcp = internalTcp;
            InternalSecureTcp = internalSecureTcp;
            ExternalTcp = externalTcp;
            ExternalSecureTcp = externalSecureTcp;
            InternalHttp = internalHttp;
            ExternalHttp = externalHttp;
            AdvertiseInternalIPAs = advertiseInternalIPAs;
            AdvertiseExternalIPAs = advertiseExternalIPAs;
            AdvertiseInternalHttpPortAs = advertiseInternalHttpPortAs;
            AdvertiseExternalHttpPortAs = advertiseExternalHttpPortAs;
        }
        public override string ToString() =>
            $"IntTcp: {InternalTcp}, IntSecureTcp: {InternalSecureTcp}\nExtTcp: {ExternalTcp}, ExtSecureTcp: {ExternalSecureTcp}\nIntHttp: {InternalHttp}, ExtHttp: {ExternalHttp}, IntAdvertiseAs: {AdvertiseInternalIPAs}:{AdvertiseInternalHttpPortAs}, ExtAdvertiseAs: {AdvertiseExternalIPAs}:{AdvertiseExternalHttpPortAs}";
    }
}
