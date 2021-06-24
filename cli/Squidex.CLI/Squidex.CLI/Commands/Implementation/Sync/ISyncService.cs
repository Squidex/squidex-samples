// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Squidex.CLI.Commands.Implementation.FileSystem;

namespace Squidex.CLI.Commands.Implementation.Sync
{
    public interface ISyncService
    {
        IFileSystem FileSystem { get; }

        T Convert<T>(object value);

        T Read<T>(IFile file, ILogger log);

        Task WriteWithSchemaAs<T>(FilePath path, object sample, string schema) where T : class
        {
            var file = FileSystem.GetFile(path);

            return WriteWithSchemaAs<T>(file, sample, schema);
        }

        Task WriteWithSchema<T>(FilePath path, T sample, string schema) where T : class
        {
            var file = FileSystem.GetFile(path);

            return WriteWithSchema(file, sample, schema);
        }

        Task WriteJsonSchemaAsync<T>(FilePath path)
        {
            var file = FileSystem.GetFile(new FilePath("__json").Combine(path));

            return WriteJsonSchemaAsync<T>(file);
        }

        Task WriteWithSchemaAs<T>(IFile file, object sample, string schema) where T : class;

        Task WriteWithSchema<T>(IFile file, T sample, string schema) where T : class;

        Task WriteJsonSchemaAsync<T>(IFile file);
    }
}
