// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing an actor that triggered an event for a DNSimple webhook.
    /// </summary>
    public sealed class WebhookActor
    {
        /// <summary>
        /// Gets or sets a unique identifier for the actor.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a string which describes what type the actor is.
        /// </summary>
        [JsonProperty("entity")]
        public string Entity { get; set; }

        /// <summary>
        /// Gets or sets a printable representation of the actor for use in display.
        /// </summary>
        [JsonProperty("pretty")]
        public string Display { get; set; }
    }
}
