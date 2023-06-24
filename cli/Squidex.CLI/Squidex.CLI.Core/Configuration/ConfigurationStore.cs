// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.CLI.Commands.Implementation.Utils;

namespace Squidex.CLI.Configuration;

public sealed class ConfigurationStore : IConfigurationStore
{
    private readonly JsonSerializer jsonSerializer = new JsonSerializer();
    private DirectoryInfo workingDirectory;

    public DirectoryInfo WorkingDirectory
    {
        get
        {
            if (workingDirectory != null)
            {
                return workingDirectory;
            }

            var folderPath = Environment.GetEnvironmentVariable("SQCLI_FOLDER");

            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                if (Directory.Exists(folderPath))
                {
                    workingDirectory = new DirectoryInfo(folderPath);
                }
            }

            if (workingDirectory == null)
            {
                var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                if (Directory.Exists(userFolder))
                {
                    workingDirectory = Directory.CreateDirectory(Path.Combine(userFolder, ".sqcli"));
                }
            }

            workingDirectory ??= new DirectoryInfo(Directory.GetCurrentDirectory());

            return workingDirectory;
        }
    }

    (T? Value, DateTimeOffset Saved) IConfigurationStore.Get<T>(string key) where T : class
    {
        try
        {
            var file = WorkingDirectory.GetFile(key);

            if (!file.Exists)
            {
                return (null, default);
            }

            using var fileStream = file.OpenRead();
            using var fileReader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(fileReader);

            var result = jsonSerializer.Deserialize<T>(jsonReader);

            return (result, new DateTimeOffset(file.LastWriteTimeUtc));
        }
        catch
        {
            return (null, default);
        }
    }

    public void Set<T>(string key, T value) where T : class
    {
        var file = WorkingDirectory.GetFile(key);

        Directory.CreateDirectory(file.Directory!.FullName);

        using var fileStream = new FileStream(file.FullName, FileMode.Create);
        using var fileWriter = new StreamWriter(fileStream);
        using var jsonWriter = new JsonTextWriter(fileWriter);

        jsonSerializer.Serialize(jsonWriter, value);
    }
}
