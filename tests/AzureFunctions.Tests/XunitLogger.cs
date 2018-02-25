// Copyright (c) Martin Costello, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MartinCostello.AzureFunctions
{
    internal sealed class XunitLogger : ILogger
    {
        private readonly ITestOutputHelper _outputHelper;

        public XunitLogger(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            _outputHelper.WriteLine($"[{logLevel}] [{eventId}] {message}");
        }
    }
}
