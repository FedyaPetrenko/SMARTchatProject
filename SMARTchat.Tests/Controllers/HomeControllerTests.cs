using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMARTchatWEB.Controllers;

namespace SMARTchat.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void IndexAction_RedirectToLoginAction()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
