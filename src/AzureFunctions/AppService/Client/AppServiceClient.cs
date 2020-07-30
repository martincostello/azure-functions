// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;

namespace MartinCostello.AzureFunctions.AppService.Client
{
    /// <summary>
    /// A class representing the default implementation of <see cref="IAppServiceClient"/>.
    /// </summary>
    public class AppServiceClient : IAppServiceClient
    {
        /// <summary>
        /// The <see cref="IFunctionsConfiguration"/> to use. This field is read-only.
        /// </summary>
        private readonly IFunctionsConfiguration _config;

        /// <summary>
        /// The <see cref="AzureCredentialsFactory"/> to use. This field is read-only.
        /// </summary>
        private readonly AzureCredentialsFactory _credentialsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceClient"/> class.
        /// </summary>
        /// <param name="config">The <see cref="IFunctionsConfiguration"/> to use.</param>
        /// <param name="credentialsFactory">The <see cref="AzureCredentialsFactory"/> to use.</param>
        public AppServiceClient(
            IFunctionsConfiguration config,
            AzureCredentialsFactory credentialsFactory)
        {
            _config = config;
            _credentialsFactory = credentialsFactory;
        }

        /// <inheritdoc />
        public async Task<ICollection<IWebApp>> GetApplicationsAsync()
        {
            IAzure service = CreateService();
            return (await service.WebApps.ListAsync()).ToList();
        }

        /// <inheritdoc />
        public async Task<IWebApp> UpdateBindingAsync(
            IWebApp application,
            string hostName,
            string thumbprint,
            byte[] certificate,
            string password)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            using (var certificateFile = new TemporaryCertificateFile(certificate))
            {
                // See https://github.com/Azure/azure-libraries-for-net/issues/56#issuecomment-335958896
                await application.Manager.AppServiceCertificates
                    .Define(string.Format(CultureInfo.InvariantCulture, "{0}##{1}#", thumbprint, application.Region.Name))
                    .WithRegion(application.Region)
                    .WithExistingResourceGroup(application.ResourceGroupName)
                    .WithPfxFile(certificateFile.FileName)
                    .WithPfxPassword(password)
                    .CreateAsync();
            }

            return await application.RefreshAsync();
        }

        /// <summary>
        /// Creates a set of credentials to use for Azure service management.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="AzureCredentials"/> to use.
        /// </returns>
        public AzureCredentials CreateCredentials()
        {
            var environment = AzureEnvironment.AzureGlobalCloud;

            if (_config.UseManagedServiceIdentity)
            {
                return _credentialsFactory.FromMSI(new MSILoginInformation(MSIResourceType.AppService), environment);
            }
            else if (_config.UseServicePrincipalAuthentication)
            {
                return _credentialsFactory.FromServicePrincipal(
                    _config.ServicePrincipalClientId,
                    _config.ServicePrincipalClientSecret,
                    _config.ServicePrincipalTenantId,
                    environment);
            }
            else
            {
                string authFile = _config.AzureCredentialsFile;

                if (string.IsNullOrEmpty(authFile))
                {
                    authFile = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        ".azure",
                        "credentials.json");
                }

                return _credentialsFactory.FromFile(authFile);
            }
        }

        /// <summary>
        /// Creates an authenticated instance of <see cref="IAzure"/>.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="IAzure"/> to use.
        /// </returns>
        protected virtual IAzure CreateService()
        {
            AzureCredentials credentials = CreateCredentials();

            return Azure
                .Configure()
                .Authenticate(credentials)
                .WithSubscription(_config.AzureSubscriptionId);
        }
    }
}
