// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing the account associated with a webhook event from DNSimple.
    /// </summary>
    public sealed class WebhookAccount
    {
        /// <summary>
        /// Gets or sets the account Id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier for the account.
        /// </summary>
        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets a printable representation of the account for use in display.
        /// </summary>
        [JsonProperty("display")]
        public string Display { get; set; }
    }
}
