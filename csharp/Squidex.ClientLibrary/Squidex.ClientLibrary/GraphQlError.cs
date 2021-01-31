// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    /// <summary>
    /// Represents a GraphQL error.
    /// </summary>
    [Serializable]
    public sealed class GraphQlError
    {
        /// <summary>
        /// Gets or sets the GraphQL error message.
        /// </summary>
        /// <value>
        /// The GraphQL error message.
        /// </value>
        public string Message { get; set; }
    }
}
