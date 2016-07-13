using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NPS.SSO.Freshdesk.Login.Tests
{
    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(IList<Claim> claims) : base(new TestIdentity(claims))
        {
        }
    }
}