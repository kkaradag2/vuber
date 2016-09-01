using CommandLine;
using ConsoleTables.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vuber.Models;

namespace Vuber
{
    class Program
    {

       

        static void Main(string[] args)
        {
            VuberConfig config = new VuberConfig("inital");

            

            Parser.Default.ParseArguments<Configration, info, migration>(args)
                            .WithParsed<Configration>(opts => Configration(opts))
                            .WithParsed<info>(opts => info(opts, config))
                            .WithParsed<migration>(opts => migrate(opts))
                            .WithNotParsed(errs => Console.WriteLine(""));


        }



        private static void migrate(migration opts)
        {
            Console.WriteLine("migrate is working..");

        

        }

        private static void info(info opts, VuberConfig config)
        {
           if(Utils.TestConfigtation())
           {
               if (Utils.isAnyPendigFile())
               {
                   string rootPath = config.Directory.ToString();
                   string[] files = Directory.GetFiles(rootPath, "*.sql", SearchOption.AllDirectories);
                   var table = new ConsoleTable("Group", "File Name", "State");
                   foreach (string file in files)
                   {
                       string filename = Path.GetFileNameWithoutExtension(file);
                       string result = new DirectoryInfo(file).Parent.Name;
                       table.AddRow(result, filename, "pending");
                   }
                   table.Write(); 
               }else
               {
                   Console.WriteLine("No Pending File");
               }
           }else
           {
               Console.WriteLine("Configration is missing...");
               Utils.DisplayConfigration();
           }
        }

        private static void Configration(Configration opts)
        {
            Console.WriteLine("config is working..");
        }
    }
}
