using Microsoft.AspNetCore.Mvc;
using Moq;
using TimelyTastes.Controllers;
using TimelyTastes.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using TimelyTastes.Tests.Unit.Helper;


namespace TimelyTastes.Tests.Unit.Controllers
{
    [TestClass]
    public class VendorControllerTest
    {
        private SQLiteDbContext? _mockContext;
        private Mock<ILogger<VendorsController>>? _mockLogger;
        private Mock<IHttpClientFactory>? _mockHttpClientFactory;

        private VendorsController? _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = TestHelper.GetInMemoryDbContext();
            _mockLogger = new Mock<ILogger<VendorsController>>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _controller = new VendorsController(_mockLogger.Object, _mockHttpClientFactory.Object, _mockContext);

        }

        [TestMethod]
        public void Create_WhenSessionVendorIdIsNull_ShouldRedirectToLogin()
        {
            byte[]? value = null;

            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.TryGetValue("VendorID", out value)).Returns(false);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession.Object;

            _controller!.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var result = _controller.Create() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("LogIn", result.ActionName);
        }

        [TestMethod]
        public void Create_WhenSessionVendorIdIsNotNull_ShouldNotRedirectToLogin()
        {

            byte[]? value = System.Text.Encoding.UTF8.GetBytes("testvendor");

            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.TryGetValue("VendorID", out value)).Returns(true);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession.Object;

            _controller!.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            var result = _controller.Create() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ActionName);
        }
    }
}