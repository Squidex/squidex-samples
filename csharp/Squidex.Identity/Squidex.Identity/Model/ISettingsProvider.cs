// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;

namespace Squidex.Identity.Model
{
    public interface ISettingsProvider
    {
        Task<SquidexSettingsData> GetSettingsAsync();
    }
}
