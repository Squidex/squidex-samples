// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;
using Squidex.ClientLibrary.Management;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.EnrichedEvents
{
    /// <summary>
    /// Event on an asset.
    /// </summary>
    public sealed class EnrichedAssetEvent : EnrichedUserEventBase, IEnrichedEntityEvent
    {
        /// <summary>
        /// Type of the event.
        /// </summary>
        public EnrichedAssetEventType Type { get; set; }

        /// <summary>
        /// Asset's id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// When the asset has been created.
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// When the asset has been modified.
        /// </summary>
        public DateTimeOffset LastModified { get; set; }

        /// <summary>
        /// Who has created the asset.
        /// </summary>
        public Actor CreatedBy { get; set; }

        /// <summary>
        /// Who has modified the asset.
        /// </summary>
        public Actor LastModifiedBy { get; set; }

        /// <summary>
        /// Mime type of the asset.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// File name of the asset.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Version of the asset.
        /// </summary>
        public long FileVersion { get; set; }

        /// <summary>
        /// Size of the asset.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Width in pixel if the asset is an image.
        /// </summary>
        public int? PixelWidth { get; set; }

        /// <summary>
        /// Height in pixel if the asset is an image.
        /// </summary>
        public int? PixelHeight { get; set; }

        /// <summary>
        /// Type of the asset.
        /// </summary>
        public AssetType AssetType { get; set; }

        /// <summary>
        /// Is an image.
        /// </summary>
        [JsonIgnore]
        public bool IsImage
        {
            get { return AssetType == AssetType.Image; }
        }
    }
}
