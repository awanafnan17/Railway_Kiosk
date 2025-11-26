using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RailwayKiosk.Tests
{
    [TestClass]
    public class LoginTests
    {
        [TestMethod]
        public void VerifyPassword_MatchesGeneratedHash()
        {
            var hash = RailwayKiosk.LoginForm.HashPassword("admin123");
            Assert.IsTrue(RailwayKiosk.LoginForm.VerifyPassword("admin123", hash));
            Assert.IsFalse(RailwayKiosk.LoginForm.VerifyPassword("wrong", hash));
        }
    }
}
