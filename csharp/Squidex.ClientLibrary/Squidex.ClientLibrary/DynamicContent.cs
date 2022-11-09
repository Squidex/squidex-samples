// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// A content with a dynamic data structure.
/// </summary>
/// <seealso cref="Content{DynamicData}" />
/// <remarks>
/// Use this type when you have a dynamic structure for your content or if you query content items across many schemas.
/// </remarks>
public sealed class DynamicContent : Content<DynamicData>
{
}
