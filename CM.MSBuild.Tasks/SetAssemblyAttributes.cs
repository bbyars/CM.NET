using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace CM.MSBuild.Tasks
{
    public class SetAssemblyAttributes : CMTask
    {
        [Required]
        public ITaskItem[] Files { get; set; }

        [Required]
        public string Attributes { get; set; }

        protected override void DoExecute()
        {
            var regex = new Regex(@"<(?<attribute>[\w\.]+)\s+Value=['""](?<value>[^'""]+)['""]");
            foreach (var file in Files)
            {
                var text = File.ReadAllText(file.ItemSpec);
                foreach (var attribute in GetAttributes())
                {
                    var match = regex.Match(attribute);
                    if (!match.Success)
                        throw new ArgumentException(string.Format("Invalid attribute format: {0}", attribute));

                    text = RemoveExistingAttribute(text, match.Groups["attribute"].Value);
                    text += string.Format("{0}[assembly: {1}(\"{2}\")]", Environment.NewLine, match.Groups["attribute"].Value, match.Groups["value"].Value);
                }
                File.WriteAllText(file.ItemSpec, text);
            }
        }

        private IEnumerable<string> GetAttributes()
        {
            var matches = Regex.Matches(Attributes, @"<[^>]+/>", RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                yield return match.Captures[0].Value;
            }
        }

        private static string RemoveExistingAttribute(string text, string attribute)
        {
            var pattern = string.Format(@"\[assembly: {0}[^\]]+\]", attribute);
            return Regex.Replace(text, pattern, "");
        }
    }
}