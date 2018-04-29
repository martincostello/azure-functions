// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace MartinCostello.AzureFunctions.DNSimple.Client
{
    /// <summary>
    /// A class representing the default implementation of <see cref="IDNSimpleApiFactory"/>.
    /// </summary>
    public class DNSimpleApiFactory : IDNSimpleApiFactory
    {
        /// <summary>
        /// The host URL for the DNSimple API. This field is read-only.
        /// </summary>
        private readonly string _hostUrl;

        /// <summary>
        /// The bearer token to authenticate with the API. This field is read-only.
        /// </summary>
        private readonly Task<string> _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="DNSimpleApiFactory"/> class.
        /// </summary>
        /// <param name="hostUrl">The host URL for the DNSimple API.</param>
        /// <param name="token">The bearer token to authenticate with the API.</param>
        public DNSimpleApiFactory(string hostUrl, string token)
        {
            _hostUrl = hostUrl;
            _token = Task.FromResult(token);
        }

        /// <inheritdoc />
        public IDNSimpleApi Create()
        {
            var settings = new Refit.RefitSettings()
            {
                AuthorizationHeaderValueGetter = () => _token,
            };

            return Refit.RestService.For<IDNSimpleApi>(_hostUrl, settings);
        }
    }
}
