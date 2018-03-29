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
                { "AZURE_SUBSCRIPTION_ID", "my_subscription_id" },
                { "CERTIFICATE_PASSWORD", "my_password" },
                { "CERTIFICATE_STORE_CONNECTION", "UseDevelopmentStorage=true" },
                { "DNSIMPLE_TOKEN", "my_token" },
                { "DNSIMPLE_URL", "https://dnsimple.local" },
                { "SERVICE_PRINCIPAL_CLIENT_ID", "client_id" },
                { "SERVICE_PRINCIPAL_CLIENT_SECRET", "client_secret" },
                { "SERVICE_PRINCIPAL_TENANT_ID", "tenant_id" },
                { "USE_MANAGED_SERVICE_IDENTITY", "false" },
                { "USE_SERVICE_PRINCIPAL", "true" },
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
                target.ServicePrincipalClientId.ShouldBe("client_id");
                target.ServicePrincipalClientSecret.ShouldBe("client_secret");
                target.ServicePrincipalTenantId.ShouldBe("tenant_id");
                target.UseManagedServiceIdentity.ShouldBe(false);
                target.UseServicePrincipalAuthentication.ShouldBe(true);
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
