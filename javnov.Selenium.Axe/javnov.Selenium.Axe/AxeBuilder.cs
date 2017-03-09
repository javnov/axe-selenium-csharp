using javnov.Selenium.Axe.Properties;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Net;

namespace javnov.Selenium.Axe
{

    /**
	 * Chainable builder for invoking aXe. Instantiate a new Builder and configure testing with the include(),
	 * exclude(), and options() methods before calling analyze() to run.
	 */
    public class AxeBuilder
    {
        private readonly IWebDriver _webDriver;
        private readonly string _scriptContent;
        private IList<string> _includes;
        private IList<string> _excludes;

        #region Properties

        public string Options { get; set; }

        #endregion Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webDriver"></param>
        public AxeBuilder(IWebDriver webDriver)
        {
            if (webDriver == null)
                throw new ArgumentNullException("webDriver");

            _webDriver = webDriver;
            _scriptContent = Resources.axe_min;
            _includes = new List<string>();
            _excludes = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="axeScriptUrl"></param>
        public AxeBuilder(IWebDriver webDriver, Uri axeScriptUrl) : this(webDriver, axeScriptUrl, new WebClient()) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webDriver"></param>
        /// <param name="axeScriptUrl"></param>
        /// <param name="webClient"></param>
        public AxeBuilder(IWebDriver webDriver, Uri axeScriptUrl, WebClient webClient) : this(webDriver)
        {
            if (axeScriptUrl == null)
                throw new ArgumentNullException("axeScriptUrl");
            if (webClient == null)
                throw new ArgumentNullException("webClient");

            var contentDownloader = new ContentDownloader(webClient);
            _scriptContent = contentDownloader.GetContent(axeScriptUrl);
        }

        #region Private

        private JObject Execute(string command, params object[] args)
        {
            this._webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(30));
            Object response = ((IJavaScriptExecutor)this._webDriver).ExecuteAsyncScript(command, args);
            return new JObject(response);
        }


        #endregion Private

        #region Public Services

        public AxeBuilder Include(string selector)
        {
            this._includes.Add(selector);
            return this;
        }

        public AxeBuilder Exclude(string selector)
        {
            this._excludes.Add(selector);
            return this;
        }

        /// <summary>
        /// Run aXe against a specific WebElement.
        /// </summary>
        /// <param name="context"> A WebElement to test</param>
        /// <returns>An aXe results document</returns>
        public JObject Analyze(IWebElement context)
        {
            string command = string.Format("axe.a11yCheck(arguments[0], {0}, arguments[arguments.length - 1]);", Options);
            return Execute(command, context);
        }

        /// <summary>
        /// Run aXe against the page.
        /// </summary>
        /// <returns>An aXe results document</returns>
        public JObject Analyze()
        {
            string command;

            if (_includes.Count > 1 || _excludes.Count > 0)
            {
                command = string.Format("axe.a11yCheck({include: [{0}], exclude: [{1}]}, {2}, arguments[arguments.length - 1]);",
                        "['" + string.Join("'],['", _includes) + "']",
                        _excludes.Count == 0 ? "" : "['" + string.Join("'],['", _excludes) + "']",
                        Options);
            }

            else if (_includes.Count == 1)
            {
                command = string.Format("axe.a11yCheck('{0}', {1}, arguments[arguments.length - 1]);", _includes[0].Replace("'", ""), Options);
            }
            else
            {
                command = string.Format("axe.a11yCheck(document, {0}, arguments[arguments.length - 1]);", Options);
            }

            return Execute(command);
        }

        #endregion Public Services
    }
}