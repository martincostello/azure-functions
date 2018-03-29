// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.AppService.Fluent;

namespace MartinCostello.AzureFunctions.AppService.Client
{
    /// <summary>
    /// Defines a client for the Azure App Service.
    /// </summary>
    public interface IAppServiceClient
    {
        /// <summary>
        /// Gets the available web applications as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get the available web applications.
        /// </returns>
        Task<ICollection<IWebApp>> GetApplicationsAsync();

        /// <summary>
        /// Updates a TLS binding for a web application as an asynchronous operation.
        /// </summary>
        /// <param name="application">The application to update the TLS binding for.</param>
        /// <param name="hostName">The host name to bind the certificate to.</param>
        /// <param name="certificate">The raw bytes of the certificate to bind.</param>
        /// <param name="password">The certificate password.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to
        /// update the TLS binding which returns the updated <see cref="IWebApp"/>.
        /// </returns>
        Task<IWebApp> UpdateBindingAsync(
            IWebApp application,
            string hostName,
            byte[] certificate,
            string password);
    }
}
