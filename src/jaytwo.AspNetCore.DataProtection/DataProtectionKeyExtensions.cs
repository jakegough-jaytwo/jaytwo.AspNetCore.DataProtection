using System;
using jaytwo.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection;

namespace jaytwo.AspNetCore.DataProtection
{
    public static class DataProtectionKeyExtensions
    {
        public static IDataProtectionBuilder UseStaticKeyFromSeed(this IDataProtectionBuilder builder, string seed)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var xmlRepository = new StaticKeyRepository(seed);
            return builder.AddKeyManagementOptions(options => options.XmlRepository = xmlRepository);
        }
    }
}
