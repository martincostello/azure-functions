// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MartinCostello.AzureFunctions.AppService.Client;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Storage.Blob;
using Moq;
using NodaTime;
using NodaTime.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.AzureFunctions.AppService
{
    public class CertificateServiceTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public CertificateServiceTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task CertificateService_Does_Nothing_If_No_Web_Applications()
        {
            // Arrange
            ICloudBlob certificate = CreateBlob();
            CertificateService target = CreateService();

            // Act
            int actual = await target.BindAsync(certificate);

            // Assert
            actual.ShouldBe(0);
        }

        [Fact]
        public async Task CertificateService_Does_Nothing_If_No_Web_Applications_For_Host_Names()
        {
            // Arrange
            IWebApp application = CreateWebApp();
            ICloudBlob certificate = CreateBlob();

            CertificateService target = CreateService(new[] { application });

            // Act
            int actual = await target.BindAsync(certificate);

            // Assert
            actual.ShouldBe(0);
        }

        [Fact]
        public async Task CertificateService_Does_Not_Update_Bindings_If_Certificate_Not_Yet_Valid()
        {
            // Arrange
            IHostNameBinding binding = CreateBinding("site.local", thumbprint: "thumbprint");
            IWebApp application = CreateWebApp("site.local", bindings: new[] { binding });

            ICloudBlob certificate = CreateBlob();
            var clock = new FakeClock(Instant.FromUtc(2018, 03, 04, 20, 17, 06));

            CertificateService target = CreateService(new[] { application }, clock: clock);

            // Act
            int actual = await target.BindAsync(certificate);

            // Assert
            actual.ShouldBe(0);
        }

        [Fact]
        public async Task CertificateService_Does_Not_Update_Bindings_If_Certificate_Has_Expired()
        {
            // Arrange
            IHostNameBinding binding = CreateBinding("site.local", thumbprint: "thumbprint");
            IWebApp application = CreateWebApp("site.local", bindings: new[] { binding });

            ICloudBlob certificate = CreateBlob();
            var clock = new FakeClock(Instant.FromUtc(2019, 03, 04, 20, 37, 08));

            CertificateService target = CreateService(new[] { application }, clock: clock);

            // Act
            int actual = await target.BindAsync(certificate);

            // Assert
            actual.ShouldBe(0);
        }

        [Fact]
        public async Task CertificateService_Updates_Bindings_If_Web_Applications_For_Host_Name()
        {
            // Arrange
            string previousThumprint = "old_thumbprint";
            string currentThumbprint = "8bc2d71c7c4b1169f143eab75f6b1ee7b8a33627";

            IHostNameBinding binding1 = CreateBinding("site.local", thumbprint: previousThumprint);
            IHostNameBinding binding2 = CreateBinding("martincostello.io", thumbprint: previousThumprint);
            IHostNameBinding binding3 = CreateBinding("martincostello.io", thumbprint: currentThumbprint);
            IHostNameBinding binding4 = CreateBinding("other.local", thumbprint: "some_other_thumbprint");

            IWebApp application1 = CreateWebApp("site-east-us", bindings: new[] { binding1 });
            IWebApp application2 = CreateWebApp("site-uk-south", bindings: new[] { binding2 });
            IWebApp application3 = CreateWebApp("site-uk-west", bindings: new[] { binding3 });
            IWebApp application4 = CreateWebApp("some-other-site", bindings: new[] { binding4 });

            ICloudBlob certificate = CreateBlob();

            CertificateService target = CreateService(new[] { application1, application2, application3, application4 });

            // Act
            int actual = await target.BindAsync(certificate);

            // Assert
            actual.ShouldBe(2);
        }

        private static ICloudBlob CreateBlob(string commonName = "site.local")
        {
            byte[] certificate = File.ReadAllBytes("self-signed.pfx");

            var metadata = new Dictionary<string, string>()
            {
                { "CommonName", commonName },
            };

            var mock = new Mock<ICloudBlob>();

            mock.Setup((p) => p.DownloadToStreamAsync(It.IsAny<Stream>()))
                .Callback((Stream stream) => stream.WriteAsync(certificate, 0, certificate.Length))
                .Returns(Task.CompletedTask);

            mock.Setup((p) => p.Metadata)
                .Returns(metadata);

            mock.Setup((p) => p.Uri)
                .Returns(new Uri("https://localhost/certificates/certificate.pfx"));

            return mock.Object;
        }

        private static IHostNameBinding CreateBinding(string hostName, string thumbprint)
        {
            var inner = new HostNameBindingInner()
            {
                Thumbprint = thumbprint,
            };

            var mock = new Mock<IHostNameBinding>();

            mock.Setup((p) => p.HostName)
                .Returns(hostName);

            mock.Setup((p) => p.Inner)
                .Returns(inner);

            return mock.Object;
        }

        private static IWebApp CreateWebApp(
            string name = "site.local",
            ICollection<IHostNameBinding> bindings = null)
        {
            if (bindings == null)
            {
                bindings = Array.Empty<IHostNameBinding>();
            }

            var hostNameBindings = bindings.ToDictionary((p) => p.HostName, (p) => p);

            var mock = new Mock<IWebApp>();

            mock.Setup((p) => p.Name)
                .Returns(name);

            mock.Setup((p) => p.GetHostNameBindingsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(hostNameBindings);

            return mock.Object;
        }

        private CertificateService CreateService(
            ICollection<IWebApp> applications = null,
            IClock clock = null)
        {
            string certificatePassword = "password";

            var mock = new Mock<IAppServiceClient>();

            mock.Setup((p) => p.GetApplicationsAsync())
                .ReturnsAsync(applications ?? Array.Empty<IWebApp>());

            mock.Setup((p) => p.UpdateBindingAsync(It.IsNotNull<IWebApp>(), It.IsNotNull<string>(), It.IsNotNull<string>(), It.IsNotNull<byte[]>(), certificatePassword))
                .ReturnsAsync(Mock.Of<IWebApp>());

            IAppServiceClient client = mock.Object;
            var logger = _outputHelper.ToLogger<CertificateService>();

            if (clock == null)
            {
                clock = SystemClock.Instance;
            }

            return new CertificateService(certificatePassword, client, clock, logger);
        }
    }
}
