// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using MartinCostello.AzureFunctions.Blob;
using MartinCostello.AzureFunctions.DNSimple;
using MartinCostello.AzureFunctions.DNSimple.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MartinCostello.AzureFunctions
{
    /// <summary>
    /// A class representing the entry-point for a DNSimple webhook request. This class cannot be inherited.
    /// </summary>
    public static class DNSimpleWebhook
    {
        /// <summary>
        /// Runs the function for a DNSimple webhook request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="logger">The logger to use.</param>
        /// <returns>
        /// The result of the webhook.
        /// </returns>
        [FunctionName("DNSimpleWebhook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "webhooks/dnsimple")] HttpRequest request,
            ILogger logger)
        {
            var config = new FunctionsConfiguration();

            var apiFactory = new DNSimpleApiFactory(config.DNSimpleUrl, config.DNSimpleToken);
            var blobClient = new BlobClient(config.CertificateStoreConnectionString);

            var service = new DNSimpleService(
                config.CertificatePassword,
                apiFactory,
                blobClient,
                logger);

            var result = await service.ProcessAsync(request);

            return new JsonResult(result)
            {
                StatusCode = result.StatusCode,
            };
        }
    }
}
