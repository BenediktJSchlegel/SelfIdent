using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SelfIdent;

namespace SelfIdentExample.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            //app.UseSession();

            // Inject custom Authentication-Middleware for token authentication

            app.UseMiddleware<SelfIdent.Token.SelfIdentTokenMiddleware>();

            //app.UseAuthentication();
            //app.UseAuthorization();

            ///////////////////////////////////
            //////////////////////////////////////

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.AddSession();

            ///////////////////////////////////
            ///////////////////////////////////
            string connectionString = Configuration.GetValue<string>("ConnectionString");

            var emailOptions = new SelfIdent.Options.EmailValidationOptions(SelfIdent.Options.EmailValidationOptions.EmailValidationPresets.Standard);
            var passwordOptions = new SelfIdent.Options.PasswordValidationOptions(SelfIdent.Options.PasswordValidationOptions.PasswordValidationPresets.Safe);
            var usernameOptions = new SelfIdent.Options.UsernameValidationOptions()
            {
                MaxLength = 30,
                MinLength = 1,
                MustBeUnique = true,
                UseCustomValidationFunction = false
            };

            var roleOptions = new SelfIdent.Options.RoleOptions()
            {
                Roles = new System.Collections.Generic.List<SelfIdent.Roles.Role>()
                {
                    new SelfIdent.Roles.Role(1, "Standard"),
                    new SelfIdent.Roles.Role(2, "Admin"),
                    new SelfIdent.Roles.Role(3, "System"),
                },

                DefaultRoles = new System.Collections.Generic.List<SelfIdent.Roles.Role>()
                {
                    new SelfIdent.Roles.Role(1, "Standard"),
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

            var contextOptions = new SelfIdent.Options.SecurityContextOptions("DefaultAuthSchema", SelfIdent.Options.SecurityContextOptions.SecurityContextAuthenticationTypes.Token)
            {
                LoginPath = "/auth/register",
                LogoutPath = "/auth/register",
                TokenLifetime = TimeSpan.FromDays(365),
                TokenSecretKey = Configuration.GetValue<string>("SecretKey")
            };

            var authenticationOptions = new SelfIdent.Options.AuthenticationServiceOptions(SelfIdent.Options.AuthenticationServiceOptions.SecurityContextTypes.Token, "DefaultAuthSchema")
            {
                LoginPath = "/auth/register",
                LogoutPath = "/auth/register"
            };

            var authorizationOptions = new SelfIdent.Options.AuthorizationServiceOptions();

            var selfIdentOptions = new SelfIdent.Options.SelfIdentOptions(SelfIdent.Enums.DatabaseTypes.MySql, connectionString, "CHOREAPP_IDENTITY")
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

            services.AddSwaggerGen();
        }
    }
}
