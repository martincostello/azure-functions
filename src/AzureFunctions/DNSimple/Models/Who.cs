// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing the result of querying the identity associated withe the API token.
    /// </summary>
    public sealed class Who
    {
        /// <summary>
        /// Gets or sets the user associated with the current identity, if any.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the account associated with the current identity, if any.
        /// </summary>
        [JsonProperty("account")]
        public Account Account { get; set; }
    }
}
