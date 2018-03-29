// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MartinCostello.AzureFunctions.AppService
{
    /// <summary>
    /// Defines a method for binding TLS certificates from Azure blob storage.
    /// </summary>
    public interface ICertificateService
    {
        /// <summary>
        /// Binds a TLS certificate as an asynchronous operation.
        /// </summary>
        /// <param name="certificate">A cloud blob representing a TLS certificate.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation to bind
        /// the TLS certificate represented by <paramref name="certificate"/> which returns
        /// the number of services the certificate was bound to.
        /// </returns>
        Task<int> BindAsync(ICloudBlob certificate);
    }
}
