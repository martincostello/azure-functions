// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.AzureFunctions
{
    public class BindPrivateCertificateTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public BindPrivateCertificateTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task BindPrivateCertificate_Does_Not_Throw_If_Certificate_Is_Null()
        {
            // Arrange
            ILogger logger = new XunitLogger(_outputHelper);
            ICloudBlob certificate = null;

            // Act
            await BindPrivateCertificate.Run(certificate, logger);
        }

        [Fact]
        public async Task BindPrivateCertificate_Does_Not_Throw_If_Certificate_Is_Invalid()
        {
            // Arrange
            ILogger logger = new XunitLogger(_outputHelper);
            ICloudBlob certificate = Mock.Of<ICloudBlob>();

            // Act
            await BindPrivateCertificate.Run(certificate, logger);
        }

        [Fact(Skip = "For local testing of the function using the Storage Emulator.")]
        public async Task BindPrivateCertificate_Can_Bind_Certificate()
        {
            // Arrange
            string blobName = "CHANGE_ME";

            var logger = new XunitLogger(_outputHelper);
            var certificate = CloudStorageAccount.DevelopmentStorageAccount
                .CreateCloudBlobClient()
                .GetContainerReference("certificates")
                .GetBlockBlobReference(blobName);

            await certificate.FetchAttributesAsync();

            string json = File.ReadAllText("local.settings.json");
            var settings = JObject.Parse(json).Value<JObject>("Values");

            foreach (var pair in settings)
            {
                Environment.SetEnvironmentVariable(pair.Key, pair.Value.Value<string>());
            }

            await BindPrivateCertificate.Run(certificate, logger);
        }
    }
}
