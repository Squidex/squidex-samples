// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.Identity.Model.Authentication
{
    public interface IAuthenticationSchemeConfigurator
    {
        Type HandlerType { get; }
    }
}
