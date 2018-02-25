// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing a webhook payload from DNSimple.
    /// </summary>
    public sealed class WebhookPayload
    {
        /// <summary>
        /// Gets or sets a string representing the name of event that occurred.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the API version used to serialize the data in the payload.
        /// </summary>
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets a UUID that provides a way to identify the request.
        /// </summary>
        [JsonProperty("request_identifier")]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets any data for the object or objects related to the event.
        /// </summary>
        [JsonProperty("data")]
        public JObject Data { get; set; }

        /// <summary>
        /// Gets or sets an object describing which account the event occurred in.
        /// </summary>
        [JsonProperty("account")]
        public WebhookAccount Account { get; set; }

        /// <summary>
        /// Gets or sets an object describing the entity that triggered the event.
        /// </summary>
        [JsonProperty("actor")]
        public WebhookActor Actor { get; set; }
    }
}
