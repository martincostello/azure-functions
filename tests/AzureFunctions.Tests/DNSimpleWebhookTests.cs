// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace MartinCostello.AzureFunctions
{
    public static class DNSimpleWebhookTests
    {
        [Fact]
        public static void LetsEncryptWebhook_Returns_Json_Response()
        {
            // Arrange
            HttpContext httpContext = new DefaultHttpContext();
            ILogger logger = Mock.Of<ILogger>();

            // Act
            IActionResult actual = DNSimpleWebhook.Run(httpContext.Request, logger);

            // Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType<JsonResult>();
        }
    }
}
