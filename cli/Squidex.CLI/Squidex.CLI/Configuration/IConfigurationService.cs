// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.ClientLibrary;

namespace Squidex.CLI.Configuration
{
    public interface IConfigurationService
    {
        Configuration GetConfiguration();

        void Upsert(string entry, ConfiguredApp appConfig);

        void Remove(string entry);

        void UseApp(string entry);

        SquidexClientManager GetClient();
    }
}
