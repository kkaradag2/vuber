using Newtonsoft.Json;
using System;
using System.IO;

namespace Vuber.Models
{
    public class VuberConfig
    {
        public VuberConfig(string initial)
        {
            if (!String.IsNullOrEmpty(initial))
            {
                var configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
                if (!File.Exists(configFile))
                    SaveConfigFile(this);

                var initialObject = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(configFile));

                ConnectionString = initialObject.ConnectionString;
                ExecutedDirectory = initialObject.ExecutedDirectory;
                RollbackDirectory = initialObject.RollbackDirectory;
                WorkingDirectory = initialObject.WorkingDirectory;
            }
        }

        public VuberConfig()
        { }

        public string ConnectionString { get; set; }
        public string ExecutedDirectory { get; set; }
        public string RollbackDirectory { get; set; }
        public string WorkingDirectory { get; set; }

        public void DisplayConfig()
        {
            var configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Configration file is not found.");
                Environment.Exit(0);
            }
            var obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(configFile));

            Console.WriteLine("Connection String    {0}", obj.ConnectionString ?? "Fail");
            Console.WriteLine("Working Directory    {0}", obj.WorkingDirectory ?? "Fail");
            Console.WriteLine("Executed Directory   {0}", obj.ExecutedDirectory ?? "Fail");
            Console.WriteLine("Rollback Directory   {0}", obj.RollbackDirectory ?? "Fail");
        }

        public void SaveConfigFile(VuberConfig obj)
        {
            var configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            string output = JsonConvert.SerializeObject(obj);
            File.WriteAllText(configFile, output);
        }
    }
}