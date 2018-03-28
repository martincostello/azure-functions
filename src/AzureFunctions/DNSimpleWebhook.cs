// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
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
            string certificatePassword = GetOption("CERTIFICATE_PASSWORD") ?? string.Empty;
            string connectionString = GetOption("CERTIFICATE_STORE_CONNECTION") ?? "UseDevelopmentStorage=true";
            string hostUrl = GetOption("DNSIMPLE_URL") ?? "https://api.dnsimple.com";
            string token = GetOption("DNSIMPLE_TOKEN") ?? string.Empty;

            var apiFactory = new DNSimple.Client.DNSimpleApiFactory(hostUrl, token);
            var blobClient = new Blob.BlobClient(connectionString);

            var service = new DNSimple.DNSimpleService(
                certificatePassword,
                apiFactory,
                blobClient,
                logger);

            var result = await service.ProcessAsync(request);

            return new JsonResult(result)
            {
                StatusCode = result.StatusCode,
            };
        }

        private static string GetOption(string name) => Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}
