using System;
using System.IO;
using System.Net;

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
            
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("fukc: giev crorcet fiel ok?");
                return;
            }

            var src = new ObcySourceFile(fileName);

            File.WriteAllText(
                Path.GetFileNameWithoutExtension(src.FileName) + ".deobf.js",
                src.Deobfuscate()
            );

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}