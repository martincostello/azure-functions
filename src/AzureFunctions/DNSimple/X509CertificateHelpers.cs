// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MartinCostello.AzureFunctions.DNSimple
{
    /// <summary>
    /// A class containing helper methods for working with X.509 certificates.
    /// </summary>
    internal static class X509CertificateHelpers
    {
        /// <summary>
        /// Creates a <see cref="X509Certificate2"/> for the specified public PEM-encoded key.
        /// </summary>
        /// <param name="publicKeyString">The PEM-encoded public key.</param>
        /// <returns>
        /// A <see cref="X509Certificate2"/> containing the public key.
        /// </returns>
        internal static X509Certificate2 CreateCertificate(string publicKeyString)
        {
            byte[] publicKeyBytes = Encoding.UTF8.GetBytes(publicKeyString);
            return new X509Certificate2(publicKeyBytes);
        }

        /// <summary>
        /// Creates a <see cref="X509Certificate2"/> by combining the specified public and private PEM-encoded keys.
        /// </summary>
        /// <param name="publicKeyString">The PEM-encoded public key.</param>
        /// <param name="privateKeyString">The PEM-encoded private key.</param>
        /// <returns>
        /// A <see cref="X509Certificate2"/> containing the combined certificate keys.
        /// </returns>
        internal static X509Certificate2 Combine(string publicKeyString, string privateKeyString)
        {
            byte[] privateKeyBytes = DecodePem(privateKeyString);

            try
            {
                RSAParameters parameters = GetRSAParameters(privateKeyBytes);

                try
                {
                    using (var privateKey = RSA.Create(parameters))
                    {
                        using (X509Certificate2 publicKey = CreateCertificate(publicKeyString))
                        {
                            return publicKey.CopyWithPrivateKey(privateKey);
                        }
                    }
                }
                finally
                {
                    Clear(parameters.D);
                    Clear(parameters.DP);
                    Clear(parameters.DQ);
                    Clear(parameters.Exponent);
                    Clear(parameters.InverseQ);
                    Clear(parameters.Modulus);
                    Clear(parameters.P);
                    Clear(parameters.Q);
                }
            }
            finally
            {
                Clear(privateKeyBytes);
            }
        }

        /// <summary>
        /// Returns an <see cref="IDictionary{TKey, TValue}"/> containing metadata for the specified certificate.
        /// </summary>
        /// <param name="certificate">The certificate to get the metadata for.</param>
        /// <returns>
        /// An <see cref="IDictionary{TKey, TValue}"/> containing metadata associated with <paramref name="certificate"/>.
        /// </returns>
        internal static IDictionary<string, string> GetMetadata(X509Certificate2 certificate)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "FriendlyName", certificate.FriendlyName },
                { "Issuer", certificate.Issuer },
                { "IssuerName", certificate.IssuerName.Name },
                { "NotAfter", certificate.NotAfter.ToString("u", CultureInfo.InstalledUICulture) },
                { "NotBefore", certificate.NotBefore.ToString("u", CultureInfo.InstalledUICulture) },
                { "SerialNumber", certificate.SerialNumber },
                { "SignatureAlgorithm", certificate.SignatureAlgorithm.FriendlyName },
                { "Subject", certificate.Subject },
                { "SubjectName", certificate.SubjectName.Name },
                { "Thumbprint", certificate.Thumbprint },
                { "Version", certificate.Version.ToString(CultureInfo.InstalledUICulture) },
            };
        }

        private static RSAParameters GetRSAParameters(byte[] privateKeyBytes)
        {
            RSAParameters parameters = new RSAParameters();

            using (var stream = new MemoryStream(privateKeyBytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    ushort format = reader.ReadUInt16();

                    if (format == 0x8130)
                    {
                        reader.ReadByte();
                    }
                    else if (format == 0x8230)
                    {
                        reader.ReadInt16();
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid private key format.");
                    }

                    if (reader.ReadUInt16() != 0x0102)
                    {
                        throw new InvalidOperationException("Invalid private key version.");
                    }

                    if (reader.ReadByte() != 0x00)
                    {
                        throw new InvalidOperationException("Invalid private key padding.");
                    }

                    byte[] ReadParameter()
                    {
                        int length = GetLength(reader);
                        return reader.ReadBytes(length);
                    }

                    parameters.Modulus = ReadParameter();
                    parameters.Exponent = ReadParameter();
                    parameters.D = ReadParameter();
                    parameters.P = ReadParameter();
                    parameters.Q = ReadParameter();
                    parameters.DP = ReadParameter();
                    parameters.DQ = ReadParameter();
                    parameters.InverseQ = ReadParameter();

                    return parameters;
                }
            }
        }

        private static byte[] DecodePem(string privateKey)
        {
            const string Header = "-----BEGIN RSA PRIVATE KEY-----\n";
            const string Footer = "-----END RSA PRIVATE KEY-----";

            int startIndex = privateKey.IndexOf(Header) + Header.Length;
            int endIndex = privateKey.IndexOf(Footer, startIndex);

            string base64 = privateKey.Substring(startIndex, endIndex - startIndex);

            return Convert.FromBase64String(base64);
        }

        private static int GetLength(BinaryReader reader)
        {
            if (reader.ReadByte() != 0x02)
            {
                return 0;
            }

            int length;
            byte value = reader.ReadByte();

            if (value == 0x81)
            {
                length = reader.ReadByte();
            }
            else if (value == 0x82)
            {
                byte highByte = reader.ReadByte();
                byte lowByte = reader.ReadByte();

                byte[] parts = { lowByte, highByte, 0, 0 };
                length = BitConverter.ToInt32(parts, 0);
            }
            else
            {
                length = value;
            }

            while (reader.ReadByte() == 0)
            {
                length -= 1;
            }

            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            return length;
        }

        private static void Clear(Array array) => Array.Clear(array, 0, array.Length);
    }
}
