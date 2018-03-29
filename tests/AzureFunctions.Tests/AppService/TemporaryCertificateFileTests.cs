// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace MartinCostello.AzureFunctions.AppService
{
    public static class TemporaryCertificateFileTests
    {
        [Fact]
        public static async Task TemporaryCertificateFile_Creates_And_Deletes_File()
        {
            // Arrange
            string fileName;
            byte[] expected = new byte[256];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(expected);
            }

            // Act
            using (var target = new TemporaryCertificateFile(expected))
            {
                fileName = target.FileName;

                // Assert
                File.Exists(fileName).ShouldBeTrue();

                // Act
                byte[] actual = await File.ReadAllBytesAsync(fileName);

                // Assert
                actual.ShouldBe(expected);
            }

            // Assert
            File.Exists(fileName).ShouldBeFalse();
        }
    }
}
