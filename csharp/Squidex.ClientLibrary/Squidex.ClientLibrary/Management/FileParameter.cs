// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using HeyRed.Mime;
using Squidex.Assets;
using Squidex.ClientLibrary.Utils;

namespace Squidex.ClientLibrary.Management;

/// <summary>
/// Holds information about a file.
/// </summary>
public partial class FileParameter
{
    /// <summary>
    /// Overrides the content length.
    /// </summary>
    public long? ContentLength { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public FileParameter(Stream data, string fileName, string contentType, int? contentLength)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        Data = data;
        FileName = fileName;
        ContentType = contentType;
        ContentLength = contentLength;
    }

    /// <summary>
    /// Creates a new file parameter from a file info object.
    /// </summary>
    /// <param name="file">The file info. Cannot be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
    /// <returns>The created file parameter.</returns>
    public static FileParameter FromFile(FileInfo file)
    {
        Guard.NotNull(file, nameof(file));

        return new FileParameter(file.OpenRead(), file.Name, MimeTypesMap.GetMimeType(file.Name));
    }

    /// <summary>
    /// Creates a new file parameter from a file path.
    /// </summary>
    /// <param name="path">The file path. Cannot be null.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <returns>The created file parameter.</returns>
    public static FileParameter FromPath(string path)
    {
        Guard.NotNull(path, nameof(path));

        return FromFile(new FileInfo(path));
    }

    internal UploadFile ToUploadFile()
    {
        return new UploadFile(Data, FileName, ContentType, ContentLength ?? Data.Length);
    }
}
