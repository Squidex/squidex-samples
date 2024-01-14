// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using CommandDotNet;
using FluentValidation;
using Squidex.CLI.Commands.Implementation;
using Squidex.CLI.Configuration;
using Squidex.ClientLibrary;

#pragma warning disable MA0048 // File name must match type name

namespace Squidex.CLI.Commands;

public sealed partial class App
{
    [Command("backup", Description = "Manage backups.")]
    [Subcommand]
    public sealed class Backup(IConfigurationService configuration, ILogger log)
    {
        [Command("create", Description = "Create and download an backup.")]
        public async Task Create(CreateArguments arguments)
        {
            var session = configuration.StartSession(arguments.App);

            var backupStarted = DateTimeOffset.UtcNow.AddMinutes(-5);

            await session.Client.Backups.PostBackupAsync();

            log.WriteLine("Backup started, waiting for completion...");

            BackupJobDto? foundBackup = null;

            using (var tcs = new CancellationTokenSource(TimeSpan.FromMinutes(arguments.Timeout)))
            {
                while (!tcs.Token.IsCancellationRequested)
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    var backups = await session.Client.Backups.GetBackupsAsync(tcs.Token);
#pragma warning restore CS0612 // Type or member is obsolete
                    var backup = backups.Items.Find(x => x.Started >= backupStarted);

                    if (backup?.Stopped != null)
                    {
                        foundBackup = backup;
                        break;
                    }
                }

                await Task.Delay(5000, tcs.Token);
            }

            if (foundBackup == null)
            {
                log.WriteLine("Failed to receive the backup in time.");
            }
            else if (foundBackup.Status == JobStatus.Completed)
            {
                log.WriteLine("Backup completed. Downloading...");

                await using (var fs = new FileStream(arguments.File, FileMode.CreateNew))
                {
#pragma warning disable CS0612 // Type or member is obsolete
                    using (var download = await session.Client.Backups.GetBackupContentAsync(foundBackup.Id))
#pragma warning restore CS0612 // Type or member is obsolete
                    {
                        await download.Stream.CopyToAsync(fs);
                    }
                }

                if (arguments.DeleteAfterDownload)
                {
                    log.WriteLine("Removing backup from app");

#pragma warning disable CS0612 // Type or member is obsolete
                    await session.Client.Backups.DeleteBackupAsync(foundBackup.Id);
#pragma warning restore CS0612 // Type or member is obsolete
                }

                log.Completed("Backup Download completed.");
            }
            else
            {
                log.Completed("Backup failed, check the logs for details.");
            }
        }

        public sealed class CreateArguments : AppArguments
        {
            [Operand("file", Description = "The target file.")]
            public string File { get; set; }

            [Option("timeout", Description = "The timeout to wait for the backup in minutes.")]
            public int Timeout { get; set; } = 30;

            [Option("deleteAfterDownload", Description = "Defines if the created backup shall be deleted from app after the backup task is completed.")]
            public bool DeleteAfterDownload { get; set; }

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
