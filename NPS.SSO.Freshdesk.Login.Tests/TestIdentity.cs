using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NPS.SSO.Freshdesk.Login.Tests
{
    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(IEnumerable<Claim> claims) : base(claims)
        {
        }
    }
}