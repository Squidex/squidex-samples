// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.CLI.Commands.Implementation
{
    public interface ILogger
    {
        void StepStart(string process);

        void StepFailed(Exception ex);

        void StepFailed(string reason);

        void StepSuccess();

        void StepSkipped(string reason);

        void WriteLine();

        void WriteLine(string message);

        void WriteLine(string message, params object[] args);

        ILogLine WriteSameLine();
    }
}
