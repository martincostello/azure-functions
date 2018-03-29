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
    }
}
