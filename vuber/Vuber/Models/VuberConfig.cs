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
        public string Directory { get; set; }
        public string ExecutedFolder { get; set; }
        public string RollBackFolder { get; set; }



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
           


        }

        public VuberConfig()
        { }





    //    //public VuberConfig()
    //    //{            
    //    //    //if (!isConfigFileExisist())
    //    //    //{
    //    //    //    this.SaveConfigFile(this);
    //    //    //}
    //    //    //string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);

    //    //    //if (!File.Exists(ConfigFile))
    //    //    //{
    //    //    //    this.SaveConfigFile(this);
    //    //    //}
            

    //    //    //VuberConfig obj =  JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));
            
    //    //    //this.ConnectionString = obj.ConnectionString;
    //    //    //this.Directory = obj.Directory;
    //    //    //this.ExecutedFolder = obj.ExecutedFolder;
    //    //    //this.RollBackFolder = obj.RollBackFolder;
    //    //}

        


    //    public bool isConfigFileExisist()
    //    {
    //        string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);

    //        if (File.Exists(ConfigFile))
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
        public void SaveConfigFile(VuberConfig obj)
        {
            string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            string output = JsonConvert.SerializeObject(obj);
            File.WriteAllText(ConfigFile, output);
        }

        


    }
}
