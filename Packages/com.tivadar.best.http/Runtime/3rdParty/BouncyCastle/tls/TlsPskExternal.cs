#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

namespace Best.HTTP.SecureProtocol.Org.BouncyCastle.Tls
{
    public interface TlsPskExternal
        : TlsPsk
    {
    }
}
#pragma warning restore
#endif
