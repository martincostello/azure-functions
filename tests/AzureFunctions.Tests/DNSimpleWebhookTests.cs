// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using MartinCostello.AzureFunctions.DNSimple.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            WebhookPayload payload = WebhookPayloadHelpers.CreateValidPayload();
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
            WebhookPayload payload = WebhookPayloadHelpers.CreateValidPayload();
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

        private async Task<IActionResult> InvokeAsync(object payload)
        {
            ILogger logger = _outputHelper.ToLogger<DNSimpleWebhookTests>();
            HttpRequest request = new MockHttpRequest(payload);

            return await DNSimpleWebhook.Run(request, logger);
        }
    }
}
