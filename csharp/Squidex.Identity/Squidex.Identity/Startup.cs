// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.ClientLibrary;
using Squidex.Identity.Model;
using Squidex.Identity.Model.Authentication;
using Squidex.Identity.Services;
using Squidex.Identity.Stores.MongoDb;

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

            services.AddMemoryCache();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddAuthentication()
                .AddCookie()
                .AddFacebook()
                .AddGoogle()
                .AddMicrosoftAccount()
                .AddTwitter();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/accessdenied";
            });

            services.AddIdentity<UserEntity, RoleEntity>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.UserInteraction.LoginUrl = "/login";
                    options.UserInteraction.LogoutUrl = "/logout";
                    options.UserInteraction.ErrorUrl = "/error";
                    options.UserInteraction.ConsentUrl = "/consent";
                })
                .AddAppAuthRedirectUriValidator()
                .AddAspNetIdentity<UserEntity>()
                .AddClientConfigurationValidator<DefaultClientConfigurationValidator>()
                .AddClientStore<ClientStore>()
                .AddDeveloperSigningCredential()
                .AddInMemoryCaching()
                .AddResourceStore<ResourceStore>();

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

            services.AddAuthenticationConfigurator<FacebookOptions, FacebookHandler>(
                AuthenticationSchemeType.Facebook, Factories.OAuth<FacebookOptions>);

            services.AddAuthenticationConfigurator<GoogleOptions, GoogleHandler>(
                AuthenticationSchemeType.Google, Factories.OAuth<GoogleOptions>);

            services.AddAuthenticationConfigurator<MicrosoftAccountOptions, MicrosoftAccountHandler>(
                AuthenticationSchemeType.Microsoft, Factories.OAuth<MicrosoftAccountOptions>);

            services.AddAuthenticationConfigurator<TwitterOptions, TwitterHandler>(
                AuthenticationSchemeType.Twitter, Factories.Twitter);

            services.AddSingleton<IAuthenticationSchemeProvider,
                SquidexAuthenticationSchemeProvider>();

            services.AddSingleton<IAuthenticationSchemeStore,
                AuthenticationSchemeStore>();

            services.AddSingleton<ISettingsProvider,
                SettingsProvider>();

            services.AddSingleton<IEmailSender,
                EmailSender>();

            var storeType = Configuration.GetValue<string>("store:type");

            if (string.Equals(storeType, "MongoDB", StringComparison.OrdinalIgnoreCase))
            {
                services.AddMongoDB(Configuration);
            }
            else
            {
                throw new ApplicationException("You have to define the store with 'store:type'. Allowed values: MongoDB");
            }
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
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
