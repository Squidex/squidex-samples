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

#pragma warning disable CS0612 // Type or member is obsolete
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

            await session.Client.Backups.PostBackupAsync();

            log.WriteLine("Backup started, waiting for completion...");

#pragma warning disable CS0618 // Type or member is obsolete
            BackupJobDto? foundBackup = null;
#pragma warning restore CS0618 // Type or member is obsolete
            using (var tcs = new CancellationTokenSource(TimeSpan.FromMinutes(arguments.Timeout)))
            {
                var backupStarted = DateTimeOffset.UtcNow.AddMinutes(-5);
                var backupInterval = TimeSpan.FromSeconds(arguments.Interval);

                while (!tcs.Token.IsCancellationRequested)
                {
                    var backups = await session.Client.Backups.GetBackupsAsync(tcs.Token);
                    var backup = backups.Items.Find(x => x.Started >= backupStarted);

                    if (backup?.Stopped != null)
                    {
                        foundBackup = backup;
                        break;
                    }

                    await Task.Delay(backupInterval, tcs.Token);
                }
            }

            if (foundBackup == null)
            {
                log.WriteLine("Failed to receive the backup in time.");
            }
            else if (foundBackup.Status == JobStatus.Completed)
            {
                log.WriteLine("Backup completed. Downloading...");

                var mode = arguments.Force ? FileMode.Create : FileMode.CreateNew;

                await using (var fs = new FileStream(arguments.File, mode))
                {
                    using var download = await session.Client.Backups.GetBackupContentAsync(foundBackup.Id);
                    await download.Stream.CopyToAsync(fs);
                }

                if (arguments.DeleteAfterDownload)
                {
                    log.WriteLine("Removing backup from app");

                    await session.Client.Backups.DeleteBackupAsync(foundBackup.Id);
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

            [Option("interval", Description = "The query interval to test if the backup is ready in seconds.")]
            public int Interval { get; set; } = 30;

            [Option("deleteAfterDownload", Description = "Defines if the created backup shall be deleted from app after the backup task is completed.")]
            public bool DeleteAfterDownload { get; set; }

            [Option("force", 'f', Description = "Overwrites the file if it already exists.")]
            public bool Force { get; set; }

            public sealed class Validator : AbstractValidator<CreateArguments>
            {
                public Validator()
                {
                    RuleFor(x => x.File).NotEmpty();
                    RuleFor(x => x.Interval).InclusiveBetween(1, 120);
                }
            }
        }
    }
}
