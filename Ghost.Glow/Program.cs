using System;
using System.IO;

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

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("fukc: giev crorcet fiel ok?");
                return;
            }

            var src = new ObcySourceFile(args[0]);

            File.WriteAllText(
                Path.GetFileNameWithoutExtension(src.FileName) + ".deobf.js", 
                src.DeobfuscateSource()
            );
            
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}