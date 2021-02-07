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

        /// <summary>
        /// The path to the failed resolver.
        /// </summary>
        public string[] Path { get; set; }

        /// <summary>
        /// The error locations.
        /// </summary>
        public GraphQLErrorLocation[] Locations { get; set; }
    }

    /// <summary>
    /// The error location within the query.
    /// </summary>
    public sealed class GraphQLErrorLocation
    {
        /// <summary>
        /// The column in the query.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The row in the query.
        /// </summary>
        public int Row { get; set; }
    }
}
