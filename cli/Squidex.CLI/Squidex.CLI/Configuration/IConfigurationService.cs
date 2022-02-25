// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.CLI.Commands.Implementation;

namespace Squidex.CLI.Configuration
{
    public interface IConfigurationService
    {
        Configuration GetConfiguration();

        void Upsert(string entry, ConfiguredApp appConfig);

        void Reset();

        void Remove(string entry);

        void UseApp(string entry);

        ISession StartSession(string app, bool emulate = false);
    }
}
