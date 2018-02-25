// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.AzureFunctions.DNSimple.Client
{
    /// <summary>
    /// Defines a method for creating instances of <see cref="IDNSimpleApi"/>.
    /// </summary>
    public interface IDNSimpleApiFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IDNSimpleApi"/>.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="IDNSimpleApi"/>.
        /// </returns>
        IDNSimpleApi Create();
    }
}
