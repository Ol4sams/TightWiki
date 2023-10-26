using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TightWiki.Shared;

namespace TightWiki
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
            services.AddRazorPages();

            IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");
            Singletons.GoogleAuthenticationClientId = googleAuthNSection["ClientId"];

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie("Cookies", options =>
                {
                    options.Cookie.Name = "RememberMeTightWiki";
                    options.LoginPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;
                }).AddGoogle(options =>
                {
                    options.ClientId = Singletons.GoogleAuthenticationClientId;
                    options.ClientSecret = googleAuthNSection["ClientSecret"];

                });

            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var cookieOptions = new CookieOptions()
                {
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddHours(1),
                    IsEssential = true,
                    HttpOnly = false,
                    Secure = false,
                };

                if (context.Request.Query.TryGetValue(StaticDataEntryPoint.HostType_ParamName, out StringValues query))
                {                    
                    context.Response.Cookies.Append(StaticDataEntryPoint.HostType_ParamName, query[0]);
                }

                if (Environment.MachineName == StaticDataEntryPoint.DeveloperMachineName)
                {
                    context.Response.Cookies.Append(StaticDataEntryPoint.Build_ParamName, StaticDataEntryPoint.RCMConstructorBaseURI_Debug, cookieOptions);
                }
                else
                {
                    context.Response.Cookies.Append(StaticDataEntryPoint.Build_ParamName, StaticDataEntryPoint.RCMConstructorBaseURI_Release, cookieOptions);
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                /*
                Next, we�ll want to change how routes are registered. By default, there is a conventional route pattern.
                */
                /*
                endpoints.MapControllerRoute(name: "Default_Page_View",
                                pattern: "{pageNavigation}",
                                defaults: new { controller = "Page", action = "Display", pageNavigation = "Home" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Page}/{action=Display}/{pageNavigation?}");
                */

                endpoints.MapControllerRoute(
                    name: "File_Attachment",
                    pattern: "File/{action}/{pageNavigation}/{fileNavigation}",
                    defaults: new { controller = "File", action = "Binary", pageNavigation = "", fileNavigation = "", pageRevision = string.Empty }
                );

                endpoints.MapControllerRoute(
                    name: "File_Attachment_Revision",
                    pattern: "File/{action}/{pageNavigation}/{fileNavigation}/r/{pageRevision}",
                    defaults: new { controller = "File", action = "Binary", pageNavigation = "", fileNavigation = "", pageRevision = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Display",
                    pattern: "{pageNavigation}",
                    defaults: new { pageNavigation = "Home", controller = "Page", action = "Display", pageRevision = string.Empty }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Display_Revision",
                    pattern: "{pageNavigation}/r/{pageRevision}",
                    defaults: new { pageNavigation = "Home", controller = "Page", action = "Display", pageRevision = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Display_History",
                    pattern: "{pageNavigation}/History/{page}",
                    defaults: new { pageNavigation = "Home", controller = "Page", action = "History", pageRevision = string.Empty, page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Display_Refresh",
                    pattern: "{pageNavigation}/Refresh",
                    defaults: new { pageNavigation = "Home", controller = "Page", action = "Refresh" }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Comments",
                    pattern: "{pageNavigation}/Comments/{page}",
                    defaults: new { pageNavigation = "Home", controller = "Page", action = "Comments", pageRevision = string.Empty, page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Revert_History",
                    pattern: "{pageNavigation}/Revert/{pageRevision}",
                    defaults: new { pageNavigation = "Home", controller = "Page", action = "Revert" }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Search",
                    pattern: "Page/Search/{page}",
                    defaults: new { controller = "Page", action = "Search", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_Moderate",
                    pattern: "Admin/Moderate/{page}",
                    defaults: new { controller = "Admin", action = "Moderate", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Edit",
                    pattern: "Page/Edit/{pageNavigation}",
                    defaults: new { controller = "Page", action = "Edit", pageNavigation = string.Empty }
                );

                endpoints.MapControllerRoute(
                    name: "Page_Default",
                    pattern: "Page/{action}/{pageNavigation}",
                    defaults: new { controller = "Page", action = "Display", pageNavigation = "Home" }
                );

                endpoints.MapControllerRoute(
                    name: "Tag_Associations",
                    pattern: "Tag/Browse/{navigation}",
                    defaults: new { controller = "Tags", action = "Browse", navigation = "Home" }
                );

                endpoints.MapControllerRoute(
                    name: "Account_Login",
                    pattern: "Account/Login",
                    defaults: new { controller = "Account", action = "Login" }
                );

                endpoints.MapControllerRoute(
                    name: "Account_UserProfile",
                    pattern: "Account/Profile",
                    defaults: new { controller = "Account", action = "UserProfile" }
                );

                endpoints.MapControllerRoute(
                    name: "Account_Logout",
                    pattern: "Account/Logout",
                    defaults: new { controller = "Account", action = "Logout" }
                );

                endpoints.MapControllerRoute(
                    name: "Account_PublicProfile",
                    pattern: "Account/{userAccountName}/Profile",
                    defaults: new { controller = "Account", action = "PublicProfile" }
                );

                endpoints.MapControllerRoute(
                    name: "Account_Avatar",
                    pattern: "Account/{userAccountName}/Avatar",
                    defaults: new { controller = "Account", action = "Avatar" }
                );

                endpoints.MapControllerRoute(
                    name: "Account_Confirm",
                    pattern: "Account/{userAccountName}/Confirm/{verificationCode}",
                    defaults: new { controller = "Account", action = "Confirm" }
                );

                endpoints.MapControllerRoute(
                    name: "Account_Reset",
                    pattern: "Account/{userAccountName}/Reset/{verificationCode}",
                    defaults: new { controller = "Account", action = "Reset" }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_Account",
                    pattern: "Admin/Account/{navigation}",
                    defaults: new { controller = "Admin", action = "Account", navigation = "admin" }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_Emoji",
                    pattern: "Admin/Emoji/{name}",
                    defaults: new { controller = "Admin", action = "Emoji", name = "undefined" }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_DeleteEmoji",
                    pattern: "Admin/DeleteEmoji/{name}",
                    defaults: new { controller = "Admin", action = "DeleteEmoji", name = "undefined" }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_DeleteAccount",
                    pattern: "Admin/DeleteAccount/{navigation}",
                    defaults: new { controller = "Admin", action = "DeleteAccount", navigation = "undefined" }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_Account_Page",
                    pattern: "Admin/Role/{navigation}/{page}",
                    defaults: new { controller = "Admin", action = "Role", navigation = "member", page = "1" }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_MenuItem_Page",
                    pattern: "Admin/MenuItem/{id}",
                    defaults: new { controller = "Admin", action = "MenuItem", id = (int?)null }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_DeleteMenuItem",
                    pattern: "Admin/DeleteMenuItem/{id}",
                    defaults: new { controller = "Admin", action = "DeleteMenuItem" }
                );

                endpoints.MapControllerRoute(
                    name: "Admin_Generic",
                    pattern: "Admin/{action}/{page}",
                    defaults: new { controller = "Admin", action = "Config", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "DefaultOther",
                    pattern: "{controller}/{action}",
                    defaults: new { controller = "Page", action = "Login" }
                );

                //endpoints.MapControllers();
            });            

            Shared.ADO.Singletons.ConnectionString = ConfigurationExtensions.GetConnectionString(this.Configuration, "TightWikiADO");

            GlobalSettings.PreloadSingletons();
        }
    }
}
