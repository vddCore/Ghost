using System;
using System.IO;
using System.Net;
using Ghost.Glow.Jsbeautifier;

namespace Ghost.Glow
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("fukc: giev fiel");
                return;
            }

            var fileName = args[0];

            if (args[0].StartsWith("http://") || args[0].StartsWith("https://"))
            {
                Console.WriteLine("htpp? ok. gon dolwnod fiel.");
                try
                {
                    fileName = "./6obcy.js";

                    using (var wc = new WebClient())
                        wc.DownloadFile(args[0], "./6obcy.js");
                }
                catch
                {
                    Console.WriteLine("fukc: fiel dlownad falied");
                    return;
                }
            }
            
            if (!File.Exists(fileName))
            {
                Console.WriteLine("fukc: giev crorcet fiel ok?");
                return;
            }

            Console.WriteLine("gon ask bro for hepl ok?");

            Console.WriteLine("---");
            var src = new ObcySourceFile(fileName);
            var deobf = src.Deobfuscate();
            Console.WriteLine("---");
            
            Console.WriteLine("thank bro! will prietifey.");
            var beautifier = new Beautifier();

            var actuallyReadableCode = beautifier.Beautify(deobf);
            File.WriteAllText(
                Path.GetFileNameWithoutExtension(src.FileName) + ".deobf.js",
                actuallyReadableCode
            );

            Console.WriteLine("im outta here.");
        }
    }
}