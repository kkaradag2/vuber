using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vuber.Models
{
    class Utils
    {

        public static string DirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return "Success";
            }
            else
            {
                return "False";
            }
        }


        public static bool TestConfigtation()
        {
            

            string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(ConfigFile))
            {
                return false;
            }

            VuberConfig obj = new VuberConfig();
            obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));


            using (var db = new HistoryContext())
            {                
                try
                {
                    db.Database.Connection.ConnectionString = obj.ConnectionString.ToString();
                    db.Database.Connection.Open();
                    if (db.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {                    
                    return false;
                }
            }

            if (!Directory.Exists(obj.Directory))
                return false;

            if (!Directory.Exists(obj.ExecutedFolder))
                return false;

            if (!Directory.Exists(obj.RollBackFolder))
                return false;

            return true;

        }

        public static void DisplayConfigration()
        {
            string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(ConfigFile))
            {
                Console.WriteLine("Configration file is not found.");
            }
            //JSON.stringify
              

            VuberConfig obj = new VuberConfig();
            obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));

            if (!Directory.Exists(obj.Directory))
                Console.WriteLine("Working Directory is {0} state of Fail", obj.Directory?? "null");
            else
                Console.WriteLine("Working Directory is {0} state of Pass", obj.Directory.ToString());

            if (!Directory.Exists(obj.ExecutedFolder))
                Console.WriteLine("Executed Directory is {0} state of Fail", obj.ExecutedFolder ?? "null");
            else
                Console.WriteLine("Executed Directory is {0} state of Pass", obj.ExecutedFolder.ToString());

            if (!Directory.Exists(obj.RollBackFolder))
                Console.WriteLine("Rollback Directory is {0} state of Fail", obj.RollBackFolder ?? "null");
            else
                Console.WriteLine("Rollback Directory is {0} state of Pass", obj.RollBackFolder.ToString());


            

            using (var db = new HistoryContext())
            {
                try
                {
                    db.Database.Connection.ConnectionString = obj.ConnectionString.ToString();
                    db.Database.Connection.Open();
                    if (db.Database.Connection.State != ConnectionState.Open)
                    {
                        Console.WriteLine("Database Connection Fail {0}", obj.ConnectionString);
                    }
                    Console.WriteLine("Connection String is Pass {0}", obj.ConnectionString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database Connection Fail\n{0} {1}", ex.Message, obj.ConnectionString);
                }

            }
            

        }


        public static bool isAnyPendigFile()
        {
            string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(ConfigFile))
            {
                return false;
            }

            VuberConfig obj = new VuberConfig();
            obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));

            string rootPath = obj.Directory.ToString();
            string[] files = Directory.GetFiles(rootPath, "*.sql", SearchOption.AllDirectories);
            if (files.Length > 0)
                return true;
            else
                return false;
        }




    }
}
