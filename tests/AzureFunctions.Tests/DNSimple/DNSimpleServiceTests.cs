// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JustEat.HttpClientInterception;
using MartinCostello.AzureFunctions.Blob;
using MartinCostello.AzureFunctions.DNSimple.Client;
using MartinCostello.AzureFunctions.DNSimple.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
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
            WebhookPayload payload = CreatePayload();

            string certificatePassword = Guid.NewGuid().ToString();
            IDNSimpleApiFactory apiFactory = CreateDNSimpleApiFactory();
            IBlobClient blobClient = CreateBlobClient();
            ILogger logger = _outputHelper.ToLogger<DNSimpleServiceTests>();

            var service = new DNSimpleService(certificatePassword, apiFactory, blobClient, logger);
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

        private static IBlobClient CreateBlobClient()
        {
            string containerName = "martincostello-io";

            var mock = new Mock<IBlobClient>();

            mock.Setup((p) => p.UploadBytesAsync(containerName, It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(Task.CompletedTask);

            mock.Setup((p) => p.UploadTextAsync(containerName, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(Task.CompletedTask);

            return mock.Object;
        }

        private static IDNSimpleApiFactory CreateDNSimpleApiFactory()
        {
            string publicCertificate = File.ReadAllText("self-signed.cer").Replace("\r\n", "\n", StringComparison.Ordinal);
            string privateCertificate = File.ReadAllText("self-signed.key").Replace("\r\n", "\n", StringComparison.Ordinal);

            var download = new
            {
                data = new
                {
                    server = publicCertificate,
                    root = null as string,
                    chain = new[] { publicCertificate },
                },
            };

            var privateKey = new
            {
                data = new
                {
                    private_key = privateCertificate,
                },
            };

            var options = new HttpClientInterceptorOptions()
            {
                ThrowOnMissingRegistration = true,
            };

            var builder = new HttpRequestInterceptionBuilder()
                .ForHttps()
                .ForHost("api.sandbox.dnsimple.com")
                .ForGet();

            builder.ForPath("v2/123/domains/456/certificates/555/download")
                   .WithJsonContent(download);

            options.Register(builder);

            builder.ForPath("v2/123/domains/456/certificates/555/private_key")
                   .WithJsonContent(privateKey);

            options.Register(builder);

            IDNSimpleApi CreateClient()
            {
                var httpClient = options.CreateHttpClient();
                httpClient.BaseAddress = new Uri("https://api.sandbox.dnsimple.com");

                return RestService.For<IDNSimpleApi>(httpClient);
            }

            var mock = new Mock<IDNSimpleApiFactory>();

            mock.Setup((p) => p.Create()).Returns(CreateClient);

            return mock.Object;
        }

        private static WebhookPayload CreatePayload()
        {
            var data = new
            {
                certificate = new
                {
                    id = 555,
                    csr = "-----BEGIN CERTIFICATE REQUEST-----\nVGhpcyBpcyBhIGZha2UgQ2VydGlmaWNhdGUgU2lnbmluZyBSZXF1ZXN0Lg==\n-----END CERTIFICATE REQUEST-----\n",
                    name = "www",
                    state = "issued",
                    years = 1,
                    domain_id = 456,
                    auto_renew = true,
                    contact_id = 789,
                    created_at = "2018-03-04T20:00:00Z",
                    expires_on = "2018-06-05",
                    updated_at = "2018-03-04T20:00:00Z",
                    common_name = "martincostello.io",
                    alternate_names = Array.Empty<string>(),
                    authority_identifier = "letsencrypt",
                },
            };

            WebhookPayload payload = WebhookPayloadHelpers.CreateValidPayload();

            payload.Data = Newtonsoft.Json.Linq.JObject.FromObject(data);

            return payload;
        }
    }
}
