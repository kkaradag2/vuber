using ConsoleTables.Core;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Vuber.Configuration;
using System.Drawing;

namespace Vuber.Models
{
    internal class Utils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Settings config = new Settings();
        public static string DirectoryExists(string path)
        {
            if (Directory.Exists(path))
                return "Success";
            return "False";
        }

        public static bool TestConfigtation()
        {
            var configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(configFile))
            {
                return false;
            }

            var obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(configFile));

            using (var db = new HistoryContext())
            {
                try
                {
                    db.Database.Connection.ConnectionString = obj.ConnectionString;
                    db.Database.Connection.Open();
                    if (db.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message);
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
            try
            {
                Console.WriteLine("\nCurrent Configuration:\n", Color.DarkGreen);

                var table = new ConsoleTable("Properties", "Values");
                table.AddRow("Working Directory",  string.IsNullOrEmpty(config.WorkingDirectory)  ? "Not Defined" : config.WorkingDirectory)
                     .AddRow("Executed Directory", string.IsNullOrEmpty(config.ExecutedDirectory) ? "Not Defined" : config.ExecutedDirectory)
                     .AddRow("Rollback Directory", string.IsNullOrEmpty(config.RollbackDirectory) ? "Not Defined" : config.RollbackDirectory)
                     .AddRow("Connection String",  string.IsNullOrEmpty(config.ConnectionString)  ? "Not Defined" : config.ConnectionString);
                
                table.Write();
            }
            catch (Exception)
            {
                Console.WriteLine("Configuration is not defined.", Color.Red);
                Console.WriteLine("You can learn how to define the configuration with the help Config.");
            }
        }

        public static string TestConnection()
        {
            var configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(configFile))
            {
                return "Fail";
            }

            var obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(configFile));
            using (var db = new HistoryContext())
            {
                try
                {
                    db.Database.Connection.ConnectionString = obj.ConnectionString;
                    db.Database.Connection.Open();
                    if (db.Database.Connection.State == ConnectionState.Open)
                    {
                        return "Success";
                    }
                    return "Fail";
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message);
                    return string.Format("Fail {0}", ex.Message);
                }
            }
        }

        public static void DisplayConfigurationTest()
        {
            var table = new ConsoleTable("Properties", "Values", "Status");

            if (!string.IsNullOrEmpty(config.ConnectionString))
            {
                
                table.AddRow("Connection String", config.ConnectionString, CheckConnectionString(config));               
            }
            else
            {
                table.AddRow("Connection String", "not defined", "Fail");
            }

            if (Directory.Exists(config.WorkingDirectory))
            {
                table.AddRow("Working Directory", config.WorkingDirectory ?? "null", "Success");
            }
            else
            {
                table.AddRow("Working Directory", string.IsNullOrEmpty(config.WorkingDirectory) ? "Not Defined" : "", "Fail");
            }

            if (Directory.Exists(config.ExecutedDirectory))
            {
                table.AddRow("Execution Directory", config.ExecutedDirectory ?? "null", "Success");
            }
            else
            {
                table.AddRow("Execution Directory", string.IsNullOrEmpty(config.ExecutedDirectory) ? "Not Defined" : "", "Fail");
            }

            if (Directory.Exists(config.RollbackDirectory))
            {
                table.AddRow("Rollback Directory", config.RollbackDirectory ?? "null", "Success");
            }
            else
            {
                table.AddRow("Rollback Directory", string.IsNullOrEmpty(config.RollbackDirectory) ? "Not Defined" : "", "Fail");
            }

            table.Write();
        }

        private static string CheckConnectionString(Settings config)
        {
           
           try {
                        using (new SqlConnection(config.ConnectionString)) { } return "Success";
           }
                catch (SqlException ex) {
                return string.Format("{0} - {1}", "Fail", ex.Message);
            }
            
            return "Fail";
        }

        public static bool IsAnyPendigFile()
        {
            var configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
            if (!File.Exists(configFile))
            {
                return false;
            }
            var obj = JsonConvert.DeserializeObject<VuberConfig>(File.ReadAllText(configFile));

            string[] dirs = Directory.GetDirectories(obj.WorkingDirectory);

            foreach (var folder in dirs)
            {
                string[] files = Directory.GetFiles(folder, "*.sql", SearchOption.AllDirectories);
                if (files.Length > 0)
                    return true;
                return false;
            }
            return false;
        }

        internal static void AddHistory(VuberHistoryLogs history, string p)
        {
            using (var ctx = new HistoryContext())
            {
                ctx.Database.Connection.ConnectionString = p;
                ctx.Histories.Add(history);
                ctx.SaveChanges();
            }
        }

        public static List<VuberHistoryLogs> GetWorkingFiles(string gid, string p)
        {
            List<VuberHistoryLogs> result;

            using (var ctx = new HistoryContext())
            {
                ctx.Database.Connection.ConnectionString = p;
                result = ctx.Histories
                    .Where(b => b.ExecutionIdentity == gid).ToList();
            }
            return result;
        }

        public static string ExecuteFiles(string commandstr, string identity, string connStrl)
        {
            string returnMessage;
            using (SqlConnection connection = new SqlConnection(connStrl))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();

                // Start a local transaction.
                var transaction = connection.BeginTransaction(identity);

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    string[] commands = commandstr.Split(new[] { "GO\r\n", "GO ", "GO\t" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string c in commands)
                    {
                        command.CommandText = c;
                        command.ExecuteNonQuery();
                    }

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    returnMessage = "Command(s) completed successfully.";
                }
                catch (Exception ex)
                {
                    returnMessage = string.Format("Commit Exception Type: {0} Message {1}", ex.GetType(), ex.Message);
                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        returnMessage = string.Format("Rollback Exception Type: {0}  Message: {1}", ex2.GetType(), ex2.Message);
                    }
                }
            }
            return returnMessage;
        }

        public static void UpdateStates(List<VuberHistoryLogs> items, string state, string message, string connstrl)
        {
            using (var ctx = new HistoryContext())
            {
                ctx.Database.Connection.ConnectionString = connstrl;
                foreach (var item in items)
                {
                    ctx.Histories.Attach(item);
                    item.State = state;
                    item.ExecutionResult = message;
                    ctx.SaveChanges();
                }
            }
        }
    }
}