// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.ImExport
{
    public interface IImportSettings
    {
        string Schema { get; }

        bool Unpublished { get; }
    }
}
