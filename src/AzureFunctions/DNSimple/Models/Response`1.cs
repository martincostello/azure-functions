// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing a response from the DNSimple API.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object returned by the DNSimple API.
    /// </typeparam>
    public sealed class Response<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets the data of the response.
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
