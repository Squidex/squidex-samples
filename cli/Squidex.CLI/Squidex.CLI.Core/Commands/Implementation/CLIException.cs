// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Runtime.Serialization;

namespace Squidex.CLI.Commands.Implementation;

[Serializable]
public class CLIException : Exception
{
    public CLIException()
    {
    }

    public CLIException(string message)
        : base(message)
    {
    }

    public CLIException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
