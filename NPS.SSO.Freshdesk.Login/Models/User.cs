using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS.SSO.Freshdesk.Login.Models
{
    public class User
    {
        public User(string name, string email, string phone, string company)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Company = company;
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Company { get; set; }
    }
}