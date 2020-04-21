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
using Squidex.CLI.Commands.Implementation.Sync;
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
                HandleException(ex, log);
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
                HandleException(ex, log);
            }
        }

        private static void HandleException(Exception ex, ILogger log)
        {
            switch (ex)
            {
                case SquidexManagementException<ErrorDto> ex1:
                    {
                        log.StepFailed(ex1.Result.ToString());
                        break;
                    }

                case SquidexManagementException ex2:
                    {
                        log.StepFailed(ex2.Message);
                        break;
                    }

                case CLIException ex4:
                    {
                        log.StepSkipped(ex4.Message);
                        break;
                    }

                case Exception ex3:
                    {
                        log.StepFailed(ex3);
                        throw ex3;
                    }
            }
        }
    }
}
