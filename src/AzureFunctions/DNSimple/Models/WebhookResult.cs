// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing the response from processing a DNSimple webhook request.
    /// </summary>
    public sealed class WebhookResult
    {
        /// <summary>
        /// Gets or sets the Id of the webhook request that was received.
        /// </summary>
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the message for the result.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the webhook was processed.
        /// </summary>
        [JsonProperty("processed")]
        public bool Processed { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code associated with the response.
        /// </summary>
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets any details associated with the error.
        /// </summary>
        [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
        public object Details { get; set; }
    }
}
