using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ghost.Glow
{
    public class ObcySourceFile
    {
        public string FileName { get; }
        public string Source { get; private set; }
        
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

        public List<StringToken> BuildLookupIndex(string strTableVarName)
        {
            var rgx = new Regex(@"^var " + strTableVarName + @"(\s+)?\=(\s+)?\[(.+?)\];$",
                RegexOptions.Singleline | RegexOptions.Multiline);

            FillInSourceBufferIfNeeded();
            var match = rgx.Match(Source);

            if (match.Success)
            {
                Console.WriteLine($"Got lookup table match [{match.Index}:{match.Index + match.Length}]");

                var rawStrings = match.Groups[3].Value;
                rawStrings = rawStrings.Replace("\n", "");

                Console.Write($"Parsing the input... ");
                var strings = ParseStrings(rawStrings);
                Console.WriteLine($"Found {strings.Count} strings.");
                return strings;
            }

            return new List<StringToken>();
        }

        public string Deobfuscate(string lookupName = "_sz8x", bool stripLookup = true)
        {
            Console.WriteLine("Gonna do the thing I guess.");

            var lookup = BuildLookupIndex(lookupName);
            var source = Source;
            var replacement = new StringBuilder();

            Console.WriteLine("Deobfuscating.");
            for (var i = 0; i < lookup.Count; i++)
            {
                var token = lookup[i];

                replacement.Append(token.DelimiterChar);
                replacement.Append(token.DecryptedValue);
                replacement.Append(token.DelimiterChar);

                source = source.Replace($"_sz8x[{i}]", replacement.ToString());

                replacement.Clear();
            }

            return stripLookup ? source.Substring(source.IndexOf('\n')) : source;
        }

        private List<StringToken> ParseStrings(string raw)
        {
            var isInString = false;
            var currentStringDelimiter = (char)0;

            var currentString = string.Empty;
            var strings = new List<StringToken>();

            for (var i = 0; i < raw.Length; i++)
            {
                var c = raw[i];

                switch (c)
                {
                    case '"':
                    case '\'':
                        if (isInString && currentStringDelimiter == c)
                        {
                            strings.Add(new StringToken
                            {
                                Value = currentString,
                                DecryptedValue = DecryptEncodedString(currentString),
                                DelimiterChar = currentStringDelimiter
                            });

                            isInString = false;
                            currentString = string.Empty;
                            currentStringDelimiter = (char)0;
                        }
                        else if (!isInString)
                        {
                            isInString = true;
                            currentStringDelimiter = c;
                        }

                        continue;

                    case '\\':
                        switch (raw[++i])
                        {
                            case 'u':
                                i++;
                                var unicodeNumber = raw[i..(i + 4)];
                                var charPoint = int.Parse(unicodeNumber, NumberStyles.HexNumber);
                                currentString += (char)charPoint;

                                i += 4;
                                continue;

                            case '\\':
                                if (isInString)
                                    currentString += "\\\\";
                                continue;

                            default:
                                currentString += raw[i];
                                continue;
                        }

                    default:
                        if (isInString)
                        {
                            currentString += c;
                        }

                        break;
                }
            }

            return strings;
        }

        private void FillInSourceBufferIfNeeded()
        {
            if (string.IsNullOrEmpty(Source))
            {
                Console.Write("Loading source code into memory... ");
                using (var sr = new StreamReader(FileName))
                {
                    Source = sr.ReadToEnd();
                    Console.WriteLine($"{Source.Length} chars.");
                }
            }
        }
    }
}