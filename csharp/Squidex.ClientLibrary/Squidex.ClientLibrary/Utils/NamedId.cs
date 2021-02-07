﻿// ==========================================================================
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
    /// Id with a name.
    /// </summary>
    public class NamedId
    {
        /// <summary>
        /// Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
    }
}
