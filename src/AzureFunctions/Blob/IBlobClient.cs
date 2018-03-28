// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartinCostello.AzureFunctions.Blob
{
    /// <summary>
    /// Defines methods for uploading data to blob storage.
    /// </summary>
    public interface IBlobClient
    {
        /// <summary>
        /// Uploads the specified byte array to blob storage as an asynchronous operation.
        /// </summary>
        /// <param name="containerName">The container to upload the data to.</param>
        /// <param name="blobName">The name of the blob to upload the data to.</param>
        /// <param name="buffer">The data to upload to blob storage.</param>
        /// <param name="metadata">The optional metadata to associate with the blob.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to upload the data.
        /// </returns>
        Task UploadBytesAsync(string containerName, string blobName, byte[] buffer, IDictionary<string, string> metadata);

        /// <summary>
        /// Uploads the specified text content to blob storage as an asynchronous operation.
        /// </summary>
        /// <param name="containerName">The container to upload the data to.</param>
        /// <param name="blobName">The name of the blob to upload the data to.</param>
        /// <param name="content">The content to upload to blob storage.</param>
        /// <param name="metadata">The optional metadata to associate with the blob.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to upload the content.
        /// </returns>
        Task UploadTextAsync(string containerName, string blobName, string content, IDictionary<string, string> metadata);
    }
}
