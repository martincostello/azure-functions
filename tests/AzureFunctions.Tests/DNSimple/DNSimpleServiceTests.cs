// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using MartinCostello.AzureFunctions.DNSimple.Client;
using MartinCostello.AzureFunctions.DNSimple.Models;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.AzureFunctions.DNSimple
{
    public class DNSimpleServiceTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public DNSimpleServiceTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task DNSimpleWebhook_Returns_Http_200_For_Certificate_Reissue()
        {
            // Arrange
            WebhookPayload payload = WebhookPayloadHelpers.CreateValidPayload();

            IDNSimpleApiFactory apiFactory = null;
            ILogger logger = new XunitLogger(_outputHelper);

            DNSimpleService service = new DNSimpleService(apiFactory, logger);

            var request = new MockHttpRequest(payload);

            // Act
            WebhookResult actual = await service.ProcessAsync(request);

            // Assert
            actual.ShouldNotBeNull();
            actual.Message.ShouldBe("Webhook 'abc123' acknowledged.");
            actual.Processed.ShouldBeTrue();
            actual.RequestId.ShouldBe(payload.RequestId);
            actual.StatusCode.ShouldBe(200);
        }
    }
}
