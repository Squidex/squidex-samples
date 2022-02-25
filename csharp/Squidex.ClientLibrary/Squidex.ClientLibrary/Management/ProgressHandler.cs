// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Assets;

namespace Squidex.ClientLibrary.Management
{
    /// <summary>
    /// Base class for all upload events.
    /// </summary>
    public abstract class AssetUploadEvent
    {
        /// <summary>
        /// The file id that can be used to resume an upload.
        /// </summary>
        public string FileId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetUploadEvent"/> class.
        /// </summary>
        /// <param name="fileId">The file id that can be used to resume an upload.</param>
        protected AssetUploadEvent(string fileId)
        {
            FileId = fileId;
        }
    }

    /// <summary>
    /// Contains information about a success upload process.
    /// </summary>
    public sealed class AssetUploadProgressEvent : AssetUploadEvent
    {
        /// <summary>
        /// The progress from 1 to 100.
        /// </summary>
        public int Progress { get; }

        /// <summary>
        /// The number of written bytes.
        /// </summary>
        public long BytesWritten { get; }

        /// <summary>
        /// The number of total bytes (length or file or asset).
        /// </summary>
        public long BytesTotal { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetUploadProgressEvent"/> class.
        /// </summary>
        /// <param name="fileId">The file id that can be used to resume an upload.</param>
        /// <param name="progress">The progress from 1 to 100.</param>
        /// <param name="bytesWritten">The number of written bytes.</param>
        /// <param name="bytesTotal">The number of total bytes (length or file or asset).</param>
        public AssetUploadProgressEvent(string fileId, int progress, long bytesWritten, long bytesTotal)
            : base(fileId)
        {
            Progress = progress;
            BytesWritten = bytesTotal;
            BytesTotal = bytesWritten;
        }
    }

    /// <summary>
    /// Contains information about a success creation of an upload.
    /// </summary>
    public sealed class AssetUploadCreatedEvent : AssetUploadEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetUploadCreatedEvent"/> class.
        /// </summary>
        /// <param name="fileId">The file id that can be used to resume an upload.</param>
        public AssetUploadCreatedEvent(string fileId)
            : base(fileId)
        {
        }
    }

    /// <summary>
    /// Contains information about a success upload process.
    /// </summary>
    public sealed class AssetUploadCompletedEvent : AssetUploadEvent
    {
        /// <summary>
        /// The created or updated asset.
        /// </summary>
        public AssetDto Asset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetUploadCompletedEvent"/> class with the asset.
        /// </summary>
        /// <param name="fileId">The file id that can be used to resume an upload.</param>
        /// <param name="asset">The created or updated asset.</param>
        public AssetUploadCompletedEvent(string fileId, AssetDto asset)
            : base(fileId)
        {
            Asset = asset;
        }
    }

    /// <summary>
    /// Contains information about a failed upload process.
    /// </summary>
    public sealed class AssetUploadExceptionEvent : AssetUploadEvent
    {
        /// <summary>
        /// The exception.
        /// </summary>
        public SquidexManagementException Exception { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetUploadExceptionEvent"/> class with the thrown exception.
        /// </summary>
        /// <param name="fileId">The file id that can be used to resume an upload.</param>
        /// <param name="exception">The thrown exception.</param>
        public AssetUploadExceptionEvent(string fileId, SquidexManagementException exception)
            : base(fileId)
        {
            Exception = exception;
        }
    }

    /// <summary>
    /// Optional options when uploading an asset.
    /// </summary>
    public struct AssetUploadOptions
    {
        /// <summary>
        /// The optional custom asset id.
        /// </summary>
        /// <remarks>
        /// Only when creating assets.
        /// </remarks>
        public string? Id { get; set; }

        /// <summary>
        /// The optional parent folder id.
        /// </summary>
        /// <remarks>
        /// Only when creating assets.
        /// </remarks>
        public string? ParentId { get; set; }

        /// <summary>
        /// True to duplicate the asset, event if the file has been uploaded.
        /// </summary>
        /// <remarks>
        /// Only when creating assets.
        /// </remarks>
        public bool? Duplicate { get; set; }

        /// <summary>
        /// The file id that can be used to resume an upload.
        /// </summary>
        public string? FileId { get; set; }

        /// <summary>
        /// The progress handler.
        /// </summary>
        public IAssetProgressHandler? ProgressHandler { get; set; }

#pragma warning disable MA0102 // Make member readonly
        internal UploadOptions ToOptions(FileParameter file, AssetsClient client)
#pragma warning restore MA0102 // Make member readonly
        {
            var result = new UploadOptions
            {
                FileId = FileId
            };

            var metadata = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                metadata[nameof(Id)] = Id!;
            }

            if (!string.IsNullOrWhiteSpace(ParentId))
            {
                metadata[nameof(ParentId)] = ParentId!;
            }

            if (Duplicate != null)
            {
                metadata[nameof(Duplicate)] = Duplicate.Value.ToString();
            }

            if (metadata.Count > 0)
            {
                result.Metadata = metadata;
            }

            if (ProgressHandler != null)
            {
                result.ProgressHandler = new ProgressHandlerAdapter(ProgressHandler, client);
            }

            return result;
        }
    }

    /// <summary>
    /// Tracks the progress of an upload.
    /// </summary>
    public interface IAssetProgressHandler
    {
        /// <summary>
        /// Invoked when the upload progress has been changed by at least one percent.
        /// </summary>
        /// <param name="event">The event containing all the data.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        Task OnProgressAsync(AssetUploadProgressEvent @event,
            CancellationToken ct);

        /// <summary>
        /// Invoked when the upload progress has been started with a new file id.
        /// </summary>
        /// <param name="event">The event containing all the data.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        Task OnCreatedAsync(AssetUploadCreatedEvent @event,
            CancellationToken ct);

        /// <summary>
        /// Invoked when asset has been uploaded successfully.
        /// </summary>
        /// <param name="event">The event containing all the data.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        Task OnCompletedAsync(AssetUploadCompletedEvent @event,
            CancellationToken ct);

        /// <summary>
        /// Invoked when an exception has occurred during upload.
        /// </summary>
        /// <param name="event">The event containing all the data.</param>
        /// <param name="ct">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// The task for completion.
        /// </returns>
        Task OnFailedAsync(AssetUploadExceptionEvent @event,
            CancellationToken ct);
    }

    /// <summary>
    /// Provides a progress handler to call delegates.
    /// </summary>
    public sealed class AssetDelegatingProgressHandler : IAssetProgressHandler
    {
        internal static readonly AssetDelegatingProgressHandler Instance = new AssetDelegatingProgressHandler();

        /// <summary>
        /// Gets or sets a delegate that is invoked when the upload progress has been changed by at least one percent.
        /// </summary>
        public Func<AssetUploadProgressEvent, CancellationToken, Task>? OnProgressAsync { get; set; }

        /// <summary>
        /// Gets or sets a delegate that is when the upload progress has been started with a new file id.
        /// </summary>
        public Func<AssetUploadCreatedEvent, CancellationToken, Task>? OnCreatedAsync { get; set; }

        /// <summary>
        /// Gets or sets a delegate that is when when asset has been uploaded successfully.
        /// </summary>
        public Func<AssetUploadCompletedEvent, CancellationToken, Task>? OnCompletedAsync { get; set; }

        /// <summary>
        /// Gets or sets a delegate that is invoked when an exception has occurred during upload.
        /// </summary>
        public Func<AssetUploadExceptionEvent, CancellationToken, Task>? OnFailedAsync { get; set; }

        async Task IAssetProgressHandler.OnProgressAsync(AssetUploadProgressEvent @event,
            CancellationToken ct)
        {
            var handler = OnProgressAsync;

            if (handler != null)
            {
                await handler(@event, ct);
            }
        }

        async Task IAssetProgressHandler.OnCreatedAsync(AssetUploadCreatedEvent @event,
            CancellationToken ct)
        {
            var handler = OnCreatedAsync;

            if (handler != null)
            {
                await handler(@event, ct);
            }
        }

        async Task IAssetProgressHandler.OnCompletedAsync(AssetUploadCompletedEvent @event,
            CancellationToken ct)
        {
            var handler = OnCompletedAsync;

            if (handler != null)
            {
                await handler(@event, ct);
            }
        }

        async Task IAssetProgressHandler.OnFailedAsync(AssetUploadExceptionEvent @event,
            CancellationToken ct)
        {
            var handler = OnFailedAsync;

            if (handler != null)
            {
                await handler(@event, ct);
            }
        }
    }
}
