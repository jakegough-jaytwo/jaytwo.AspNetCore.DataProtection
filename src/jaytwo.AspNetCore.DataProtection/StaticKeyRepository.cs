using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace jaytwo.AspNetCore.DataProtection
{
    public class StaticKeyRepository : IXmlRepository
    {
        private readonly IReadOnlyCollection<XElement> _elements;

        public StaticKeyRepository(params string[] seeds)
            : this(CreateKeyXmlFromSeeds(seeds))
        {
        }

        public StaticKeyRepository(IReadOnlyCollection<XElement> elements)
        {
            _elements = elements;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return _elements;
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            throw new NotSupportedException();
        }

        internal static IReadOnlyCollection<XElement> CreateKeyXmlFromSeeds(IList<string> seeds)
        {
            return seeds
                .Select(CreateKeyXmlFromSeed)
                .ToList()
                .AsReadOnly();
        }

        internal static XElement CreateKeyXmlFromSeed(string seed)
        {
            if (string.IsNullOrWhiteSpace(seed))
            {
                throw new ArgumentNullException(nameof(seed));
            }

            var seedBytes = Encoding.UTF8.GetBytes(seed);
            byte[] seedHash;
            using (var hasher = SHA512.Create())
            {
                seedHash = hasher.ComputeHash(seedBytes);
            }

            var seedHashBase64 = Convert.ToBase64String(seedHash);
            var seedGuid = new Guid(seedHash.Take(16).ToArray());

            const string keyXmlFormat =
                @"<?xml version=""1.0"" encoding=""utf-8""?>
                <key id=""{0}"" version=""1"">
                    <creationDate>2018-10-15T00:00:00.0000000Z</creationDate>
                    <activationDate>2018-10-15T00:00:00.0000000Z</activationDate>
                    <expirationDate>2999-12-31T23:59:59.9999999Z</expirationDate>
                    <descriptor deserializerType=""Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorDescriptorDeserializer, Microsoft.AspNetCore.DataProtection, Version=2.1.1.0, Culture=neutral, PublicKeyToken=adb9793829ddae60"">
                    <descriptor>
                        <encryption algorithm=""AES_256_CBC"" />
                        <validation algorithm=""HMACSHA256"" />
                        <masterKey p4:requiresEncryption=""true"" xmlns:p4=""http://schemas.asp.net/2015/03/dataProtection"">
                        <value>{1}</value>
                        </masterKey>
                    </descriptor>
                    </descriptor>
                </key>";

            var keyXml = string.Format(keyXmlFormat, seedGuid, seedHashBase64);

            using (var reader = new StringReader(keyXml))
            {
                var xelement = XElement.Load(reader);
                return xelement;
            }
        }
    }
}
