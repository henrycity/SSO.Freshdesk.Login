using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using NPS.SSO.Freshdesk.Login.Models;

namespace NPS.SSO.Freshdesk.Login.Controllers
{
    public class ControllerBase : Controller
    {
        protected readonly log4net.ILog logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected User GetCurrentUser()
        {
            var currentUser = ClaimsPrincipal.Current;
            if (currentUser == null)
            {
                throw new ArgumentException("Current user is null or empty", "currentUser");
            }
            var name = GetUserInfo(currentUser, "name");
            var email = GetUserInfo(currentUser, "email");
            var phone = GetUserInfo(currentUser, "phone_number");
            var company = GetUserInfo(currentUser, "companyid");
            var user = new User(name, email, phone, company);
            return user;
        }

        private string GetUserInfo(ClaimsPrincipal user, string info)
        {
            if (user.FindFirst(info) == null || string.IsNullOrEmpty(user.FindFirst(info).Value))
            {
                logger.Warn($"{info} of {GetUserName()} is empty.");
            }
            return user.FindFirst(info) != null ? user.FindFirst(info).Value : "";
        }

        protected string GetUserName()
        {
            return ClaimsPrincipal.Current.FindFirst("name").Value;
        }
    }
}