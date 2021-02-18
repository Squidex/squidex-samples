// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation
{
    public static class Extension
    {
        public static bool JsonEquals<T, TOther>(this T lhs, TOther rhsOther)
        {
            var rhsOtherJson = JsonConvert.SerializeObject(rhsOther);

            var rhs = JsonConvert.DeserializeObject<T>(rhsOtherJson);

            var lhsJson = JsonConvert.SerializeObject(lhs);
            var rhsJson = JsonConvert.SerializeObject(rhs);

            return lhsJson == rhsJson;
        }

        public static bool SetEquals(this IEnumerable<string> lhs, IEnumerable<string> rhs)
        {
            if (rhs == null)
            {
                return false;
            }

            return new HashSet<string>(lhs).SetEquals(rhs);
        }

        public static async Task DoVersionedAsync(this ILogger log, string process, long version, Func<Task<long>> action)
        {
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
        }

        public static async Task DoSafeAsync(this ILogger log, string process, Func<Task> action)
        {
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
        }

        public static void ProcessSkipped(this ILogger log, string process, string reason)
        {
            lock (log)
            {
                log.StepStart(process);
                log.StepSkipped(reason);
            }
        }

        public static void ProcessCompleted(this ILogger log, string process)
        {
            lock (log)
            {
                log.StepStart(process);
                log.StepSuccess();
            }
        }

        public static void ProcessFailed(this ILogger log, string process, Exception exception)
        {
            lock (log)
            {
                log.StepStart(process);

                HandleException(exception, log.StepFailed);
            }
        }

        public static async Task DoSafeLineAsync(this ILogger log, string process, Func<Task> action)
        {
            try
            {
                log.WriteLine("Start: {0}", process);

                await action();

                log.WriteLine("Done : {0}", process);
            }
            catch (Exception ex)
            {
                HandleException(ex, error => log.WriteLine("Error: {0}; {1}", process, error));
            }
            finally
            {
                log.WriteLine();
            }
        }

        private static void HandleException(Exception ex, Action<string> error)
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
