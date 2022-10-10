﻿using SharpWiki.Shared.Library;
using SharpWiki.Shared.Models;
using SharpWiki.Shared.Repository;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SharpWiki.Site.Controllers
{
    public class ControllerHelperBase : Controller
    {
        public StateContext context = new StateContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Configure();
        }

        public void Configure()
        {
            HydrateSecurityContext();

            var basicConfig = ConfigurationRepository.GetConfigurationEntryValuesByGroupName("Basic");
            var htmlConfig = ConfigurationRepository.GetConfigurationEntryValuesByGroupName("HTML Layout");

            ViewBag.Config = new ViewBagConfig
            {
                Context = context,
                HTMLHeader = htmlConfig.ValueAs<string>("Header"),
                HTMLFooter = htmlConfig.ValueAs<string>("Footer"),
                HTMLPreBody = htmlConfig.ValueAs<string>("Pre-Body"),
                HTMLPostBody = htmlConfig.ValueAs<string>("Post-Body"),
                BrandImageSmall = basicConfig.ValueAs<string>("Brand Image (Small)"),
                Name = basicConfig.ValueAs<string>("Name"),
                Title = basicConfig.ValueAs<string>("Name"), //Default the title to the name. This will be replaced when the page is found and loaded.
                FooterBlurb = basicConfig.ValueAs<string>("FooterBlurb"),
                Copyright = basicConfig.ValueAs<string>("Copyright"),
                PageNavigation = RouteValue("pageNavigation"),
                PageRevision = RouteValue("pageRevision"),
                AllowGuestsToViewHistory = basicConfig.ValueAs<bool>("Allow Guests to View History"),
                MenuItems = MenuItemRepository.GetAllMenuItems()
            };
        }

        public void HydrateSecurityContext()
        {
            context.IsAuthenticated = false;

            if (User.Identity.IsAuthenticated)
            {
                var cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var decodedTicket = FormsAuthentication.Decrypt(cookie.Value);
                var roles = decodedTicket.UserData.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var principal = new GenericPrincipal(User.Identity, roles);

                context.IsAuthenticated = User.Identity.IsAuthenticated;
                if (context.IsAuthenticated)
                {
                    context.Roles = roles.ToList();
                    context.User = UserRepository.GetUserById(int.Parse(principal.Identity.Name));
                }
            }
            else if (ConfigurationRepository.Get("Membership", "Allow Guest", false))
            {
                PerformGuestLogin();
            }
        }

        public bool PerformGuestLogin()
        {
            var guestAccount = ConfigurationRepository.Get<string>("Membership", "Guest Account");

            var user = UserRepository.GetUserByNavigation(guestAccount);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);

                var roles = UserRepository.GetUserRolesByUserId(user.Id);
                string arrayOfRoles = string.Join("|", roles.Select(o => o.Name));

                var ticket = new FormsAuthenticationTicket(
                     version: 1,
                     name: user.Id.ToString(),
                     issueDate: DateTime.Now,
                     expiration: DateTime.Now.AddMinutes(Session.Timeout),
                     isPersistent: false,
                     userData: String.Join("|", arrayOfRoles));

                var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                UserRepository.UpdateUserLastLoginDateByUserId(user.Id);

                Response.Cookies.Add(cookie);

                return true;
            }

            return false;
        }

        public void PerformLogin(string emailAddress, string password)
        {
            var requireEmailVerification = ConfigurationRepository.Get<bool>("Membership", "Require Email Verification");

            var user = UserRepository.GetUserByEmailAndPassword(emailAddress, password);
            if (user != null)
            {
                if (requireEmailVerification == true && user.EmailVerified == false)
                {
                    throw new Exception("Email address has not been verified. Check you email or use the password reset link to confirm your email address.");
                }

                FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);

                var roles = UserRepository.GetUserRolesByUserId(user.Id);
                string arrayOfRoles = string.Join("|", roles.Select(o => o.Name));

                var ticket = new FormsAuthenticationTicket(
                     version: 1,
                     name: user.Id.ToString(),
                     issueDate: DateTime.Now,
                     expiration: DateTime.Now.AddMinutes(Session.Timeout),
                     isPersistent: false,
                     userData: String.Join("|", arrayOfRoles));

                var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                UserRepository.UpdateUserLastLoginDateByUserId(user.Id);

                Response.Cookies.Add(cookie);
            }
            else
            {
                throw new Exception("Invalid Username or Password");
            }
        }

        public string RouteValue(string key, string defaultValue = "")
        {
            if (RouteData.Values.ContainsKey(key))
            {
                return RouteData.GetRequiredString(key);
            }
            return defaultValue;
        }
    }
}