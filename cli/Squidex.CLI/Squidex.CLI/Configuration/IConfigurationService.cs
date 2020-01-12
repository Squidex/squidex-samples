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

        void Reset();

        void Remove(string entry);

        void UseApp(string entry);

        void UseAppInSession(string entry);

        (string App, SquidexClientManager Client) Setup();
    }
}
