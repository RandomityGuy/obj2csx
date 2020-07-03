using System;

namespace obj2csx
{
    class Program
    {
        static void Main(string[] args)
        {
            bool tomap = false;
            bool split = false;
            bool flipnormals = false;
            int splitcount = 0;

            for (var i = 0; i < args.Length;i++)
            {
                var curarg = args[i];
                if (args[i] == "-map") tomap = true;
                if (args[i] == "-split")
                {
                    split = true;
                    splitcount = Convert.ToInt32(args[i+1]);
                }
                if (args[i] == "-flipnormals") flipnormals = true;
                if (args[i] == "-help")
                {
                    Console.WriteLine(@"-----obj2csx options:-------");
                    Console.WriteLine("-map: convert obj to map instead");
                    Console.WriteLine("-split <count>: split obj to <count> pieces");
                    Console.WriteLine("-flipnormals: flip normals");
                }
            }

            if (tomap)
            {
                var c = new MAPExporter();
                Console.WriteLine("Converting obj to csx");
                c.ConvertToMAP(args[0], args[0].Substring(0, args[0].Length - 3) + "map");
            }
            else
            {
                var c = new Converter();
                Console.WriteLine("Converting obj to csx");
                c.ConvertToCSX(args[0], args[0].Substring(0, args[0].Length - 3) + "csx",splitcount,flipnormals);
            }

        }
    }
}
