using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS.SSO.Freshdesk.Login.Models
{
    public class User
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Company { get; set; }
    }
}