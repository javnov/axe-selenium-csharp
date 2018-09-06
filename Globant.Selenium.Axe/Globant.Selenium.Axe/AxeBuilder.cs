using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Globant.Selenium.Axe
{
    /// <summary>
    /// Fluent style builder for invoking aXe. Instantiate a new Builder and configure testing with the include(),
    /// exclude(), and options() methods before calling analyze() to run.
    /// </summary>
    public class AxeBuilder
    {
        private readonly IWebDriver _webDriver;
        private readonly IncludeExcludeManager _includeExcludeManager = new IncludeExcludeManager();

        private static readonly AxeBuilderOptions DefaultOptions = new AxeBuilderOptions { ScriptProvider = new EmbeddedResourceAxeProvider() };

        public string Options { get; set; } = "null";
        public bool WaitForResult { get; set; } = true;
        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        public AxeBuilder(IWebDriver webDriver) : this(webDriver, DefaultOptions)
        {
        }

        /// <summary>
        /// Initialize an instance of <see cref="AxeBuilder"/>
        /// </summary>
        /// <param name="webDriver">Selenium driver to use</param>
        /// <param name="options">Builder options</param>
        public AxeBuilder(IWebDriver webDriver, AxeBuilderOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _webDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            _webDriver.Inject(options.ScriptProvider);
        }

        /// <summary>
        /// Execute the script into the target.
        /// </summary>
        /// <param name="command">Script to execute.</param>
        /// <param name="args"></param>
        private AxeResult Execute(string command, params object[] args)
        {
            ((IJavaScriptExecutor)_webDriver).ExecuteScript(command, args);

            if (WaitForResult)
            {
                var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(60));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("accessibilityResultReady")));
            }

            object response = ((IJavaScriptExecutor)_webDriver).ExecuteScript("return window.promiseResultAccessibility;", args);
            var jObject = JObject.FromObject(response);
            return new AxeResult(jObject);
        }

        /// <summary>
        /// Selectors to include in the validation.
        /// </summary>
        /// <param name="selectors">Any valid CSS selectors</param>
        /// <returns></returns>
        public AxeBuilder Include(params string[] selectors)
        {
            _includeExcludeManager.IncludeToAnalyzer(selectors);
            return this;
        }

        /// <summary>
        /// Exclude selectors
        /// Selectors to exclude in the validation.
        /// </summary>
        /// <param name="selectors">Any valid CSS selectors</param>
        /// <returns></returns>
        public AxeBuilder Exclude(params string[] selectors)
        {
            _includeExcludeManager.ExcludeToAnalyzer(selectors);
            return this;
        }

        /// <summary>
        /// Run aXe against a specific WebElement.
        /// </summary>
        /// <param name="context"> A WebElement to test</param>
        /// <returns>An aXe results document</returns>
        public AxeResult Analyze(IWebElement context)
        {
            var id = context.GetAttribute("id");
            if (string.IsNullOrEmpty(id)){
                throw new ArgumentException("WebElement must have a valid id");
            }

            var commandParams = Options == "null" ? $"document.getElementById('{id}')" : $"document.getElementById('{id}'), {Options}";
            var fullCommand = @"window.promiseResultAccessibility = null;
                                axe.run([params]).then(function (result) {
                                promiseResultAccessibility = result;
                                $(document).add('div').attr('id','accessibilityResultReady');
                            });
                        ";
            fullCommand = fullCommand.Replace("[params]", commandParams);
            return Execute(fullCommand, context);
        }

        /// <summary>
        /// Run aXe against the page.
        /// </summary>
        /// <returns>An aXe results document</returns>
        public AxeResult Analyze()
        {
            string commandParams;

            if (_includeExcludeManager.HasMoreThanOneSelectorsToIncludeOrSomeToExclude())
            {
                commandParams = Options == "null" ? $"{ _includeExcludeManager.ToJson()}" : $"{ _includeExcludeManager.ToJson()}, {Options}";
            }
            else if (_includeExcludeManager.HasOneItemToInclude())
            {
                string itemToInclude = _includeExcludeManager.GetFirstItemToInclude().Replace("'", "");
                commandParams = Options == "null" ? $"'{itemToInclude}'" : $"'{itemToInclude}', {Options}";
            }
            else
            {
                commandParams = Options == "null" ? $"document" : $"document, {Options}";
            }
            var fullCommand = @"window.promiseResultAccessibility = null;
                                axe.run([params]).then(function (result) {
                                promiseResultAccessibility = result;
                                $(document).add('div').attr('id','accessibilityResultReady');
                            });
                        ";
            fullCommand = fullCommand.Replace("[params]", commandParams);
            return Execute(fullCommand);
        }
    }
}