// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;

namespace MartinCostello.AzureFunctions
{
    /// <summary>
    /// A class representing the default implementation of <see cref="IFunctionsConfiguration"/>. This class cannot be inherited.
    /// </summary>
    public sealed class FunctionsConfiguration : IFunctionsConfiguration
    {
        /// <inheritdoc />
        public string AzureCredentialsFile => GetOption("AZURE_CREDENTIALS_FILE");

        /// <inheritdoc />
        public string AzureSubscriptionId => GetOption("APPSERVICE_SUBSCRIPTION_ID");

        /// <inheritdoc />
        public string CertificatePassword => GetOption("CERTIFICATE_PASSWORD");

        /// <inheritdoc />
        public string CertificateStoreConnectionString => GetOption("CERTIFICATE_STORE_CONNECTION", "UseDevelopmentStorage=true");

        /// <inheritdoc />
        public string DNSimpleToken => GetOption("DNSIMPLE_TOKEN");

        /// <inheritdoc />
        public string DNSimpleUrl => GetOption("DNSIMPLE_URL", "https://api.dnsimple.com");

        public bool UseManagedServiceIdentity => string.Equals(GetOption("USE_MANAGED_SERVICE_IDENTITY", bool.TrueString), bool.TrueString, StringComparison.OrdinalIgnoreCase);

        private static string GetOption(string name, string defaultValue = "") =>
            Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process) ?? defaultValue;
    }
}
