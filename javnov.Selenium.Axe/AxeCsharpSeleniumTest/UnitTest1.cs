using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using javnov.Selenium.Axe;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;

namespace AxeCsharpSeleniumTest
{
    [TestClass]
    public class UnitTest1
    {
        private IWebDriver _webDriver;
        private const string targetTestUrl = "https://www.carnival.com/cruise-ships.aspx";

        [TestInitialize]
        public void Initialize() {
            _webDriver = new FirefoxDriver();
        }

        [TestCleanup()]
        public virtual void TearDown()
        {
            _webDriver.Quit();
            _webDriver.Dispose();
        }


        [TestMethod]
        public void TestMethod1()
        {
            _webDriver.Navigate().GoToUrl(targetTestUrl);
            AxeBuilder axeBuilder = new AxeBuilder(_webDriver);
            var results = axeBuilder.Analyze();
        }
    }
}
