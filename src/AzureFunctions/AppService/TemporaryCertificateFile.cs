// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace MartinCostello.AzureFunctions.AppService
{
    public sealed class TemporaryCertificateFile : IDisposable
    {
        private bool _disposed;

        public TemporaryCertificateFile(byte[] rawData)
        {
            FileName = Path.GetTempFileName();
            File.WriteAllBytes(FileName, rawData);
        }

        ~TemporaryCertificateFile()
        {
            DisposeImpl();
        }

        public string FileName { get; }

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
