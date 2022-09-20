using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelfIdent;

namespace SelfIdentExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Maybe Move into Lib
            services.AddSession();

            ///////////////////////////////////
            ///////////////////////////////////
            string connectionString = Configuration.GetValue<string>("ConnectionString");

            var emailOptions = new SelfIdent.Options.EmailValidationOptions(SelfIdent.Options.EmailValidationOptions.EmailValidationPresets.Standard);
            var passwordOptions = new SelfIdent.Options.PasswordValidationOptions(SelfIdent.Options.PasswordValidationOptions.PasswordValidationPresets.Safe);
            var usernameOptions = new SelfIdent.Options.UsernameValidationOptions()
            {
                MaxLength = 20,
                MinLength = 1,
                MustBeUnique = true,
                UseCustomValidationFunction = false
            };

            var roleOptions = new SelfIdent.Options.RoleOptions()
            {
                Roles = new System.Collections.Generic.List<SelfIdent.Roles.Role>()
                {
                    new SelfIdent.Roles.Role(1, "Standard"),
                    new SelfIdent.Roles.Role(2, "Supervisor"),
                    new SelfIdent.Roles.Role(3, "Admin"),
                    new SelfIdent.Roles.Role(4, "Development"),
                    new SelfIdent.Roles.Role(5, "HR"),
                    new SelfIdent.Roles.Role(6, "Integration")
                },

                DefaultRoles = new System.Collections.Generic.List<SelfIdent.Roles.Role>()
                {
                    new SelfIdent.Roles.Role(1, "Standard"),
                    new SelfIdent.Roles.Role(4, "Development")
                }
            };

            var validationOptions = new SelfIdent.Options.ValidationOptions()
            {
                EmailValidationOptions = emailOptions,
                PasswordValidationOptions = passwordOptions,
                UsernameValidationOptions = usernameOptions,
                ValidateEmailOnRegister = false,
                ValidatePasswordOnRegister = false,
                ValidateUsernameOnRegister = false
            };

            var cryptographyOptions = new SelfIdent.Options.Hashing.PBKDFHashingOptions()
            {
                HashByteLength = 32,
                SaltByteLength = 32,
                HashingFunction = Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256,
                Iterations = 10000
            };

            // TODO: Check redundency between SecurityContextOptions and AuthenticationServiceOptions
            var contextOptions = new SelfIdent.Options.SecurityContextOptions("AuthenticationSchemaName", SelfIdent.Options.SecurityContextOptions.SecurityContextAuthenticationTypes.CookieAndToken)
            {
                LoginPath = "/Account/Login",
                LogoutPath = "/Account/Login",
                TokenLifetime = TimeSpan.FromDays(365),
                TokenSecretKey = Configuration.GetValue<string>("SecretKey")
            };


            var authenticationOptions = new SelfIdent.Options.AuthenticationServiceOptions(SelfIdent.Options.AuthenticationServiceOptions.SecurityContextTypes.Cookie, "AuthenticationSchemaName")
            {
                LoginPath = "/Account/Login",
                LogoutPath = "/Account/Login"
            };

            var authorizationOptions = new SelfIdent.Options.AuthorizationServiceOptions();

            var selfIdentOptions = new SelfIdent.Options.SelfIdentOptions(SelfIdent.Enums.DatabaseTypes.MySql, connectionString, "NET_IDENTITY_TEST")
            {
                LockoutActive = true,
                LockoutTryCount = 3,
                GenerateCustomDataTable = true,
                LockAccountOnRegistration = false,
                LockKeyLength = 32,
                PasswordHashOptions = cryptographyOptions,
                ValidationOptions = validationOptions,
                HashFunctionType = SelfIdent.Enums.HashFunctionTypes.PBKDF,
                MultiFactorAuthenticationActive = true,
                MFAOptions = new SelfIdent.Options.MFAOptions() { CheckSessionKey = true, KeyLength = 8, KeyTimeout = new System.TimeSpan(0, 10, 0) },
                RoleOptions = roleOptions,
                SecurityContextOptions = contextOptions,
                InvalidationStrictness = SelfIdent.Enums.SecurityContextInvalidationStrictness.PasswordChange
            };

            var selfIdent = new SelfIdent.SelfIdent(selfIdentOptions);

            services.AddSingleton<SelfIdent.SelfIdent>(s => selfIdent);

            selfIdent.ConfigureAuthenticationServices(services, authenticationOptions);
            selfIdent.ConfigureAuthorizationServices(services, authorizationOptions);

            ///////////////////////////////////
            ///////////////////////////////////

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            ///////////////////////////////////
            ///////////////////////////////////
            //Inject Middleware
            app.UseSession();

            // Inject custom Authentication-Middleware for token authentication
            app.UseMiddleware<SelfIdent.Token.SelfIdentTokenMiddleware>();
            //app.UseAuthentication();

            app.UseAuthorization();

            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };

            app.UseCookiePolicy(cookiePolicyOptions);

            ///////////////////////////////////
            //////////////////////////////////////

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });



        }
    }
}
