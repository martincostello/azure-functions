// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using MartinCostello.AzureFunctions.AppService;
using MartinCostello.AzureFunctions.AppService.Client;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace MartinCostello.AzureFunctions
{
    /// <summary>
    /// A class representing the entry-point for a function that configures the TLS binding for
    /// Azure App Service instances when a new private key is uploaded to an Azure blob container.
    /// This class cannot be inherited.
    /// </summary>
    public static class BindPrivateCertificate
    {
        /// <summary>
        /// Runs the function to bind the TLS certificate to any relevant Azure App Service instances.
        /// </summary>
        /// <param name="certificate">The blob associated with the TLS certificate.</param>
        /// <param name="logger">The logger to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to bind the certificate.
        /// </returns>
        [FunctionName("BindPrivateCertificate")]
        public static async Task Run(
            [BlobTrigger("certificates/{name}.privkey.pfx", Connection = "CERTIFICATE_STORE_CONNECTION")] ICloudBlob certificate,
            ILogger logger)
        {
            var config = new FunctionsConfiguration();

            var client = new AppServiceClient(config, SdkContext.AzureCredentialsFactory);
            var clock = SystemClock.Instance;

            var service = new CertificateService(config.CertificatePassword, client, clock, logger);

            try
            {
                await service.BindAsync(certificate);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to bind private certificate {Certificate} from Azure blob.", certificate?.Name);
                throw;
            }
        }
    }
}
