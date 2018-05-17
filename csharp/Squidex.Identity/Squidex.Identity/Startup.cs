// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;
using Squidex.Identity.Model;
using Squidex.Identity.Services;

namespace Squidex.Identity
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SquidexOptions>(Configuration.GetSection("app"));
            services.Configure<SettingsData>(Configuration.GetSection("defaultSettings"));

            services.AddSingleton(c =>
                SquidexClientManager.FromOption(c.GetRequiredService<IOptions<SquidexOptions>>().Value));

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddAuthentication()
                .AddGoogle()
                .AddMicrosoftAccount()
                .AddCookie();

            services.AddIdentity<UserEntity, Role>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                })
                .AddAppAuthRedirectUriValidator()
                .AddClientConfigurationValidator<DefaultClientConfigurationValidator>()
                .AddClientStoreCache<ClientStore>()
                .AddDeveloperSigningCredential()
                .AddResourceStoreCache<ResourceStore>();

            services.AddMvc()
                .AddViewLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(AppResources));
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Manage");
                    options.Conventions.AuthorizePage("/Logout");
                });

            services.AddSingleton<AuthenticationSchemaProvider>();

            services.AddSingleton<IAuthenticationSchemeProvider>(
                c => c.GetRequiredService<AuthenticationSchemaProvider>());

            services.AddSingleton<IOptionsMonitor<GoogleOptions>>(
                c => c.GetRequiredService<AuthenticationSchemaProvider>());

            services.AddSingleton<IOptionsMonitor<MicrosoftAccountOptions>>(
                c => c.GetRequiredService<AuthenticationSchemaProvider>());

            services.AddSingleton<ISettingsProvider,
                SettingsProvider>();

            services.AddSingleton<IEmailSender,
                EmailSender>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
