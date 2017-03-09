using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe
{
    public class Injector
    {
        private ContentDownloader _contentDownloader;
        private readonly IWebDriver _webDriver;

        public Injector(IWebDriver webDriver)
        {
            if (webDriver == null)
                throw new ArgumentNullException("webDriver is null.");

            _webDriver = webDriver;
            _contentDownloader = new ContentDownloader(new System.Net.WebClient());
        }

        /// <summary>
        /// Recursively injects aXe into all iframes and the top level document.
        /// </summary>
        /// <param name="driver">WebDriver instance to inject into</param>
        /// <param name="resourceUrl"></param>
        public void Inject(IWebDriver driver, Uri resourceUrl)
        {
            string script = _contentDownloader.GetContent(resourceUrl);
             IList<IWebElement> parents = new List<IWebElement>();

            InjectIntoFrames(driver, script, parents);

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            driver.SwitchTo().DefaultContent();
            js.ExecuteScript(script);
        }

        /// <summary>
        ///  Recursively find frames and inject a script into them.
        /// </summary>
        /// <param name="driver">An initialized WebDriver</param>
        /// <param name="script">Script to inject</param>
        /// <param name="parents">A list of all toplevel frames</param>
        private void InjectIntoFrames(IWebDriver driver, string script, IList<IWebElement> parents)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            IList<IWebElement > frames = driver.FindElements(By.TagName("iframe"));

            foreach(var frame in frames)
            {
                driver.SwitchTo().DefaultContent();

                if (parents != null)
                {
                    foreach (IWebElement parent in parents)
                    {
                        driver.SwitchTo().Frame(parent);
                    }
                }

                driver.SwitchTo().Frame(frame);
                js.ExecuteScript(script);

                 IList<IWebElement> localParents = parents.ToList();
                localParents.Add(frame);

                InjectIntoFrames(driver, script, localParents);
            }
        }

    }
}
