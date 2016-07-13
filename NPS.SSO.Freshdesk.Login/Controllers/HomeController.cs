using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NPS.SSO.Freshdesk.Login.Models;

namespace NPS.SSO.Freshdesk.Login.Controllers
{
    public class HomeController : Controller
    {
        readonly log4net.ILog logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            // Change this to our own Freshdesk portal.
            var baseUrl = "https://letsgocena.freshdesk.com";
            var currentUser = ClaimsPrincipal.Current;
            var name = GetUserInfo(currentUser, "name");
            var email = GetUserInfo(currentUser, "email");
            var phone = GetUserInfo(currentUser, "phone_number");
            var company = GetUserInfo(currentUser, "companyid");
            // Use TimeProvider to unit test against the current time.
            var timestamp = TimeProvider.Current.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            // Change this to our portal secret key.  
            var secret = "d3dc809dc6c5231897a1b8b9ee4c108c";
            var hash = GetHash(secret, name, email, timestamp);
            var path = $@"{baseUrl}/login/sso?name={HttpUtility.UrlEncode(name)
                }&email={HttpUtility.UrlEncode(email)
                }&timestamp={timestamp
                }&phone={HttpUtility.UrlEncode(phone)
                }&company={HttpUtility.UrlEncode(company)
                }&hash={hash}";
            logger.Info($"{name} is redirecting to Freshdesk.");
            return path;
        }

        private string GetUserInfo(ClaimsPrincipal user, string info)
        {
            if (user.FindFirst(info) == null || string.IsNullOrEmpty(user.FindFirst(info).Value))
            {
                logger.Warn($"{info} of {GetUserName()} is empty.");
            }
            return user.FindFirst(info) != null ? user.FindFirst(info).Value : "";
        }

        private string GetUserName()
        {
            return ClaimsPrincipal.Current.FindFirst("name").Value;
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