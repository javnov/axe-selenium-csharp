using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace javnov.Selenium.Axe
{
    public static class Reporter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="violations">JSONArray of violations</param>
        /// <returns>readable report of accessibility violations found</returns>
        public static string Report(JArray violations)
        {
            throw new NotImplementedException("Implemented me Perverse lord");
        }

        private static void AppendFixes(StringBuilder sb, JArray arr, string heading)
        {
            throw new NotImplementedException("Implemented me Perverse lord");
        }

        private static string GetOrdinal(int number)
        {
            String ordinal = "";

            int mod;

            while (number > 0)
            {
                mod = (number - 1) % 26;
                ordinal = (char)(mod + 97) + ordinal;
                number = (number - mod) / 26;
            }

            return ordinal;
        }

        /**
         * Writes a raw object out to a JSON file with the specified name.
         * @param name Desired filename, sans extension
         * @param output Object to write. Most useful if you pass in either the Builder.analyze() response or the
         *               violations array it contains.
         */
        public static void WriteResults(string name, object output)
        {
            using (StreamWriter writer = new StreamWriter(name + ".json")) { 
                writer.WriteLine(output.ToString());
            }
        }
    }
}
