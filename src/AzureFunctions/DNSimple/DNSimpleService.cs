// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
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
        /// The <see cref="IDNSimpleApiFactory"/> to use. This field is read-only.
        /// </summary>
        private readonly IDNSimpleApiFactory _apiFactory;

        /// <summary>
        /// The <see cref="ILogger"/> to use. This field is read-only.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DNSimpleService"/> class.
        /// </summary>
        /// <param name="apiFactory">The <see cref="IDNSimpleApiFactory"/> to use.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public DNSimpleService(IDNSimpleApiFactory apiFactory, ILogger logger)
        {
            _apiFactory = apiFactory;
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
            }

            return result;
        }

        /// <summary>
        /// Returns whether the specified webhook payload can be handled.
        /// </summary>
        /// <param name="payload">The webhook payload.</param>
        /// <returns>
        /// <see langword="true"/> if the payload can be handled; otherwise <see langword="false"/>
        /// </returns>
        private bool CanHandlePayload(WebhookPayload payload)
        {
            if (!string.Equals(payload.ApiVersion, "v2", StringComparison.Ordinal))
            {
                _logger.LogInformation("DNSimple payload {Id} is for an unknown API version: {ApiVersion}", payload.RequestId, payload.ApiVersion);
                return false;
            }

            if (!string.Equals(payload.Name, "certificate.reissue", StringComparison.Ordinal))
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

        private async Task<bool> ProcessCertificateAsync(WebhookPayload payload)
        {
            await Task.Delay(1);
            return true;
        }
    }
}
