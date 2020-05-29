// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MartinCostello.AzureFunctions.AppService.Client;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace MartinCostello.AzureFunctions.AppService
{
    /// <summary>
    /// A class for managing TLS certificates for Azure App Service instances. This class cannot be inherited.
    /// </summary>
    public sealed class CertificateService : ICertificateService
    {
        /// <summary>
        /// The password to use for certificates. This field is read-only.
        /// </summary>
        private readonly string _certificatePassword;

        /// <summary>
        /// The App Service client to use. This field is read-only.
        /// </summary>
        private readonly IAppServiceClient _client;

        /// <summary>
        /// The clock to use. This field is read-only.
        /// </summary>
        private readonly IClock _clock;

        /// <summary>
        /// The <see cref="ILogger"/> to use. This field is read-only.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateService"/> class.
        /// </summary>
        /// <param name="certificatePassword">The password used to protect certificate private keys.</param>
        /// <param name="client">The <see cref="IAppServiceClient"/> to use.</param>
        /// <param name="clock">The <see cref="IClock"/> to use.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public CertificateService(
            string certificatePassword,
            IAppServiceClient client,
            IClock clock,
            ILogger logger)
        {
            _certificatePassword = certificatePassword;
            _client = client;
            _clock = clock;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<int> BindAsync(ICloudBlob certificate)
        {
            CertificateData data = await ParseCertificateAsync(certificate);

            if (data == null)
            {
                _logger.LogInformation("No valid certificate found to process bindings for blob {0}.", certificate?.Uri);
                return 0;
            }

            ICollection<IWebApp> applications = await _client.GetApplicationsAsync();

            int bindingsUpdated = 0;

            foreach (var application in applications)
            {
                bindingsUpdated += await UpdateBindingsAsync(application, data);
            }

            _logger.LogInformation(
                "Certificate with thumbprint {0} in blob {1} bound to {2:N0} App Service instance host name(s).",
                data.Thumbprint,
                certificate.Uri,
                bindingsUpdated);

            return bindingsUpdated;
        }

        private async Task<byte[]> GetCertificateAsync(ICloudBlob blob)
        {
            byte[] rawData;

            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                rawData = stream.ToArray();
            }

            return rawData;
        }

        private bool IsCertificateValidNow(X509Certificate2 certificate)
        {
            DateTime utcNow = _clock.GetCurrentInstant().ToDateTimeUtc();

            if (certificate.NotBefore.ToUniversalTime() > utcNow)
            {
                _logger.LogWarning("Cannot bind certificate with thumbprint {0} because it is not valid until {1:u}.", certificate.Thumbprint, certificate.NotBefore);
                return false;
            }

            if (certificate.NotAfter.ToUniversalTime() < utcNow)
            {
                _logger.LogWarning("Cannot bind certificate with thumbprint {0} because it expired at {1:u}.", certificate.Thumbprint, certificate.NotAfter);
                return false;
            }

            return true;
        }

        private async Task<CertificateData> ParseCertificateAsync(ICloudBlob blob)
        {
            if (blob == null)
            {
                _logger.LogWarning("Not processing cloud blob as it is invalid.");
                return null;
            }

            string thumbprint = string.Empty;
            var hostNames = new List<string>();

            byte[] rawData = await GetCertificateAsync(blob);

            if (rawData == null || rawData.Length < 1)
            {
                _logger.LogWarning("Not processing cloud blob as contains no data.");
                return null;
            }

            using (var certificate = new X509Certificate2(rawData, _certificatePassword))
            {
                if (!IsCertificateValidNow(certificate))
                {
                    return null;
                }

                ICollection<string> subjectAlternateNames = X509CertificateHelpers.GetSubjectAlternateNames(certificate);

                hostNames.AddRange(subjectAlternateNames);
                thumbprint = certificate.Thumbprint.ToLowerInvariant();
            }

            string commonName = blob.Metadata["CommonName"];

            if (!hostNames.Contains(commonName))
            {
                hostNames.Add(commonName);
            }

            return new CertificateData()
            {
                HostNames = hostNames,
                Password = _certificatePassword,
                RawData = rawData,
                Thumbprint = thumbprint,
            };
        }

        private async Task<int> UpdateBindingsAsync(IWebApp application, CertificateData certificate)
        {
            IReadOnlyDictionary<string, IHostNameBinding> bindings = await application.GetHostNameBindingsAsync();

            int bindingsUpdated = 0;

            foreach (var binding in bindings)
            {
                string hostName = binding.Key;

                bool isCertificateSupported = certificate.HostNames.Contains(hostName);

                if (!isCertificateSupported)
                {
                    _logger.LogDebug("Certificate with thumbprint {0} is not supported for host name {1}.", certificate.Thumbprint, hostName);
                    continue;
                }

                if (binding.Value.Inner == null ||
                    string.IsNullOrEmpty(binding.Value.Inner.Thumbprint))
                {
                    _logger.LogDebug("No inner binding is available for host name {0}.", hostName);
                    continue;
                }

                bool isCertificateInstalled = string.Equals(binding.Value.Inner.Thumbprint, certificate.Thumbprint, StringComparison.OrdinalIgnoreCase);

                if (isCertificateInstalled)
                {
                    _logger.LogDebug("Certificate with thumbprint {0} is already bound to host name {1}.", certificate.Thumbprint, hostName);
                    continue;
                }

                await _client.UpdateBindingAsync(
                    application,
                    hostName,
                    certificate.Thumbprint,
                    certificate.RawData,
                    certificate.Password);

                _logger.LogInformation(
                    "Bound certificate with thumbprint {0} to host name {1} for App Service {2}.",
                    certificate.Thumbprint,
                    hostName,
                    application.Name);

                bindingsUpdated++;
            }

            _logger.LogInformation(
                "Updated {0:N0} host name binding(s) for App Service {1}.",
                bindingsUpdated,
                application.Name);

            return bindingsUpdated;
        }

        private sealed class CertificateData
        {
            internal ICollection<string> HostNames { get; set; }

            internal string Password { get; set; }

            internal byte[] RawData { get; set; }

            internal string Thumbprint { get; set; }
        }
    }
}
