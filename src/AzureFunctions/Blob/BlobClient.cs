// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace MartinCostello.AzureFunctions.Blob
{
    /// <summary>
    /// A class representing an implementation of <see cref="IBlobClient"/> that uses Windows Azure storage. This class cannot be inherited.
    /// </summary>
    public sealed class BlobClient : IBlobClient
    {
        /// <summary>
        /// The <see cref="CloudBlobClient"/> to use. This field is read-only.
        /// </summary>
        private readonly CloudBlobClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobClient"/> class.
        /// </summary>
        /// <param name="connectionString">The cloud storage connection string.</param>
        public BlobClient(string connectionString)
        {
            var account = CloudStorageAccount.Parse(connectionString);
            _client = account.CreateCloudBlobClient();
        }

        /// <inheritdoc />
        public async Task UploadBytesAsync(string containerName, string blobName, byte[] buffer, IDictionary<string, string> metadata)
        {
            CloudBlockBlob blob = await GetBlobAsync(containerName, blobName);

            await blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
            await SetMetadataAsync(blob, metadata);
        }

        /// <inheritdoc />
        public async Task UploadTextAsync(string containerName, string blobName, string content, IDictionary<string, string> metadata)
        {
            CloudBlockBlob blob = await GetBlobAsync(containerName, blobName);

            await blob.UploadTextAsync(content);
            await SetMetadataAsync(blob, metadata);
        }

        /// <summary>
        /// Ensures that the specified blob container exists as an asynchronous operation.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that returns the specified cloud blob container.
        /// </returns>
        private async Task<CloudBlobContainer> EnsureContainerAsync(string containerName)
        {
            CloudBlobContainer container = _client.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            return container;
        }

        /// <summary>
        /// Gets the specified cloud blob blob as an asynchronous operation.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that returns the specified cloud block blob.
        /// </returns>
        private async Task<CloudBlockBlob> GetBlobAsync(string containerName, string blobName)
        {
            CloudBlobContainer container = await EnsureContainerAsync(containerName);
            return container.GetBlockBlobReference(blobName);
        }

        /// <summary>
        /// Sets the metadata on the specified cloud block blob as an asynchronous operation.
        /// </summary>
        /// <param name="blob">The blob to set the metadata for.</param>
        /// <param name="metadata">The metadata to set for the blob.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation to set the metadata for <paramref name="blob"/>.
        /// </returns>
        private async Task SetMetadataAsync(CloudBlockBlob blob, IDictionary<string, string> metadata)
        {
            if (metadata?.Count > 0)
            {
                foreach (var pair in metadata.Where((p) => !string.IsNullOrEmpty(p.Value)))
                {
                    blob.Metadata[pair.Key] = pair.Value;
                }

                await blob.SetMetadataAsync();
            }
        }
    }
}
