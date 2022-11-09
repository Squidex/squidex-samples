// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.Options;

namespace Squidex.ClientLibrary.ServiceExtensions;

internal sealed class OptionsValidator : IValidateOptions<SquidexServiceOptions>
{
    public ValidateOptionsResult Validate(string name, SquidexServiceOptions options)
    {
        try
        {
            options.CheckAndFreeze();

            return ValidateOptionsResult.Success;
        }
        catch (Exception ex)
        {
            return ValidateOptionsResult.Fail(ex.Message);
        }
    }
}
