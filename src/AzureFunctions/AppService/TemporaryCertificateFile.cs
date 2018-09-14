// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace MartinCostello.AzureFunctions.AppService
{
    /// <summary>
    /// A class representing a temporary X.509 certificate file. This class cannot be inherited.
    /// </summary>
    public sealed class TemporaryCertificateFile : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryCertificateFile"/> class.
        /// </summary>
        /// <param name="rawData">The raw certificate data.</param>
        public TemporaryCertificateFile(byte[] rawData)
        {
            FileName = Path.GetTempFileName();
            File.WriteAllBytes(FileName, rawData);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TemporaryCertificateFile"/> class.
        /// </summary>
        ~TemporaryCertificateFile()
        {
            DisposeImpl();
        }

        /// <summary>
        /// Gets the temporary file name associated with the certificate.
        /// </summary>
        public string FileName { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            DisposeImpl();
            GC.SuppressFinalize(this);
        }

        private void DisposeImpl()
        {
            if (!_disposed)
            {
                try
                {
                    File.Delete(FileName);
                    _disposed = true;
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }
    }
}
