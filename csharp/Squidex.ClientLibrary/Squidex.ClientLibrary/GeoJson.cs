// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Squidex.ClientLibrary;

/// <summary>
/// GeoJSON helper.
/// </summary>
public static class GeoJson
{
    /// <summary>
    /// Creates a new geo-json point.
    /// </summary>
    /// <param name="longitude">The longitude.</param>
    /// <param name="latitude">The latitude.</param>
    /// <param name="oldFormat">Uses the old format.</param>
    /// <returns>
    /// The geojson object.
    /// </returns>
    public static object Point(double longitude, double latitude, bool oldFormat = false)
    {
        if (oldFormat)
        {
            return new { longitude, latitude };
        }

        return new
        {
            type = "Point",
            coordinates = new[]
            {
                longitude,
                latitude,
            }
        };
    }
}
