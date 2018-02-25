// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MartinCostello.AzureFunctions.DNSimple.Models;
using Refit;

namespace MartinCostello.AzureFunctions.DNSimple.Client
{
    /// <summary>
    /// Defines the interface to the DNSimple API.
    /// </summary>
    [Headers("Accept: application/json")]
    public interface IDNSimpleApi
    {
        /// <summary>
        /// Gets the certificates for an account and domain as an asynchronous operation.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <param name="domainId">The domain Id.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation
        /// to get the domains for the specified account Id and domain Id.
        /// </returns>
        [Get("/v2/{accountId}/domains/{domainId}/certificates")]
        Task<Response<ICollection<Certificate>>> GetCertificatesAsync(int accountId, int domainId);

        /// <summary>
        /// Gets a certificate as an asynchronous operation.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <param name="domainId">The domain Id.</param>
        /// <param name="certificateId">The certificate Id.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation
        /// to get the certificate with the specified Id.
        /// </returns>
        [Get("/v2/{accountId}/domains/{domainId}/certificates/{certificateId}")]
        Task<Response<Certificate>> GetCertificateAsync(int accountId, int domainId, int certificateId);

        /// <summary>
        /// Gets a certificate's chain as an asynchronous operation.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <param name="domainId">The domain Id.</param>
        /// <param name="certificateId">The certificate Id.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation
        /// to get the chain for the certificate with the specified Id.
        /// </returns>
        [Get("/v2/{accountId}/domains/{domainId}/certificates/{certificateId}/download")]
        Task<Response<CertificateChain>> GetCertificateChainAsync(int accountId, int domainId, int certificateId);

        /// <summary>
        /// Gets a certificate's private key as an asynchronous operation.
        /// </summary>
        /// <param name="accountId">The account Id.</param>
        /// <param name="domainId">The domain Id.</param>
        /// <param name="certificateId">The certificate Id.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation
        /// to get the private key for the certificate with the specified Id.
        /// </returns>
        [Get("/v2/{accountId}/domains/{domainId}/certificates/{certificateId}/private_key")]
        Task<Response<CertificatePrivateKey>> GetCertificatePrivateKeyAsync(int accountId, int domainId, int certificateId);
    }
}
