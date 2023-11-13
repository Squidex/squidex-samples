// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Squidex.ClientLibrary.Utils;

internal static class ListFallbackConverter
{
    public static readonly JsonSerializer DefaultSerializer = JsonSerializer.Create(new JsonSerializerSettings());

    public static readonly ListFallbackConverter<AppLanguagesDto, AppLanguageDto> Languages =
        new ListFallbackConverter<AppLanguagesDto, AppLanguageDto>(
            items => new AppLanguagesDto
            {
                Items = items
            });

    public static readonly ListFallbackConverter<ClientsDto, ClientDto> Clients =
        new ListFallbackConverter<ClientsDto, ClientDto>(
            items => new ClientsDto
            {
                Items = items
            });

    public static readonly ListFallbackConverter<SchemasDto, SchemaDto> Schemas =
        new ListFallbackConverter<SchemasDto, SchemaDto>(
            items => new SchemasDto
            {
                Items = items
            });

    public static readonly ListFallbackConverter<RulesDto, RuleDto> Rules =
        new ListFallbackConverter<RulesDto, RuleDto>(
            items => new RulesDto
            {
                Items = items
            });
}
