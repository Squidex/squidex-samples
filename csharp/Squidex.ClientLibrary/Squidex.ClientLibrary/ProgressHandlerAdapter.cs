// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Assets;

namespace Squidex.ClientLibrary;

internal sealed class ProgressHandlerAdapter : IProgressHandler
{
    private static readonly Dictionary<string, IEnumerable<string>> NullHeaders = [];
    private readonly IAssetProgressHandler inner;
    private readonly AssetsClient client;

    public ProgressHandlerAdapter(IAssetProgressHandler inner, AssetsClient client)
    {
        this.inner = inner;
        this.client = client;
    }

    public Task OnProgressAsync(UploadProgressEvent @event,
        CancellationToken ct)
    {
        var assetEvent = new AssetUploadProgressEvent(@event.FileId, @event.Progress, @event.BytesWritten, @event.BytesTotal);

        return inner.OnProgressAsync(assetEvent, ct);
    }

    public Task OnCreatedAsync(UploadCreatedEvent @event,
        CancellationToken ct)
    {
        var assetEvent = new AssetUploadCreatedEvent(@event.FileId);

        return inner.OnCreatedAsync(assetEvent, ct);
    }

    public async Task OnCompletedAsync(UploadCompletedEvent @event,
        CancellationToken ct)
    {
        var (asset, _) = await client.ReadObjectResponseCoreAsync<AssetDto>(@event.Response, NullHeaders, default);

        if (asset == null)
        {
            var exception = new SquidexException("Response was null which was not expected.", 0, string.Empty, NullHeaders, null);

            await inner.OnFailedAsync(new AssetUploadExceptionEvent(@event.FileId, exception), ct);
            return;
        }

        await inner.OnCompletedAsync(new AssetUploadCompletedEvent(@event.FileId, asset), ct);
    }

    public async Task OnFailedAsync(UploadExceptionEvent @event,
        CancellationToken ct)
    {
        var exception = await ParseExceptionAsync(@event.Response, @event.Exception, ct);

        await inner.OnFailedAsync(new AssetUploadExceptionEvent(@event.FileId, exception), ct);
    }

    private async Task<SquidexException> ParseExceptionAsync(HttpResponseMessage? response, Exception exception,
        CancellationToken ct)
    {
        if (response == null)
        {
            return new SquidexException("Failed with internal exception", 0, string.Empty, NullHeaders, exception);
        }

        var status = (int)response.StatusCode;
        switch (status)
        {
            case 400:
                var (errorDto, text) = await client.ReadObjectResponseCoreAsync<ErrorDto>(response, NullHeaders, ct);

                if (errorDto == null)
                {
                    return new SquidexException("Response was null which was not expected.", status, text, NullHeaders, exception);
                }

                return new SquidexException<ErrorDto>("Asset request not valid.", status, text, NullHeaders, errorDto, exception);
            case 413:
                (errorDto, text) = await client.ReadObjectResponseCoreAsync<ErrorDto>(response, NullHeaders, ct);

                if (errorDto == null)
                {
                    return new SquidexException("Response was null which was not expected.", status, text, NullHeaders, exception);
                }

                return new SquidexException<ErrorDto>("Asset exceeds the maximum upload size.", status, text, NullHeaders, errorDto, exception);
            case 500:
                (errorDto, text) = await client.ReadObjectResponseCoreAsync<ErrorDto>(response, NullHeaders, ct);

                if (errorDto == null)
                {
                    return new SquidexException("Response was null which was not expected.", status, text, NullHeaders, exception);
                }

                return new SquidexException<ErrorDto>("Operation failed.", status, text, NullHeaders, errorDto, exception);
            case 404:
                var responseText1 = await response.Content.ReadAsStringAsync();

                return new SquidexException("App not found.", status, responseText1, NullHeaders, exception);
            default:
                var responseText2 = await response.Content.ReadAsStringAsync();

                return new SquidexException($"Exception with unexpected status ({status}): {exception.Message}.", status, responseText2, NullHeaders, exception);
        }
    }
}
