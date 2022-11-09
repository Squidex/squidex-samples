// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Integration.Localize.Controllers;

public partial class DefaultController : ControllerBase
{
    public override async Task<EnvResponse> Env(
        CancellationToken cancellationToken = default)
    {
        var clientManager = BuildClientManager();

        var response = new EnvResponse
        {
            Locales = new List<Locale>(),
            CacheItemStructure = new CacheItemStructure
            {
                [MetaFields.SchemaName] = "Schema",
                [MetaFields.ContentId] = "Content Id",
                [MetaFields.DateCreated] = "Creation Date",
                [MetaFields.DateUpdated] = "Update"
            }
        };

        var languages =
            await clientManager.CreateAppsClient().GetLanguagesAsync(clientManager.App,
                cancellationToken);

        foreach (var language in languages.Items)
        {
            response.Locales.Add(new Locale { Code = language.Iso2Code, Name = language.EnglishName });

            if (language.IsMaster)
            {
                response.DefaultLocale = language.Iso2Code;
            }
        }

        return response;
    }
}
