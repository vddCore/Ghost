using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ghost.Glow
{
    public class ObcySourceFile
    {
        public string FileName { get; }

        public ObcySourceFile(string fileName)
        {
            FileName = fileName;
        }

        public string DecryptEncodedString(string str)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < str.Length; i++)
            {
                var charCode = str[i];
                if (charCode >= 65 && charCode <= 90)
                {
                    sb.Append((char)(90 - (charCode - 65)));
                }
                else if (charCode >= 97 && charCode <= 122)
                {
                    sb.Append((char)(122 - (charCode - 97)));
                }
                else
                {
                    sb.Append(charCode);
                }
            }

            return sb.ToString();
        }

        public string DeobfuscateSources(out List<string> lookup)
        {
            using (var sr = new StreamReader(FileName))
            {
                lookup = new List<string>();

                var sources = sr.ReadToEnd();
                var lines = sources.Split('\n');

                var rawString = lines[2].Substring(11, lines[2].Length - 11 - 6);
                var rawStrings = rawString.Split(',').Select(x => DecryptEncodedString(x)).ToList();
                var deobufscationRegex = new Regex(@"_sz8x\[(\d+)\]", RegexOptions.Multiline);
                var aryMatches = deobufscationRegex.Matches(sources);

                foreach (Match aryMatch in aryMatches)
                {
                    var aryIndexString = aryMatch.Groups[1].Value;
                    var aryIndex = int.Parse(aryIndexString);

                    sources = sources.Replace($"_sz8x[{aryIndexString}]", rawStrings[aryIndex]);
                }

                return sources;
            }
        }
    }
}
