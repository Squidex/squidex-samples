// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace Squidex.ClientLibrary.Utils
{
    /// <summary>
    /// Represent an actor that make actions.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        public string Type { get; set; }
    }
}
