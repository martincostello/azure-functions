// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace MartinCostello.AzureFunctions
{
    public sealed class FunctionsConfigurationTests
    {
        [Fact]
        public static void FunctionsConfiguration_Reads_Environment_Variables()
        {
            // Arrange
            var environment = new Dictionary<string, string>()
            {
                { "AZURE_CREDENTIALS_FILE", "azure-creds.json" },
                { "APPSERVICE_SUBSCRIPTION_ID", "my_subscription_id" },
                { "CERTIFICATE_PASSWORD", "my_password" },
                { "CERTIFICATE_STORE_CONNECTION", "UseDevelopmentStorage=true" },
                { "DNSIMPLE_TOKEN", "my_token" },
                { "DNSIMPLE_URL", "https://dnsimple.local" },
                { "USE_MANAGED_SERVICE_IDENTITY", "false" },
            };

            foreach (var pair in environment)
            {
                Environment.SetEnvironmentVariable(pair.Key, pair.Value);
            }

            try
            {
                // Act
                var target = new FunctionsConfiguration();

                // Assert
                target.AzureCredentialsFile.ShouldBe("azure-creds.json");
                target.AzureSubscriptionId.ShouldBe("my_subscription_id");
                target.CertificatePassword.ShouldBe("my_password");
                target.CertificateStoreConnectionString.ShouldBe("UseDevelopmentStorage=true");
                target.DNSimpleToken.ShouldBe("my_token");
                target.DNSimpleUrl.ShouldBe("https://dnsimple.local");
                target.UseManagedServiceIdentity.ShouldBe(false);
            }
            finally
            {
                foreach (string key in environment.Keys)
                {
                    Environment.SetEnvironmentVariable(key, string.Empty);
                }
            }
        }
    }
}
