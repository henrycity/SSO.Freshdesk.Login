using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NPS.SSO.Freshdesk.Login.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var baseUrl = "https://letsgocena.freshdesk.com";
            var user = ClaimsPrincipal.Current;
            var name = user.FindFirst("name").Value;
            var email = user.FindFirst("email").Value;
            var secret = "d3dc809dc6c5231897a1b8b9ee4c108c";
            var path = GetSsoUrl(baseUrl, secret, name, email);
            return Redirect(path);
        }

        private string GetSsoUrl(string baseUrl, string secret, string name, string email)
        {
            var timems = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            var hash = GetHash(secret, name, email, timems);
            var path = $@"{baseUrl}/login/sso?name={Server.UrlEncode(name)
                          }&email={Server.UrlEncode(email)
                          }&timestamp={timems
                          }&hash={hash}";
            return path;
        }

        private string GetHash(string secret, string name, string email, string timems)
        {
            var input = GetUserInformation();
            var keybytes = Encoding.UTF8.GetBytes(secret);
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var crypto = new HMACMD5(keybytes);
            var hash = crypto.ComputeHash(inputBytes);
            return hash.Select(b => b.ToString("x2"))
                .Aggregate(new StringBuilder(),
                    (current, next) => current.Append(next),
                    current => current.ToString());
        }

        private string GetUserInformation()
        {
            var user = ClaimsPrincipal.Current;
            var name = user.FindFirst("name").Value;
            var email = user.FindFirst("email").Value;
            var secret = "d3dc809dc6c5231897a1b8b9ee4c108c";
            var input = user + name + email + secret;
            return input;
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


        public ActionResult Signout()
        {
            Request.GetOwinContext().Authentication.SignOut();
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