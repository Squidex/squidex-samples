// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading;
using System.Threading.Tasks;

namespace Squidex.ClientLibrary
{
    public interface IExtendableRulesClient
    {
        Task<ExtendableRules> GetRulesAsync(CancellationToken ct = default);

        Task<ExtendableRuleDto> CreateRuleAsync(CreateExtendableRuleDto request, CancellationToken ct = default);

        Task<ExtendableRuleDto> UpdateRuleAsync(string id, UpdateExtendableRuleDto request, CancellationToken ct = default);

        Task<ExtendableRuleDto> EnableRuleAsync(string id, CancellationToken ct = default);

        Task<ExtendableRuleDto> DisableRuleAsync(string id, CancellationToken ct = default);

        Task DeleteRuleAsync(string id, CancellationToken ct = default);
    }
}
