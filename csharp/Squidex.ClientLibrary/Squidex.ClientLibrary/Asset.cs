// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;

namespace Squidex.ClientLibrary
{
    public class Asset
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public int FileSize { get; set; }

        public int FileVersion { get; set; }

        public bool IsImage { get; set; }

        public int PixelWidth { get; set; }

        public int PixelHeight { get; set; }

        public DateTimeOffset Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public string LastModifiedBy { get; set; }

        public int Version { get; set; }
    }
}
