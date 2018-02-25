// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace MartinCostello.AzureFunctions
{
    internal sealed class MockHttpRequest : DefaultHttpRequest
    {
        private readonly MemoryStream _stream;

        internal MockHttpRequest(object content)
            : base(new DefaultHttpContext())
        {
            byte[] buffer = Array.Empty<byte>();

            if (content != null)
            {
                string json = JsonConvert.SerializeObject(content);
                buffer = Encoding.UTF8.GetBytes(json);
            }

            _stream = new MemoryStream(buffer);
        }

        public override Stream Body { get => _stream; }
    }
}
