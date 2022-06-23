// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation
{
    public interface ILogLine : IDisposable
    {
        bool CanWriteToSameLine { get; }

        void WriteLine(string message);

        void WriteLine(string message, params object[] args);
    }
}
