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

        public static async Task DoSafeAsync(this ILogger log, string process, Func<Task> action)
        {
            try
            {
                log.StepStart(process);

                await action();

                log.StepSuccess();
            }
            catch (SquidexManagementException<ErrorDto> ex)
            {
                if (ex.StatusCode.Equals(400))
                {
                    log.StepSkipped(ex.Result.Message);
                }
                else
                {
                    log.StepFailed(ex.Result.Message);
                    throw;
                }
            }
            catch (SquidexManagementException ex)
            {
                if (ex.StatusCode.Equals(400))
                {
                    log.StepSkipped(ex.Message);
                }
                else
                {
                    log.StepFailed(ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                log.StepFailed(ex);
                throw;
            }
        }
    }
}
