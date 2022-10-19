// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.AspNetCore.Mvc;
using Squidex.ClientLibrary;

namespace Integration.Localize.Controllers
{
    public partial class DefaultController : ControllerBase
    {
        public override async Task<PublishResponse> Publish([FromBody] PublishRequest body,
            CancellationToken cancellationToken = default)
        {
            var clientManager = BuildClientManager();

            if (body.Items.Count == 0)
            {
                return new PublishResponse();
            }

            var contentClient = clientManager.CreateDynamicContentsClient(body.Items[0].Metadata[MetaFields.SchemaName]);

            var ordered = body.Items.OrderBy(x => x.GroupId);

            foreach (var batch in ordered.Batch(200))
            {
                var update = new BulkUpdate
                {
                    Jobs = new List<BulkUpdateJob>(),
                    DoNotScript = true,
                    DoNotValidate = false,
                    DoNotValidateWorkflow = false,
                    OptimizeValidation = true
                };

                foreach (var content in batch.GroupBy(x => x.GroupId))
                {
                    var contentId = content.Key;

                    var data = new Dictionary<string, IDictionary<string, string>>();

                    foreach (var field in content)
                    {
                        data[field.Metadata[MetaFields.ContentField]] = field.Translations;
                    }

                    var job = new BulkUpdateJob
                    {
                        Id = contentId,
                        // We only make a patch to not update the other fields.
                        Type = BulkUpdateType.Patch,
                        // The schema is needed to resolve the data.
                        Schema = content.First().Metadata[MetaFields.SchemaName],
                        Data = data,
                    };

                    update.Jobs.Add(job);
                }

                var response = await contentClient.BulkUpdateAsync(update, cancellationToken);

                var error = response.Find(x => x.Error != null)?.Error;

                if (error != null)
                {
                    throw new SquidexException(error.Message, 400, error);
                }
            }

            return new PublishResponse
            {
                Code = 200
            };
        }
    }
}
