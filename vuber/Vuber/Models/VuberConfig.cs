using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vuber.Models
{





    public class VuberConfig
    {
        public string ConnectionString { get; set; }
        public string WorkingDirectory { get; set; }
        public string ExecutedDirectory { get; set; }
        public string RollbackDirectory { get; set; }



        public VuberConfig(string initial)
        {
            string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(ConfigFile))
            {
                SaveConfigFile(this);
            }

            VuberConfig initialObject = new VuberConfig();
            initialObject = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));

            this.ConnectionString = initialObject.ConnectionString;
            this.WorkingDirectory = initialObject.WorkingDirectory;
            this.ExecutedDirectory = initialObject.ExecutedDirectory;
            this.RollbackDirectory = initialObject.RollbackDirectory;


        }

        public VuberConfig()
        { }


        public bool isConfigurated()
        {
             string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
             if (!File.Exists(ConfigFile))
             {
                 return false;
             }
             VuberConfig obj = new VuberConfig();
             obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));

             if (obj.ConnectionString == null)
                 return false;

             if (obj.WorkingDirectory == null )
                 return false;

             if (obj.ExecutedDirectory == null)
                 return false;

             if (obj.RollbackDirectory == null)
                 return false;


             return true;

        }

        public void DisplayConfig()
        {
            string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(ConfigFile))
            {
                Console.WriteLine("Configration file is not found.");
                Environment.Exit(0);
            }
            VuberConfig obj = new VuberConfig();
            obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));

            Console.WriteLine("Connection String    {0}", obj.ConnectionString ?? "Fail");
            Console.WriteLine("Working Directory    {0}", obj.WorkingDirectory ?? "Fail");
            Console.WriteLine("Executed Directory   {0}", obj.ExecutedDirectory ?? "Fail");
            Console.WriteLine("Rollback Directory   {0}", obj.RollbackDirectory ?? "Fail");

        }





        public void SaveConfigFile(VuberConfig obj)
        {
            string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            string output = JsonConvert.SerializeObject(obj);
            File.WriteAllText(ConfigFile, output);
        }

        


    }
}
