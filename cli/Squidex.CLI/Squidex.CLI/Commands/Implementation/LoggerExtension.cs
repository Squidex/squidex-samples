// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands.Implementation
{
    public static class LoggerExtension
    {
        public static async Task DoSafeAsync(this ILogger log, string process, Func<Task> action)
        {
            try
            {
                log.StepStart(process);

                await action();

                log.StepSuccess();
            }
            catch (SquidexManagementException ex)
            {
                if (ex.StatusCode.Equals(400))
                {
                    log.StepSkipped("already exists");
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
