// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Betalgo.Ranul.OpenAI;
using Betalgo.Ranul.OpenAI.Managers;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary;

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands.Implementation.AI;

public sealed class AIContentExecutor(ISession session, ILogger log)
{
    public async Task ExecuteAsync(GenerateRequest request, GeneratedContent content,
        CancellationToken ct)
    {
        await GenerateImagesAsync(request, content, ct);
        await GenerateSchemaAsync(request, content, ct);
        await GenerateContentAsync(request, content, ct);

        log.Completed("Generation completed.");
    }

    private async Task GenerateImagesAsync(GenerateRequest request, GeneratedContent generated,
        CancellationToken ct)
    {
        var targets = new List<GenerateImageTarget>();
        foreach (var content in generated.Contents)
        {
            foreach (var field in generated.Schema.Fields.Where(x => x.Type == SimplifiedFieldType.Image))
            {
                if (!content.TryGetValue(field.Name, out var fieldValue))
                {
                    continue;
                }

                if (field.IsLocalized)
                {
                    var asObject = (JObject)fieldValue;
                    foreach (var property in asObject.Properties())
                    {
                        var image = property.Value.ToObject<SimplifiedImage>()!;

                        targets.Add(new GenerateImageTarget(image, update =>
                        {
                            lock (asObject)
                            {
                                content[field.Name] = update;
                            }
                        }));
                    }
                }
                else
                {
                    var image = fieldValue.ToObject<SimplifiedImage>()!;

                    targets.Add(new GenerateImageTarget(image, update =>
                    {
                        lock (content)
                        {
                            content[field.Name] = update;
                        }
                    }));
                }
            }
        }

        if (!request.GenerateImages)
        {
            foreach (var target in targets)
            {
                target.Updater(new JArray());
            }

            return;
        }

        var client = new OpenAIService(new OpenAIOptions
        {
            ApiKey = request.OpenAIApiKey,
        });

        await Parallel.ForEachAsync(targets, ct, async (target, ct) =>
        {
            var response = await client.CreateImage(new ImageCreateRequest(target.Image.Description), ct);

            var error = response.Error;
            if (error != null)
            {
                throw new InvalidOperationException($"Failed to generate image. {error.FormatError(response.HttpStatusCode)}");
            }

            var url = response.Results.FirstOrDefault()?.Url;
            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException($"Failed to generate image. No result provided.");
            }

            var asset = await session.Client.Assets.PostAssetAsync(url: url, cancellationToken: ct);

            target.Updater(new JArray(asset.Id));
        });
    }

    private async Task GenerateSchemaAsync(GenerateRequest request, GeneratedContent generated,
        CancellationToken ct)
    {
        var schemaProcess = "Creating Schema";

        if (!request.NoSchema)
        {
            if (request.DeleteSchema)
            {
                try
                {
                    await session.Client.Schemas.DeleteSchemaAsync(generated.Schema.Name, cancellationToken: ct);
                }
                catch (SquidexException ex) when (ex.StatusCode == 400)
                {
                }
            }

            try
            {
                await session.Client.Schemas.PostSchemaAsync(
                    new CreateSchemaDto
                    {
                        Name = generated.Schema.Name,
                        IsPublished = true,
                        IsSingleton = false,
                    }, ct);
            }
            catch (SquidexException ex) when (ex.StatusCode == 400)
            {
            }

            await session.Client.Schemas.PutSchemaSyncAsync(generated.Schema.Name,
                new SynchronizeSchemaDto
                {
                    IsPublished = true,
                    Fields = generated.Schema.Fields.Select(x => x.ToField()).ToList(),
                }, ct);

            log.ProcessCompleted(schemaProcess);
        }
        else
        {
            log.ProcessSkipped(schemaProcess, "Disabled");
        }
    }

    private async Task GenerateContentAsync(GenerateRequest request, GeneratedContent generated,
        CancellationToken ct)
    {
        if (generated.Contents.Count <= 0)
        {
            return;
        }

        var contentsProcess = $"Creating {generated.Contents} content items";

        if (!request.NoContents)
        {
            await session.Client.DynamicContents(generated.Schema.Name)
                .BulkUpdateAsync(new BulkUpdate
                {
                    Jobs = generated.Contents.Select(
                        x => new BulkUpdateJob
                        {
                            Data =
                                x.ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp.Value.Type == JTokenType.Object ? (object)kvp : new { iv = kvp.Value }),
                        }).ToList(),
                }, ct);

            log.ProcessCompleted(contentsProcess);
        }
        else
        {
            log.ProcessSkipped(contentsProcess, "Disabled");
        }
    }

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it
    record GenerateImageTarget(SimplifiedImage Image, Action<JToken> Updater);
#pragma warning restore RECS0082 // Parameter has the same name as a member and hides it
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
