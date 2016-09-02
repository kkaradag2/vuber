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

            if (!Directory.Exists(obj.WorkingDirectory))
                return false;

            if (!Directory.Exists(obj.ExecutedDirectory))
                return false;

            if (!Directory.Exists(obj.RollbackDirectory))
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

            if (!Directory.Exists(obj.WorkingDirectory))
                Console.WriteLine("Working Directory is {0} state of Fail", obj.WorkingDirectory?? "null");
            else
                Console.WriteLine("Working Directory is {0} state of Pass", obj.WorkingDirectory.ToString());

            if (!Directory.Exists(obj.ExecutedDirectory))
                Console.WriteLine("Executed Directory is {0} state of Fail", obj.ExecutedDirectory ?? "null");
            else
                Console.WriteLine("Executed Directory is {0} state of Pass", obj.ExecutedDirectory.ToString());

            if (!Directory.Exists(obj.RollbackDirectory))
                Console.WriteLine("Rollback Directory is {0} state of Fail", obj.RollbackDirectory ?? "null");
            else
                Console.WriteLine("Rollback Directory is {0} state of Pass", obj.RollbackDirectory.ToString());


            

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


        public static string TestConnection()
        {

             string ConfigFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(ConfigFile))
            {
                return "Fail";
            }
            //JSON.stringify
              

            VuberConfig obj = new VuberConfig();
            obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(ConfigFile));

            using (var db = new HistoryContext())
            {
               
                try
                {
                    db.Database.Connection.ConnectionString = obj.ConnectionString.ToString();
                    db.Database.Connection.Open();
                    if (db.Database.Connection.State == ConnectionState.Open)
                    {
                        return "Success";
                    }
                    return "Fail";
                }
                catch (Exception ex)
                {
                    return string.Format("Fail {0}", ex.Message);
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

            string rootPath = obj.WorkingDirectory.ToString();
            string[] files = Directory.GetFiles(rootPath, "*.sql", SearchOption.AllDirectories);
            if (files.Length > 0)
                return true;
            else
                return false;
        }




    }
}
