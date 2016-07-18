using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NPS.SSO.Freshdesk.Login.Models;
using NPS.SSO.Freshdesk.Login.Settings;

namespace NPS.SSO.Freshdesk.Login.Controllers
{
    public class HomeController : ControllerBase
    {
        [Authorize]
        public ActionResult Index()
        {
            logger.Info($"{GetUserName()} logs in.");
            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            ViewBag.Message = "Claims";
            var user = User as ClaimsPrincipal;
            var token = user.FindFirst("access_token");
            if (token != null)
            {
                ViewData["access_token"] = token.Value;
            }
            return View();
        }

        [Authorize]
        public ActionResult Freshdesk()
        {
            try
            {
                return Redirect(GetFreshdeskSsoUrl());
            }     
            catch (Exception ex)
            {
                logger.Error($"{GetUserName()} gets following error\r\n{ex.Message}");
                throw;
            }
        }

        private string GetFreshdeskSsoUrl()
        {
            var user = GetCurrentUser();
            var url = GenerateFreshdeskSsoUrl(user);
            logger.Info($"{user.Name} is redirecting to Freshdesk.");
            return url;
        }

        private string GenerateFreshdeskSsoUrl(User user)
        {
            var settings = new SSOFreshdeskClientSettings();
            var timestamp = TimeProvider.Current.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            var hash = GetHash(settings.SecretKey, user.Name, user.Email, timestamp);
            var url = $@"{settings.BaseUrl}/login/sso?name={HttpUtility.UrlEncode(user.Name)
                }&email={HttpUtility.UrlEncode(user.Email)
                }&timestamp={timestamp
                }&phone={HttpUtility.UrlEncode(user.Phone)
                }&company={HttpUtility.UrlEncode(user.Company)
                }&hash={hash}";
            return url;
        }

        private string GetHash(string secret, string name, string email, string timems)
        {
            var input = name + secret + email + timems;
            var keybytes = Encoding.UTF8.GetBytes(secret);
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var crypto = new HMACMD5(keybytes);
            var hash = crypto.ComputeHash(inputBytes);
            return hash.Select(b => b.ToString("x2"))
                .Aggregate(new StringBuilder(),
                    (current, next) => current.Append(next),
                    current => current.ToString());
        }

        public ActionResult Signout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            logger.Info($"{GetUserName()} logs out.");
            return Redirect("/");
        }

        public void SignoutCleanup(string sid)
        {
            var cp = (ClaimsPrincipal) User;
            var sidClaim = cp.FindFirst("sid");
            if (sidClaim != null && sidClaim.Value == sid)
            {
                Request.GetOwinContext().Authentication.SignOut("Cookies");
            }
        }
    }
}