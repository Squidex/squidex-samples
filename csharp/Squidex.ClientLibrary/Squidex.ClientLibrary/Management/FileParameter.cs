// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using HeyRed.Mime;
using Squidex.Assets;
using Squidex.ClientLibrary.Utils;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Squidex.ClientLibrary.Management;

public partial class FileParameter
{
    public static FileParameter FromFile(FileInfo file)
    {
        Guard.NotNull(file, nameof(file));

        return new FileParameter(file.OpenRead(), file.Name, MimeTypesMap.GetMimeType(file.Name));
    }

    public static FileParameter FromPath(string path)
    {
        Guard.NotNull(path, nameof(path));

        return FromFile(new FileInfo(path));
    }

    internal UploadFile ToUploadFile()
    {
        return new UploadFile(Data, FileName, ContentType);
    }
}
