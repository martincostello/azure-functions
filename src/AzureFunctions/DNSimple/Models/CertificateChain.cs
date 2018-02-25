// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing a certificate chain in the DNSimple API.
    /// </summary>
    public sealed class CertificateChain
    {
        /// <summary>
        /// Gets or sets the server certificate.
        /// </summary>
        [JsonProperty("server")]
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the root certificate, if any.
        /// </summary>
        [JsonProperty("root")]
        public string Root { get; set; }

        /// <summary>
        /// Gets or sets the certificate chain.
        /// </summary>
        [JsonProperty("chain")]
        public ICollection<string> Chain { get; set; }
    }
}
