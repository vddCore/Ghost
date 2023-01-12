using System;
using System.IO;

namespace Ghost.Glow
{
    class Program
    {
        static void Main(string[] args)
        {
            var src = new ObcySourceFile("H:\\scriptbox.js");

            using (var sw = new StreamWriter(Path.GetFileNameWithoutExtension(src.FileName) + ".deobf.js"))
            {
                sw.WriteLine(src.DeobfuscateSources(out var lookup));

                using (var sw2 = new StreamWriter(Path.GetFileNameWithoutExtension(src.FileName) + ".lookup.txt"))
                {
                    foreach(var s in lookup)
                    {
                        sw2.WriteLine(s);
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
