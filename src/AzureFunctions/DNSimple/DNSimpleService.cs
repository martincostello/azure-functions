// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MartinCostello.AzureFunctions.Blob;
using MartinCostello.AzureFunctions.DNSimple.Client;
using MartinCostello.AzureFunctions.DNSimple.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple
{
    /// <summary>
    /// A class for handling DNSimple webhooks.
    /// </summary>
    public class DNSimpleService
    {
        /// <summary>
        /// The password to use for certificates. This field is read-only.
        /// </summary>
        private readonly string _certificatePassword;

        /// <summary>
        /// The <see cref="IDNSimpleApiFactory"/> to use. This field is read-only.
        /// </summary>
        private readonly IDNSimpleApiFactory _apiFactory;

        /// <summary>
        /// The <see cref="IBlobClient"/> to use. This field is read-only.
        /// </summary>
        private readonly IBlobClient _blobClient;

        /// <summary>
        /// The <see cref="ILogger"/> to use. This field is read-only.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DNSimpleService"/> class.
        /// </summary>
        /// <param name="certificatePassword">The password to use to protect certificate private keys.</param>
        /// <param name="apiFactory">The <see cref="IDNSimpleApiFactory"/> to use.</param>
        /// <param name="blobClient">The <see cref="IBlobClient"/> to use.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public DNSimpleService(
            string certificatePassword,
            IDNSimpleApiFactory apiFactory,
            IBlobClient blobClient,
            ILogger logger)
        {
            _certificatePassword = certificatePassword;
            _apiFactory = apiFactory;
            _blobClient = blobClient;
            _logger = logger;
        }

        /// <summary>
        /// Handles an HTTP request for a DNSimple webhook as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request to process.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to process the webhook.
        /// </returns>
        public async Task<WebhookResult> ProcessAsync(HttpRequest request)
        {
            var result = new WebhookResult()
            {
                Processed = false,
                StatusCode = StatusCodes.Status200OK,
            };

            try
            {
                _logger.LogInformation("Received HTTP request to DNSimpleWebhook.");

                WebhookPayload payload = await DeserializePayloadAsync(request);

                if (payload == null)
                {
                    result.Message = "Bad request.";
                    result.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    _logger.LogInformation(
                        "Received DNSimple webhook event {Name} with request Id {RequestId}.",
                        payload.Name,
                        payload.RequestId);

                    result.RequestId = payload.RequestId;
                    result.Message = $"Webhook '{payload.RequestId}' acknowledged.";

                    if (CanHandlePayload(payload))
                    {
                        result.Processed = await ProcessCertificateAsync(payload);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle DNSimple webhook: {Message}", ex.Message);

                result.Message = "Internal server error.";
                result.StatusCode = StatusCodes.Status500InternalServerError;

#if DEBUG
                result.Details = ex;
#endif
            }

            return result;
        }

        /// <summary>
        /// Returns whether the specified webhook payload can be handled.
        /// </summary>
        /// <param name="payload">The webhook payload.</param>
        /// <returns>
        /// <see langword="true"/> if the payload can be handled; otherwise <see langword="false"/>.
        /// </returns>
        private bool CanHandlePayload(WebhookPayload payload)
        {
            if (!string.Equals(payload.ApiVersion, "v2", StringComparison.Ordinal))
            {
                _logger.LogInformation("DNSimple payload {Id} is for an unknown API version: {ApiVersion}", payload.RequestId, payload.ApiVersion);
                return false;
            }

            if (!string.Equals(payload.Name, "certificate.issue", StringComparison.Ordinal))
            {
                _logger.LogInformation("DNSimple payload {Id} is of an unknown name: {Name}", payload.RequestId, payload.Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deserializes the payload from the specified HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request to deserialize the payload from.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to deserialize the request content,
        /// which returns an instance of <see cref="WebhookPayload"/> if successful; otherwise <see langword="null"/>.
        /// </returns>
        private async Task<WebhookPayload> DeserializePayloadAsync(HttpRequest request)
        {
            string json = await request.ReadAsStringAsync();

            _logger.LogDebug(@"Request content: ""{Content}""", json);

            return JsonConvert.DeserializeObject<WebhookPayload>(json);
        }

        /// <summary>
        /// Processes the certificate as an asynchronous operation.
        /// </summary>
        /// <param name="payload">The HTTP request to process the certificate payload for.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to process the certificate.
        /// </returns>
        private async Task<bool> ProcessCertificateAsync(WebhookPayload payload)
        {
            CertificateData data = await GetCertificateDataAsync(payload);

            await UploadCertificatesAsync(data);

            return true;
        }

        /// <summary>
        /// Gets the certificate data from the specified payload as an asynchronous operation.
        /// </summary>
        /// <param name="payload">The HTTP request to deserialize the certificate data from.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to return the certificate data.
        /// </returns>
        private async Task<CertificateData> GetCertificateDataAsync(WebhookPayload payload)
        {
            var payloadData = payload.Data.ToObject<Newtonsoft.Json.Linq.JObject>();
            Certificate certificate = payloadData["certificate"].ToObject<Certificate>();

            int accountId = payload.Account.Id;
            int domainId = certificate.DomainId;
            int certificateId = certificate.Id;

            if (accountId == 0)
            {
                throw new InvalidOperationException("Failed to deserialize the account Id from the payload.");
            }

            if (domainId == 0)
            {
                throw new InvalidOperationException("Failed to deserialize the domain Id from the payload.");
            }

            if (certificateId == 0)
            {
                throw new InvalidOperationException("Failed to deserialize the certificate Id from the payload.");
            }

            _logger.LogInformation(
                "Getting data from account {AccountId} for domain {DomainId} and certificate {CertificateId}.",
                accountId,
                domainId,
                certificate);

            IDNSimpleApi client = _apiFactory.Create();

            CertificateChain chain = (await client.GetCertificateChainAsync(accountId, domainId, certificateId)).Data;
            CertificatePrivateKey privateKey = (await client.GetCertificatePrivateKeyAsync(accountId, domainId, certificateId)).Data;

            _logger.LogInformation(
                "Downloaded certificate data from account {AccountId} for domain {DomainId} and certificate {CertificateId}.",
                accountId,
                domainId,
                certificate);

            var data = new CertificateData()
            {
                Chain = chain.Chain,
                PrivateKeyPem = privateKey.PrivateKey,
                Root = chain.Root,
                Server = chain.Server,
            };

            using (X509Certificate2 privateCertificate = X509CertificateHelpers.Combine(data.Server, data.PrivateKeyPem))
            {
                data.PrivateKeyPfx = privateCertificate.Export(X509ContentType.Pfx, _certificatePassword);
            }

            _logger.LogTrace("Extracted PFX private key from PEM private key.");

            return data;
        }

        /// <summary>
        /// Updates the certificates to blob storage as an asynchronous operation.
        /// </summary>
        /// <param name="data">The certificate data to upload to blob storage.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to upload the certificates.
        /// </returns>
        private async Task UploadCertificatesAsync(CertificateData data)
        {
            X509Certificate2 certificate = X509CertificateHelpers.CreateCertificate(data.Server);
            IDictionary<string, string> metadata = X509CertificateHelpers.GetMetadata(certificate);

            string commonName = certificate.Subject.Substring(3); // Remove "CN=" prefix
            string safeCommonName = commonName.Replace(".", "-", StringComparison.Ordinal);

            string thumbprintLower = certificate.Thumbprint.ToLowerInvariant();
            string timestamp = certificate.NotBefore.ToUniversalTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            metadata["CommonName"] = commonName;

            string blobPrefix = $"{safeCommonName}_{thumbprintLower}_{timestamp}";

            await UploadCertificatesAsync(data, "certificates", blobPrefix, metadata);
        }

        /// <summary>
        /// Updates the certificates to blob storage as an asynchronous operation.
        /// </summary>
        /// <param name="data">The certificate data to upload to blob storage.</param>
        /// <param name="containerName">The name of the container to upload the certificates to.</param>
        /// <param name="blobPrefix">The prefix to use for the blob names.</param>
        /// <param name="metadata">The metadata to associated with the blobs.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to upload the certificates.
        /// </returns>
        private async Task UploadCertificatesAsync(
            CertificateData data,
            string containerName,
            string blobPrefix,
            IDictionary<string, string> metadata)
        {
            _logger.LogInformation("Uploading certificates to container {ContainerName}.", containerName);

            int count = 0;

            if (data.Chain?.Count > 0)
            {
                var chainCerts = data.Chain.ToArray();

                if (chainCerts.Length == 1)
                {
                    await _blobClient.UploadTextAsync(containerName, $"{blobPrefix}.chain.pem", chainCerts[0], metadata);

                    count++;
                    _logger.LogTrace("Uploaded PEM chain certificate.");
                }
                else
                {
                    for (int i = 0; i < chainCerts.Length; i++)
                    {
                        await _blobClient.UploadTextAsync(containerName, $"{blobPrefix}.chain.{i}.pem", chainCerts[i], metadata);

                        count++;
                        _logger.LogTrace("Uploaded PEM chain certificate {Index}.", i);
                    }
                }
            }

            if (data.Server != null)
            {
                await _blobClient.UploadTextAsync(containerName, $"{blobPrefix}.cert.pem", data.Server, metadata);

                count++;
                _logger.LogTrace("Uploaded PEM server certificate.");
            }

            if (data.Root != null)
            {
                await _blobClient.UploadTextAsync(containerName, $"{blobPrefix}.root.pem", data.Root, metadata);

                count++;
                _logger.LogTrace("Uploaded PEM root certificate.");
            }

            if (data.PrivateKeyPem != null)
            {
                await _blobClient.UploadTextAsync(containerName, $"{blobPrefix}.privkey.pem", data.PrivateKeyPem, metadata);

                count++;
                _logger.LogTrace("Uploaded PEM private key.");
            }

            if (data.PrivateKeyPfx != null)
            {
                await _blobClient.UploadBytesAsync(containerName, $"{blobPrefix}.privkey.pfx", data.PrivateKeyPfx, metadata);

                count++;
                _logger.LogTrace("Uploaded PFX private key.");
            }

            _logger.LogInformation("Uploaded {Count} certificates to container {ContainerName}.", count, containerName);
        }

        private sealed class CertificateData
        {
            public string Root { get; set; }

            public ICollection<string> Chain { get; set; }

            public string Server { get; set; }

            public string PrivateKeyPem { get; set; }

            public byte[] PrivateKeyPfx { get; set; }
        }
    }
}
