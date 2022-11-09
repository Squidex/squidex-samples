// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.CLI.Commands.Implementation.ImExport;

public interface IExportSettings
{
    string Schema { get; }

    string Filter { get; }

    string OrderBy { get; }

    string FullText { get; }

    bool Unpublished { get; }
}
