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
            Console.Write((message + "...").PadRight(80));
        }

        public void StepSuccess()
        {
            Console.WriteLine("succeeded.");
        }

        public void StepSkipped(string reason)
        {
            Console.WriteLine($"skipped: {reason}");
        }

        public void StepFailed(Exception ex)
        {
            Console.WriteLine($"failed with {ex.Message}.");
        }

        public void WriteLine()
        {
            Console.WriteLine();
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
