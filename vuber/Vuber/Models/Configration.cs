using System;
using CommandLine;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Vuber.Configuration;
using System.Reflection;
using System.Linq;

namespace Vuber.Models
{
    [Verb("config", HelpText = "Configure server connection string and working directory. Test configration or display current configrations.")]
    internal class Configration
    {
        [Option('c', "connection-string", HelpText = "Set SQL server connection string. SAMPLE: vucer config -c Server=yourserver;Database=yourDB;Trusted_Connection=True;")]
        public string ConnectionString { get; set; }

        [Option('w', "working-directory", HelpText = @"Set working directory that save pending files on your computer. SAMPLE: vucer config -w c:\sql\migration")]
        public string WorkingDirectory { get; set; }        

        [Option('e', "executed-directory", HelpText = @"Set folder that after migration files where to move. SAMPLE: vucer config -e c:\sql\complated")]
        public string ExecutedDirectory { get; set; }

        [Option('r', "rollback-directory", HelpText = @"Set folder that after failed migration files where to move. SAMPLE: vucer config -r c:\sql\Rollback")]
        public string RollbackDirectory { get; set; }

        [Option('l', "List", HelpText = @"List current configration")]
        public bool List { get; set; }

        [Option('t', "Test", HelpText = @"Test curent configration. Test SQL server, working directoryies.")]
        public bool Test { get; set; }

        private static Settings config = new Settings();

        internal void RunConfiguration()
        {
            if(!string.IsNullOrEmpty(ConnectionString))
            {
                try
                {
                    using (new SqlConnection(ConnectionString)){}
                    config.ConnectionString.Write(ConnectionString);                  
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection string is missing. {0}", ConnectionString);
                }
            }       
            
            if(!string.IsNullOrEmpty(WorkingDirectory))
            {
                if(ValidatePath(WorkingDirectory))
                {
                    config.WorkingDirectory.Write(WorkingDirectory);
                }
                else
                {
                    Console.WriteLine("Invalid Working directory. {0}", WorkingDirectory);
                }
            }

            if (!string.IsNullOrEmpty(RollbackDirectory))
            {
                if (ValidatePath(RollbackDirectory))
                {
                    config.RollbackDirectory.Write(RollbackDirectory);
                }
                else
                {
                    Console.WriteLine("Invalid Rollback directory. {0}", RollbackDirectory);
                }
            }

            if (!string.IsNullOrEmpty(ExecutedDirectory))
            {
                if (ValidatePath(ExecutedDirectory))
                {
                    config.ExecutedDirectory.Write(ExecutedDirectory);
                }
                else
                {
                    Console.WriteLine("Invalid Executed directory. {0}", ExecutedDirectory);
                }
            }


            if (List)
            {
                Utils.DisplayConfigration();
            }

            if(Test)
            {
                Utils.DisplayConfigurationTest();
            }

            if (isAllPropertiesEmpty())
                Parser.Default.ParseArguments<Configration>(new string[] { "verb", "--help" });



        }

        private static bool ValidatePath(string path)
        {
            Regex folder = new Regex(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");  // Folder regex  c:\sql\working
            Regex share = new Regex(@"^(\\)(\\[A-Za-z0-9-_]+){2,}(\\?)$");                               // Share Folder regex   \\fileserver\shared

            if (folder.Match(path).Success || share.Match(path).Success)
                return true;

            return false;                
        }

        public bool isAllPropertiesEmpty()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            int nullCount = 0;

            foreach (var pi in properties)
            {
                if (pi.PropertyType == typeof(string))         // To find any string propery is null or empty
                {
                    string value = (string)pi.GetValue(this);
                    if (string.IsNullOrEmpty(value)) { nullCount++; }
                }

                if (pi.PropertyType == typeof(bool))        // To find andy bool propery is null or empty
                {
                    bool value = (bool)pi.GetValue(this);
                    if (!value) { nullCount++; }
                }
            }

            if (nullCount == properties.Count())
                return true;

            return false;
        }


    }
}