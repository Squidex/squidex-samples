// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Integration.Localize.Controllers
{
    public partial class DefaultController : ControllerBase
    {
        public override Task Anonymous(
            CancellationToken cancellationToken = default)
        {
            // Nothing to do, because it is just a health check.
            return Task.CompletedTask;
        }

        public override Task Health(
            CancellationToken cancellationToken = default)
        {
            // Nothing to do, because it is just a health check.
            return Task.CompletedTask;
        }
    }
}
