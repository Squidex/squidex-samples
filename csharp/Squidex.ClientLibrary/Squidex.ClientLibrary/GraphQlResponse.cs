// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary
{
    public sealed class GraphQlResponse<TData>
    {
        public TData Data { get; set; }

        public GraphQlError[] Errors { get; set; }
    }
}
