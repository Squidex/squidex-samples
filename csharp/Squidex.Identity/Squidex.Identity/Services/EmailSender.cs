// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Squidex.Identity.Model;

namespace Squidex.Identity.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ISettingsProvider settingsProvider;

        public EmailSender(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task SendEmailConfirmationAsync(string email, string link)
        {
            var settings = await settingsProvider.GetSettingsAsync();

            var smtpClient = new SmtpClient(settings.SmtpServer, 587)
            {
                Credentials = new NetworkCredential(
                    settings.SmtpUsername,
                    settings.SmtpPassword),
                EnableSsl = true
            };

            smtpClient.Send(settings.SmtpFrom, email, settings.EmailConfirmationSubject, settings.EmailConfirmationText);
        }

        public async Task SendResetPasswordAsync(string email, string link)
        {
            var settings = await settingsProvider.GetSettingsAsync();

            var smtpClient = new SmtpClient(settings.SmtpServer, 587)
            {
                Credentials = new NetworkCredential(
                    settings.SmtpUsername,
                    settings.SmtpPassword),
                EnableSsl = true
            };

            smtpClient.Send(settings.SmtpFrom, email, settings.EmailPasswordResetSubject, settings.EmailPasswordResetText);
        }
    }
}
