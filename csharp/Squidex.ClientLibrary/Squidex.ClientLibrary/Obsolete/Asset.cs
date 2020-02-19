// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;

namespace Squidex.ClientLibrary
{
    [Obsolete("Use the management API for assets instead.")]
    public sealed class Asset : EntityBase
    {
        public string FileName { get; set; }

        public string MimeType { get; set; }

        public int FileSize { get; set; }

        public int FileVersion { get; set; }

        public bool IsImage { get; set; }

        public int PixelWidth { get; set; }

        public int PixelHeight { get; set; }

        public List<string> Tags { get; set; }
    }
}
