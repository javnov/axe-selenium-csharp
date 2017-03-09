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
            StringBuilder sb = new StringBuilder();
            sb
                    .Append("Found ")
                    .Append(violations.Count)
                    .Append(" accessibility violations:");

            for (int i = 0; i < violations.Count; i++)
            {
                JObject violation = (JObject)violations.ElementAt(i);
                sb
                        .Append(System.Environment.NewLine)
                        .Append(i + 1)
                        .Append(") ")
                        .Append(violation["help"]);

                //if (violation["helpUrl"].Count} has("helpUrl"))
                if (violation["helpUrl"].HasValues)
                {
                    string helpUrl = violation["helpUrl"].ToString();
                    sb.Append(": ")
                            .Append(helpUrl);
                }

                JArray nodes = (JArray)violation["nodes"];

                for (int j = 0; j < nodes.Count; j++)
                {
                    JObject node = (JObject)nodes[j];
                    sb
                            .Append(System.Environment.NewLine)
                            .Append("  ")
                            .Append(GetOrdinal(j + 1))
                            .Append(") ")
                            .Append(node. .getJSONArray("target"))
                            .Append(System.Environment.NewLine);

                    JArray all = node.getJSONArray("all");
                    JArray none = node.getJSONArray("none");

                    for (int k = 0; k < none.length(); k++)
                    {
                        all.put(none.getJSONObject(k));
                    }

                    appendFixes(sb, all, "Fix all of the following:");
                    appendFixes(sb, node.getJSONArray("any"), "Fix any of the following:");
                }
            }

            return sb.ToString();
        }

        private static void AppendFixes(StringBuilder sb, JArray arr, string heading)
        {
            if (arr != null && arr.Count > 0)
            {
                sb
                        .Append("    ")
                        .Append(heading)
                        .Append(System.Environment.NewLine);

                for (int i = 0; i < arr.Count; i++)
                {
                    JObject fix = arr.getJSONObject(i);

                    sb
                            .Append("      ")
                            .Append(fix["message"])
                            .Append(System.Environment.NewLine);
                }

                sb.Append(System.Environment.NewLine);
            }
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
