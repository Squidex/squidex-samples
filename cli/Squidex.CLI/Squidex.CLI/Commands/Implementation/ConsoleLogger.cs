// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.CLI.Commands.Implementation
{
    public class ConsoleLogger : ILogger
    {
        private const int MaxActionLength = 40;

        private sealed class ConsoleLine : ILogLine
        {
            private readonly int consoleTop;

            public ConsoleLine()
            {
                consoleTop = Console.CursorTop;
            }

            public void Dispose()
            {
            }

            public void WriteLine(string message, params object[] args)
            {
                Console.WriteLine(message, args);
                Console.SetCursorPosition(0, consoleTop);
            }
        }

        public void StepStart(string message)
        {
            if (message.Length > MaxActionLength - 3)
            {
                var length = MaxActionLength - 3;

                message = message[..length];
            }

            message += "...";
            message = message.PadRight(MaxActionLength);

            Console.Write(message);
        }

        public void StepSuccess(string details = null)
        {
            if (!string.IsNullOrWhiteSpace(details))
            {
                Console.WriteLine($"succeeded ({details}).");
            }
            else
            {
                Console.WriteLine("succeeded.");
            }
        }

        public void StepSkipped(string reason)
        {
            Console.WriteLine($"skipped: {reason.TrimEnd('.')}.");
        }

        public void StepFailed(string reason)
        {
            Console.WriteLine($"failed: {reason.TrimEnd('.')}.");
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public ILogLine WriteSameLine()
        {
            return new ConsoleLine();
        }
    }
}
