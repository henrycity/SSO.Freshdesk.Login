using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Moq;
using NPS.SSO.Freshdesk.Login.Controllers;
using NPS.SSO.Freshdesk.Login.Models;
using NUnit.Framework;

namespace NPS.SSO.Freshdesk.Login.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void Should_redict_to_freshdesk_Url()
        {
            //Arrange
            var sut = new HomeController();
            IEnumerable<Claim> claimCollection = new List<Claim>
            {
                new Claim("name", "John Doe"),
                new Claim("email", "johndoe@yahoo.com"),
                new Claim("companyid", "Some Company"),
                new Claim("phone_number", "0123456")
            };
            Thread.CurrentPrincipal = new TestPrincipal(claimCollection);
            var timeMock = new Mock<TimeProvider>();
            timeMock.SetupGet(tp => tp.UtcNow).Returns(new DateTime(2016, 12, 7));
            TimeProvider.Current = timeMock.Object;
            var expectedUrl =
                "https://letsgocena.freshdesk.com/login/sso?name=John+Doe&email=johndoe%40yahoo.com&timestamp=1481068800&phone=0123456&company=Some+Company&hash=83af5fd25f4701b03e43d95debeed630";

            // Act
            var viewresult = sut.Freshdesk() as RedirectResult;

            // Assert
            Assert.That(viewresult.Url, Is.EqualTo(expectedUrl));
        }

        [TearDown]
        public void TearDown()
        {
            TimeProvider.ResetToDefault();
        }
    }
}