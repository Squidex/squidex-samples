// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation;

public interface ILogger
{
    void StepStart(string process);

    void StepFailed(string reason);

    void StepSuccess(string? details = null);

    void StepSkipped(string reason);

    void WriteLine();

    void WriteJson(object message);

    void WriteLine(string message);

    void WriteLine(string message, params object?[] args);

    ILogLine WriteSameLine();
}
