// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.AzureFunctions
{
    /// <summary>
    /// Defines the configuration for the functions.
    /// </summary>
    public interface IFunctionsConfiguration
    {
        /// <summary>
        /// Gets the path to a file containing the Azure credentials to use if not using Managed Service Identity.
        /// </summary>
        string AzureCredentialsFile { get; }

        /// <summary>
        /// Gets the Azure subscription Id to use.
        /// </summary>
        string AzureSubscriptionId { get; }

        /// <summary>
        /// Gets the password to use for PFX files.
        /// </summary>
        string CertificatePassword { get; }

        /// <summary>
        /// Gets the connection string for the Azure storage account to store certificates in.
        /// </summary>
        string CertificateStoreConnectionString { get; }

        /// <summary>
        /// Gets the authorization token to use for the DNSimple API.
        /// </summary>
        string DNSimpleToken { get; }

        /// <summary>
        /// Gets the URL of the DNSimple API.
        /// </summary>
        string DNSimpleUrl { get; }

        /// <summary>
        /// Gets the client Id to use when <see cref="UseServicePrincipalAuthentication"/> is <see langword="true"/>.
        /// </summary>
        string ServicePrincipalClientId { get; }

        /// <summary>
        /// Gets the client secret to use when <see cref="UseServicePrincipalAuthentication"/> is <see langword="true"/>.
        /// </summary>
        string ServicePrincipalClientSecret { get; }

        /// <summary>
        /// Gets the tenant Id to use when <see cref="UseServicePrincipalAuthentication"/> is <see langword="true"/>.
        /// </summary>
        string ServicePrincipalTenantId { get; }

        /// <summary>
        /// Gets a value indicating whether to use a Managed Service Identity for accessing Azure resources.
        /// </summary>
        bool UseManagedServiceIdentity { get; }

        /// <summary>
        /// Gets a value indicating whether to use a Service Principal for accessing Azure resources.
        /// </summary>
        bool UseServicePrincipalAuthentication { get; }
    }
}
