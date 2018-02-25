// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions.DNSimple.Models
{
    /// <summary>
    /// A class representing a certificate in the DNSimple API.
    /// </summary>
    public sealed class Certificate
    {
        /// <summary>
        /// Gets or sets the Id of the certificate.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Id of the domain associated with the certificate.
        /// </summary>
        [JsonProperty("domain_id")]
        public int DomainId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the contact who requested the certificate.
        /// </summary>
        [JsonProperty("contact_id")]
        public int ContactId { get; set; }

        /// <summary>
        /// Gets or sets the name of the certificate.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the common name of the certificate.
        /// </summary>
        [JsonProperty("common_name")]
        public string CommonName { get; set; }

        /// <summary>
        /// Gets or sets the number of years the certificate is valid for.
        /// </summary>
        [JsonProperty("years")]
        public int Years { get; set; }

        /// <summary>
        /// Gets or sets the Certificate Signing Request.
        /// </summary>
        [JsonProperty("csr")]
        public string Csr { get; set; }

        /// <summary>
        /// Gets or sets the state of the certificate.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the certificate is auto-renewed.
        /// </summary>
        [JsonProperty("auto_renew")]
        public bool AutoRenew { get; set; }

        /// <summary>
        /// Gets or sets the alternate names for the certificate, if any.
        /// </summary>
        [JsonProperty("alternate_names")]
        public ICollection<string> AlternateNames { get; set; }

        /// <summary>
        /// Gets or sets the Id of the authority that issued the certificate.
        /// </summary>
        [JsonProperty("authority_identifier")]
        public string AuthorityId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the certificate was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time the certificate was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time the certificate expires.
        /// </summary>
        [JsonProperty("expires_on")]
        public DateTimeOffset ExpiresOn { get; set; }
    }
}
