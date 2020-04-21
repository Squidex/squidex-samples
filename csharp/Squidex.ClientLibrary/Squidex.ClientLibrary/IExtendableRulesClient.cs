// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public interface IExtendableRulesClient
    {
        Task<ExtendableRules> GetRulesAsync(CancellationToken ct = default);

        Task<ExtendableRuleDto> CreateRuleAsync(CreateExtendableRuleDto request, CancellationToken ct = default);

        Task<ExtendableRuleDto> UpdateRuleAsync(Guid id, UpdateExtendableRuleDto request, CancellationToken ct = default);

        Task<ExtendableRuleDto> EnableRuleAsync(Guid id, CancellationToken ct = default);

        Task<ExtendableRuleDto> DisableRuleAsync(Guid id, CancellationToken ct = default);

        Task DeleteRuleAsync(Guid id, CancellationToken ct = default);
    }
}
