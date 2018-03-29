// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Moq;
using Shouldly;
using Xunit;

namespace MartinCostello.AzureFunctions.AppService.Client
{
    public static class AppServiceClientTests
    {
        [Fact]
        public static void AppServiceClient_Creates_MSI_Credentials()
        {
            // Arrange
            var mock = new Mock<IFunctionsConfiguration>();

            mock.Setup((p) => p.UseManagedServiceIdentity)
                .Returns(true);

            IFunctionsConfiguration config = mock.Object;

            var target = new AppServiceClient(config, SdkContext.AzureCredentialsFactory);

            // Act
            AzureCredentials actual = target.CreateCredentials();

            // Assert
            actual.ShouldNotBeNull();
            actual.Environment.ShouldBe(AzureEnvironment.AzureGlobalCloud);
            actual.ClientId.ShouldBeNull();
            actual.TenantId.ShouldBeNull();
        }

        [Fact]
        public static void AppServiceClient_Creates_Service_Principal_Credentials()
        {
            // Arrange
            var mock = new Mock<IFunctionsConfiguration>();

            mock.Setup((p) => p.ServicePrincipalClientId).Returns("client_id");
            mock.Setup((p) => p.ServicePrincipalClientSecret).Returns("client_secret");
            mock.Setup((p) => p.ServicePrincipalTenantId).Returns("tenant_id");
            mock.Setup((p) => p.UseServicePrincipalAuthentication).Returns(true);

            IFunctionsConfiguration config = mock.Object;

            var target = new AppServiceClient(config, SdkContext.AzureCredentialsFactory);

            // Act
            AzureCredentials actual = target.CreateCredentials();

            // Assert
            actual.ShouldNotBeNull();
            actual.Environment.ShouldBe(AzureEnvironment.AzureGlobalCloud);
            actual.ClientId.ShouldBe("client_id");
            actual.TenantId.ShouldBe("tenant_id");
        }
    }
}
