// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation
{
    public static class LogExtensions
    {
        private static readonly SemaphoreSlim LockObject = new SemaphoreSlim(1);

        public static async Task DoVersionedAsync(this ILogger log, string process, long version, Func<Task<long>> action)
        {
            await LockObject.WaitAsync();
            try
            {
                log.StepStart(process);

                var newVersion = await action();

                if (version != newVersion)
                {
                    log.StepSuccess();
                }
                else
                {
                    log.StepSkipped("Nothing changed.");
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, log.StepFailed);
            }
            finally
            {
                LockObject.Release();
            }
        }

        public static async Task DoSafeAsync(this ILogger log, string process, Func<Task> action)
        {
            await LockObject.WaitAsync();
            try
            {
                log.StepStart(process);

                await action();

                log.StepSuccess();
            }
            catch (Exception ex)
            {
                HandleException(ex, log.StepFailed);
            }
            finally
            {
                LockObject.Release();
            }
        }

        public static void ProcessSkipped(this ILogger log, string process, string reason)
        {
            LockObject.Wait();
            try
            {
                log.StepStart(process);

                log.StepSkipped(reason);
            }
            finally
            {
                LockObject.Release();
            }
        }

        public static void ProcessCompleted(this ILogger log, string process)
        {
            LockObject.Wait();
            try
            {
                log.StepStart(process);

                log.StepSuccess();
            }
            finally
            {
                LockObject.Release();
            }
        }

        public static void ProcessFailed(this ILogger log, string process, Exception exception)
        {
            LockObject.Wait();
            try
            {
                log.StepStart(process);

                HandleException(exception, log.StepFailed);
            }
            finally
            {
                LockObject.Release();
            }
        }

        public static void HandleException(Exception ex, Action<string> error)
        {
            switch (ex)
            {
                case SquidexManagementException<ErrorDto> ex1:
                    {
                        error(ex1.Result.ToString());
                        break;
                    }

                case SquidexManagementException ex2:
                    {
                        error(ex2.Message);
                        break;
                    }

                case CLIException ex4:
                    {
                        error(ex4.Message);
                        break;
                    }

                case { } ex3:
                    {
                        error(ex3.ToString());
                        throw ex3;
                    }
            }
        }
    }
}
