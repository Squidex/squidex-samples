// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandDotNet;
using CommandDotNet.Attributes;
using FluentValidation;
using FluentValidation.Attributes;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary.Management;

namespace Squidex.CLI.Commands
{
    public sealed partial class App
    {
        [ApplicationMetadata(Name = "backup", Description = "Manage backups.")]
        [SubCommand]
        public sealed class Backup
        {
            [InjectProperty]
            public IConfigurationService Configuration { get; set; }

            [ApplicationMetadata(Name = "create", Description = "Create and download an backup.")]
            public async Task Create(CreateArguments arguments)
            {
                var (app, service) = Configuration.GetClient();

                var backupStarted = DateTime.UtcNow.AddMinutes(-5);
                var backupsClient = service.CreateBackupsClient();

                await backupsClient.PostBackupAsync(app);

                Console.WriteLine("Backup started, waiting for completion...");

                BackupJobDto foundBackup = null;

                using (var tcs = new CancellationTokenSource(TimeSpan.FromMinutes(arguments.Timeout)))
                {
                    while (!tcs.Token.IsCancellationRequested)
                    {
                        var backups = await backupsClient.GetBackupsAsync(app);
                        var backup = backups.Items.FirstOrDefault(x => x.Started >= backupStarted);

                        if (backup != null && backup.Stopped.HasValue)
                        {
                            foundBackup = backup;
                            break;
                        }
                    }

                    await Task.Delay(5000);
                }

                if (foundBackup == null)
                {
                    Console.WriteLine("Failed to receive the backup in time.");
                }
                else if (foundBackup.Status == JobStatus.Completed)
                {
                    Console.WriteLine("Backup completed. Downloading...");

                    using (var fs = new FileStream(arguments.File, FileMode.CreateNew))
                    {
                        var download = await backupsClient.GetBackupContentAsync(app, foundBackup.Id.ToString());

                        await download.Stream.CopyToAsync(fs);
                    }

                    Console.WriteLine("Backup completed. Download completed");

                    if (arguments.DeleteAfterDownload)
                    {
                        Console.WriteLine("Removing backup from app...");
                        await backupsClient.DeleteBackupAsync(app, foundBackup.Id.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Failed to make the backup, check the logs for details.");
                }
            }

            [Validator(typeof(Validator))]
            public sealed class CreateArguments : IArgumentModel
            {
                [Option(LongName = "timeout", Description = "The timeout to wait for the backup in minutes.")]
                public int Timeout { get; set; } = 30;

                [Option(LongName = "deleteAfterDownload", Description = "Defines wether the created backup shall be deleted from app after the backup task is completed")]
                public bool DeleteAfterDownload { get; set; }

                [Argument(Name = "file", Description = "The target file.")]
                public string File { get; set; }

                public sealed class Validator : AbstractValidator<CreateArguments>
                {
                    public Validator()
                    {
                        RuleFor(x => x.File).NotEmpty();
                    }
                }
            }
        }
    }
}
