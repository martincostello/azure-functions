// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing a certificate private key in the DNSimple API.
    /// </summary>
    public sealed class CertificatePrivateKey
    {
        /// <summary>
        /// Gets or sets the private key of the certificate.
        /// </summary>
        [JsonProperty("private_key")]
        public string PrivateKey { get; set; }
    }
}
