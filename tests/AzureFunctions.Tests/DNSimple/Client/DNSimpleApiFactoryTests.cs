// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using MartinCostello.AzureFunctions.DNSimple.Models;
using Shouldly;
using Xunit;

namespace MartinCostello.AzureFunctions.DNSimple.Client
{
    public static class DNSimpleApiFactoryTests
    {
        [Fact]
        public static void Can_Create_IDNSimpleApi_Instance()
        {
            // Arrange
            string hostUrl = "https://api.dnsimple.com";
            string token = "not_a_real_token";

            IDNSimpleApiFactory factory = new DNSimpleApiFactory(hostUrl, token);

            // Act
            IDNSimpleApi actual = factory.Create();

            // Assert
            actual.ShouldNotBeNull();
        }

        [Theory(Skip = "Requires an API token.")]
        [InlineData("https://api.dnsimple.com", "")]
        [InlineData("https://api.sandbox.dnsimple.com", "")]
        public static async Task Can_Query_DNSimple_Api(string hostUrl, string token)
        {
            // Arrange
            IDNSimpleApiFactory factory = new DNSimpleApiFactory(hostUrl, token);
            IDNSimpleApi client = factory.Create();

            // Act
            Response<Who> actual = await client.GetWhoAmIAsync();

            // Assert
            actual.ShouldNotBeNull();
            actual.Data.ShouldNotBeNull();
            (actual.Data.User == null ^ actual.Data.Account == null).ShouldBeTrue();
        }
    }
}
