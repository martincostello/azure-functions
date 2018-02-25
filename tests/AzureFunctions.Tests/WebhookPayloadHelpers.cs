// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using MartinCostello.AzureFunctions.DNSimple.Models;
using Newtonsoft.Json.Linq;

namespace MartinCostello.AzureFunctions
{
    /// <summary>
    /// A class containing helper methods for working with <see cref="WebhookPayload"/> instances. This class cannot be inherited.
    /// </summary>
    internal static class WebhookPayloadHelpers
    {
        /// <summary>
        /// Creates a valid instance of <see cref="WebhookPayload"/> for a certificate reissue.
        /// </summary>
        /// <returns>
        /// A valid instance of <see cref="WebhookPayload"/> for certificate reissue.
        /// </returns>
        internal static WebhookPayload CreateValidPayload()
        {
            return new WebhookPayload()
            {
                Name = "certificate.reissue",
                ApiVersion = "v2",
                RequestId = "abc123",
                Account = new WebhookAccount()
                {
                    Id = 123,
                    Identifier = "foo-bar",
                    Display = "My Account",
                },
                Actor = new WebhookActor()
                {
                    Id = "actor-id",
                    Entity = "some-entity",
                    Display = "An Entity",
                },
                Data = new JObject(),
            };
        }
    }
}
