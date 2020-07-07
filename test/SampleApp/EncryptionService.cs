using System;
using Microsoft.AspNetCore.DataProtection;

namespace SampleApp
{
    // taken from https://edi.wang/post/2019/1/15/caveats-in-aspnet-core-data-protection

    public class EncryptionService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string Purpose = "testing";

        public EncryptionService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(Purpose);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(Purpose);
            return protector.Unprotect(cipherText);
        }
    }
}
