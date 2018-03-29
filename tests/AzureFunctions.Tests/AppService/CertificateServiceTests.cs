// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.AzureFunctions.AppService
{
    public class CertificateServiceTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public CertificateServiceTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
    }
}
