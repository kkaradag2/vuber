using CommandLine;
using ConsoleTables.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
                            .WithParsed<Configration>(opts => Configration(opts, config))
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
                   string rootPath = config.WorkingDirectory.ToString();
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

        private static void Configration(Configration opts, VuberConfig config)
        {            
            foreach (var item in opts.GetType().GetProperties())
            {
                if (item.GetValue(opts, null) != null && item.PropertyType == typeof(string))
                {
                    if (item.Name != "ConnectionString")
                    {
                        Regex folder = new Regex(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");  // Folder regex  c:\sql\working
                        Regex share =  new Regex(@"^(\\)(\\[A-Za-z0-9-_]+){2,}(\\?)$");                              // Share Folder regex   \\fileserver\shared
                        if (folder.Match(item.GetValue(opts, null).ToString()).Success || share.Match(item.GetValue(opts, null).ToString()).Success)
                        {
                            config.GetType().GetProperty(item.Name).SetValue(config, item.GetValue(opts, null).ToString().Replace(@"\","\\"), null);
                        }else
                        {
                            Console.WriteLine("{0} property is not match folder or shared folder expressions. {1}", item.Name, item.GetValue(opts, null).ToString());
                        }
                        
                    }else
                    {

                        try
                        {
                            var conn = new SqlConnection(item.GetValue(opts, null).ToString());
                            config.GetType().GetProperty(item.Name).SetValue(config, item.GetValue(opts, null).ToString().Replace(@"\","\\"), null);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Connection string is missing. {0}", item.GetValue(opts, null).ToString());                            
                        }
                        
                    }
                }
            }

            config.SaveConfigFile(config);

            if (opts.list)            {
        

                    foreach (PropertyInfo prop in typeof(VuberConfig).GetProperties())
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            Console.WriteLine("{0} Configration is {1}", prop.Name, config.GetType().GetProperty(prop.Name).GetValue(config) ?? "Empty");
                        }
                    }
                
            }

            if (opts.test)
            {
                  string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (File.Exists(ConfigFile))
            {
                    Console.WriteLine("Connection String    {0}", Utils.TestConnection());
                    Console.WriteLine("Working  Directory   {0}", Utils.DirectoryExists(config.WorkingDirectory.ToString()));
                    Console.WriteLine("Executed Directory   {0}", Utils.DirectoryExists(config.ExecutedDirectory.ToString()));
                    Console.WriteLine("Rollback Directory   {0}", Utils.DirectoryExists(config.RollbackDirectory.ToString()));
                }else
                { 
                    Console.WriteLine("Sometings is wrong to configration.\nPleace use vuber --help config");
                }
            }


        }
    }
}
