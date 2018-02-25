// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using MartinCostello.AzureFunctions.DNSimple.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.AzureFunctions
{
    public class DNSimpleWebhookTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public DNSimpleWebhookTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task DNSimpleWebhook_Returns_Http_400_For_Invalid_Request_Body()
        {
            // Arrange
            object payload = string.Empty;

            // Act
            IActionResult actual = await InvokeAsync(payload);

            // Assert
            actual.ShouldNotBeNull();
            var response = actual.ShouldBeOfType<JsonResult>();

            response.StatusCode.ShouldBe(400);

            response.Value.ShouldNotBeNull();
            var result = response.Value.ShouldBeOfType<WebhookResult>();

            result.Message.ShouldBe("Bad request.");
            result.Processed.ShouldBeFalse();
            result.RequestId.ShouldBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public async Task DNSimpleWebhook_Returns_Http_200_For_Unknown_Version()
        {
            // Arrange
            WebhookPayload payload = CreateValidPayload();
            payload.ApiVersion = "foo";

            // Act
            IActionResult actual = await InvokeAsync(payload);

            // Assert
            actual.ShouldNotBeNull();
            var response = actual.ShouldBeOfType<JsonResult>();

            response.StatusCode.ShouldBe(200);

            response.Value.ShouldNotBeNull();
            var result = response.Value.ShouldBeOfType<WebhookResult>();

            result.Message.ShouldBe("Webhook 'abc123' acknowledged.");
            result.Processed.ShouldBeFalse();
            result.RequestId.ShouldBe(payload.RequestId);
            result.StatusCode.ShouldBe(200);
        }

        [Fact]
        public async Task DNSimpleWebhook_Returns_Http_200_For_Unknown_Name()
        {
            // Arrange
            WebhookPayload payload = CreateValidPayload();
            payload.Name = "zone.create";

            // Act
            IActionResult actual = await InvokeAsync(payload);

            // Assert
            actual.ShouldNotBeNull();
            var response = actual.ShouldBeOfType<JsonResult>();

            response.StatusCode.ShouldBe(200);

            response.Value.ShouldNotBeNull();
            var result = response.Value.ShouldBeOfType<WebhookResult>();

            result.Message.ShouldBe("Webhook 'abc123' acknowledged.");
            result.Processed.ShouldBeFalse();
            result.RequestId.ShouldBe(payload.RequestId);
            result.StatusCode.ShouldBe(200);
        }

        [Fact]
        public async Task DNSimpleWebhook_Returns_Http_200_For_Certificate_Reissue()
        {
            // Arrange
            WebhookPayload payload = CreateValidPayload();

            // Act
            IActionResult actual = await InvokeAsync(payload);

            // Assert
            actual.ShouldNotBeNull();
            var response = actual.ShouldBeOfType<JsonResult>();

            response.StatusCode.ShouldBe(200);

            response.Value.ShouldNotBeNull();
            var result = response.Value.ShouldBeOfType<WebhookResult>();

            result.Message.ShouldBe("Webhook 'abc123' acknowledged.");
            result.Processed.ShouldBeTrue();
            result.RequestId.ShouldBe(payload.RequestId);
            result.StatusCode.ShouldBe(200);
        }

        private static WebhookPayload CreateValidPayload()
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

        private async Task<IActionResult> InvokeAsync(object payload)
        {
            ILogger logger = new XunitLogger(_outputHelper);
            HttpRequest request = new MockHttpRequest(payload);

            return await DNSimpleWebhook.Run(request, logger);
        }
    }
}
