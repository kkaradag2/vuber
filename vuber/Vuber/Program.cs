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
                            .WithParsed<migration>(opts => migrate(opts,config))
                            .WithNotParsed(errs => Console.WriteLine(""));


        }

        private static void migrate(migration opts, VuberConfig config)
        {
            if (Utils.TestConfigtation())
            {
                if (Utils.isAnyPendigFile())
                {
                    string rootPath = config.WorkingDirectory.ToString();
                    string[] files = Directory.GetFiles(rootPath, "*.sql", SearchOption.AllDirectories);
                    List<string> Group = new List<string>();

                    foreach (var item in files)
                    {
                        string result = new DirectoryInfo(item).Parent.Name;
                        Group.Add(result);
                    }

                    foreach (var item in Group.Distinct().ToArray())
                    {
                        string SubrootPath = string.Format(@"{0}\{1}", config.WorkingDirectory.ToString(), item);
                        string[] Subfiles = Directory.GetFiles(SubrootPath, "*.sql");
                        string identity = Guid.NewGuid().ToString();
                        foreach (var file in Subfiles)
                        {
                            if (File.Exists(file))
                            {

                                VuberHistoryLogs history = new VuberHistoryLogs()
                                {
                                    Execution = DateTime.Now,
                                    ExecutionResult = string.Empty,
                                    FileName = new DirectoryInfo(file).Name,
                                    LogicalGroup = item,
                                    State = "Working",
                                    ExecutionIdentity = identity,
                                    FileContext = File.ReadAllText(file),
                                    UserBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                };
                                Utils.AddHistory(history, config.ConnectionString);
                            }
                        }
                        string strExecuteSQL = string.Empty;
                        List<VuberHistoryLogs> WorkingRows = Utils.GetWorkingFiles(identity,config.ConnectionString);
                        foreach (VuberHistoryLogs fl in WorkingRows)
                        {
                            strExecuteSQL += fl.FileContext;
                        }

                        string message = Utils.ExecuteFiles(strExecuteSQL, item, config.ConnectionString);

                        if (message == "Command(s) completed successfully.")
                        {
                            Console.WriteLine(message);
                            Utils.UpdateStates(WorkingRows, "Success", message, config.ConnectionString);
                            string moveDirectory = string.Format(@"{0}\{1}", config.ExecutedDirectory, item);

                            if (Directory.Exists(moveDirectory))
                                Directory.Delete(moveDirectory, true);
                            Directory.Move(SubrootPath, moveDirectory);
                        }
                        else
                        {
                            Console.WriteLine(message);
                            Utils.UpdateStates(WorkingRows, "Rollback", message,config.ExecutedDirectory);
                            string rollbackDirectory = string.Format(@"{0}\{1}", config.RollbackDirectory, identity);
                            Directory.Move(SubrootPath, rollbackDirectory);
                            Console.WriteLine("file(s) moved rollback folder as {0}", rollbackDirectory);
                        }

                        var table = new ConsoleTable("Group", "File Name", "State", "Message", "Execution");
                        string rollbackmsg = string.Empty;
                        foreach (VuberHistoryLogs worked in WorkingRows)
                        {
                            table.AddRow(worked.LogicalGroup, worked.FileName, worked.State, worked.ExecutionResult.Substring(0, 20), worked.Execution.ToShortDateString());
                            rollbackmsg = worked.ExecutionResult;
                        }
                        table.Write();
                    }

                }
                else
                {
                    Console.WriteLine("No pending file...");
                }
            }
            else
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
