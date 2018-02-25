// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MartinCostello.AzureFunctions
{
    public static class DNSimpleWebhook
    {
        [FunctionName("DNSimpleWebhook")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "webhooks/dnsimple")] HttpRequest request,
            ILogger logger)
        {
            logger.LogInformation("HTTP trigger function processed a request for DNSimpleWebhook.");

            var value = new
            {
                message = "Response from DNSimpleWebhook."
            };

            return new JsonResult(value);
        }
    }
}
