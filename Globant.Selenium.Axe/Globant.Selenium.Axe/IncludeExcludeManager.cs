using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Globant.Selenium.Axe
{
    /// <summary>
    /// Handle all initialization, serialization and validations for includeExclude aXe object.
    /// For more info check this: https://github.com/dequelabs/axe-core/blob/master/doc/API.md#include-exclude-object
    /// </summary>
    public class IncludeExcludeManager
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public List<string[]> Include { get; set; }
        public List<string[]> Exclude { get; set; }

        /// <summary>
        /// Include the given selectors, i.e "#foo", "ul.bar .target", "div"
        /// </summary>
        /// <param name="selectors">Selectors to include</param>
        public void IncludeToAnalyzer(params string[] selectors)
        {
            ValidateParameters(selectors);
            if (Include == null)
                Include = new List<string[]>();
            Include.Add(selectors);
        }

        /// <summary>
        /// Include the given selectors, i.e "frame", "div.foo"
        /// </summary>
        /// <param name="selectors">Selectors to exclude</param>
        public void ExcludeToAnalyzer(params string[] selectors)
        {
            ValidateParameters(selectors);
            if (Exclude == null)
                Exclude = new List<string[]>();
            Exclude.Add(selectors);
        }

        /// <summary>
        /// Indicate if we have more than one entry on include list or we have entries on exclude list
        /// </summary>
        /// <returns>True or False</returns>
        public bool HasMoreThanOneSelectorsToIncludeOrSomeToExclude()
        {
            bool hasMoreThanOneSelectorsToInclude = Include != null && Include.Count > 1;
            bool hasSelectorsToExclude = Exclude != null && Exclude.Count > 0;

            return hasMoreThanOneSelectorsToInclude || hasSelectorsToExclude;
        }

        /// <summary>
        /// Indicate we have one entry on the include list
        /// </summary>
        /// <returns>True or False</returns>
        public bool HasOneItemToInclude() => Include != null && Include.Count == 1;

        /// <summary>
        /// Get first selector of the first entry on include list
        /// </summary>
        /// <returns></returns>
        public string GetFirstItemToInclude()
        {
            if (Include == null || Include.Count == 0)
                throw new InvalidOperationException("You must add at least one selector to include");

            return Include.First().First();
        }

        /// <summary>
        /// Serialize this instance on JSON format
        /// </summary>
        /// <returns>This instance serialized in JSON format</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, JsonSerializerSettings);
        }

        private static void ValidateParameters(string[] selectors)
        {
            if (selectors == null)
                throw new ArgumentNullException(nameof(selectors));

            if (selectors.Any(string.IsNullOrEmpty))
                throw new ArgumentException("There is some items null or empty", nameof(selectors));
        }
    }
}
