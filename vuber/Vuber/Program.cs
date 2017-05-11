using CommandLine;
using ConsoleTables.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Vuber.Models;

namespace Vuber
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            var configuration   = new VuberConfig("inital");
            //var migration       = new Migration();

            Parser.Default.ParseArguments<Configration, Info, Migration>(args)
                            .WithParsed<Configration>(opts => Configration(opts, configuration))
                            .WithParsed<Info>(opts => Info(configuration))
                            .WithParsed<Migration>(opts => Migrate(configuration, new Migration()))
                            .WithNotParsed(errs => Console.WriteLine(""));
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
                        Regex share = new Regex(@"^(\\)(\\[A-Za-z0-9-_]+){2,}(\\?)$");                              // Share Folder regex   \\fileserver\shared
                        if (folder.Match(item.GetValue(opts, null).ToString()).Success || share.Match(item.GetValue(opts, null).ToString()).Success)
                        {
                            config.GetType().GetProperty(item.Name).SetValue(config, item.GetValue(opts, null).ToString().Replace(@"\", "\\"), null);
                        }
                        else
                        {
                            Console.WriteLine("{0} property is not match folder or shared folder expressions. {1}", item.Name, item.GetValue(opts, null));
                        }
                    }
                    else
                    {
                        try
                        {
                            using (new SqlConnection(item.GetValue(opts, null).ToString()))
                            {
                            }
                            config.GetType().GetProperty(item.Name).SetValue(config, item.GetValue(opts, null).ToString().Replace(@"\", "\\"), null);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Connection string is missing. {0}", item.GetValue(opts, null));
                        }
                    }
                }
            }

            config.SaveConfigFile(config);

            if (opts.List)
            {
                foreach (PropertyInfo prop in typeof(VuberConfig).GetProperties())
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        Console.WriteLine("{0} Configration is {1}", prop.Name, config.GetType().GetProperty(prop.Name).GetValue(config) ?? "Empty");
                    }
                }
            }

            if (opts.Test)
            {
                string configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
                if (File.Exists(configFile))
                {
                    Console.WriteLine("Connection String    {0}", Utils.TestConnection());
                    Console.WriteLine("Working  Directory   {0}", Utils.DirectoryExists(config.WorkingDirectory));
                    Console.WriteLine("Executed Directory   {0}", Utils.DirectoryExists(config.ExecutedDirectory));
                    Console.WriteLine("Rollback Directory   {0}", Utils.DirectoryExists(config.RollbackDirectory));
                }
                else
                {
                    Console.WriteLine("Sometings is wrong to configration.\nPleace use vuber --help config");
                }
            }
        }

        private static void Info(VuberConfig config)
        {
            if (Utils.TestConfigtation())
            {
                if (Utils.IsAnyPendigFile())
                {
                    string rootPath = config.WorkingDirectory;

                    string[] dirs = Directory.GetDirectories(rootPath);

                    foreach (var folder in dirs)
                    {
                        string[] files = Directory.GetFiles(folder, "*.sql", SearchOption.AllDirectories);

                        var table = new ConsoleTable("Group", "File Name", "State","Owner");
                        foreach (string file in files)
                        {
                            string filename = Path.GetFileNameWithoutExtension(file);

                            FileInfo fileInfo = new FileInfo(file);
                            FileSecurity fileSecurity = fileInfo.GetAccessControl();
                            IdentityReference identityReference = fileSecurity.GetOwner(typeof(NTAccount));

                      
                            var directoryInfo = new DirectoryInfo(file).Parent;
                            if (directoryInfo != null)
                            {
                                string result = directoryInfo.Name;
                                table.AddRow(result, filename, "pending", identityReference.Value);
                            }
                        }
                        table.Write();
                    }

                }
                else
                {
                    Console.WriteLine("No Pending File");
                }
            }
            else
            {
                Console.WriteLine("Configration is missing...");
                Utils.DisplayConfigration();
            }
        }


        private static object Migrate(VuberConfig conf, Migration migration)
        {
               
            if (Utils.TestConfigtation())
            {
                if (Utils.IsAnyPendigFile())
                {

                    
                    
                    var exculudeDirectory = migration.ExculudeDirectory;

                    string[] dirs = Directory.GetDirectories(conf.WorkingDirectory);

                    foreach (var folder in dirs)
                    {
                        string[] files = Directory.GetFiles(folder, "*.sql", SearchOption.AllDirectories);
                        List<string> @group = new List<string>();

                        if (!String.IsNullOrEmpty(migration.MigrateDirectory))
                        {
                            Console.WriteLine(migration.MigrateDirectory);
                        }
                        else
                        {
                            foreach (var item in files)
                            {
                                var directoryInfo = new DirectoryInfo(item).Parent;
                                if (directoryInfo != null && directoryInfo.Name != exculudeDirectory)
                                {
                                    string result = directoryInfo.Name;
                                    @group.Add(result);
                                }
                            }
                        }
                        return 0;

                        foreach (var item in @group.Distinct().ToArray())
                        {
                            string subrootPath = string.Format(@"{0}\{1}", conf.WorkingDirectory, item);
                            string[] subfiles = Directory.GetFiles(subrootPath, "*.sql");
                            string identity = Guid.NewGuid().ToString();
                            foreach (var file in subfiles)
                            {
                                if (File.Exists(file))
                                {
                                    FileInfo fileInfo = new FileInfo(file);
                                    FileSecurity fileSecurity = fileInfo.GetAccessControl();
                                    IdentityReference identityReference = fileSecurity.GetOwner(typeof(NTAccount));
                                    
                                    VuberHistoryLogs history = new VuberHistoryLogs()
                                    {
                                        Execution = DateTime.Now,
                                        ExecutionResult = string.Empty,
                                        FileName = new DirectoryInfo(file).Name,
                                        LogicalGroup = item,
                                        State = "Working",
                                        ExecutionIdentity = identity,
                                        FileContext = File.ReadAllText(file),
                                        UserBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                                        Owner = identityReference.Value
                                    };
                                    Utils.AddHistory(history, conf.ConnectionString);
                                }
                            }
                            string strExecuteSql = string.Empty;
                            List<VuberHistoryLogs> workingRows = Utils.GetWorkingFiles(identity, conf.ConnectionString);
                            foreach (VuberHistoryLogs fl in workingRows)
                            {
                                strExecuteSql += fl.FileContext;
                            }

                            string message = Utils.ExecuteFiles(strExecuteSql, item, conf.ConnectionString);

                            if (message == "Command(s) completed successfully.")
                            {
                                Console.WriteLine(message);
                                Utils.UpdateStates(workingRows, "Success", message, conf.ConnectionString);
                                string moveDirectory = string.Format(@"{0}\{1}", conf.ExecutedDirectory, item);

                                if (Directory.Exists(moveDirectory))
                                    Directory.Delete(moveDirectory, true);
                                Directory.Move(subrootPath, moveDirectory);
                            }
                            else
                            {
                                Console.WriteLine(message);
                                Utils.UpdateStates(workingRows, "Rollback", message, conf.ConnectionString);
                                string rollbackDirectory = string.Format(@"{0}\{1}", conf.RollbackDirectory, identity);
                                Directory.Move(subrootPath, rollbackDirectory);
                                Console.WriteLine("file(s) moved rollback folder as {0}", rollbackDirectory);
                            }

                            var table = new ConsoleTable("Group", "File Name", "State", "Message", "Execution");

                            foreach (VuberHistoryLogs worked in workingRows)
                            {
                                table.AddRow(worked.LogicalGroup, worked.FileName, worked.State,
                                    worked.ExecutionResult.Substring(0, 20), worked.Execution.ToShortDateString());
                            }
                            table.Write();
                        }

                    }

                }
                else
                {
                    Console.WriteLine("No pending file to migration.");
                }
            }
            else
            {
                string configFile = string.Format(@"{0}\config.json", Environment.CurrentDirectory);
                if (File.Exists(configFile))
                {
                    Console.WriteLine("Connection String    {0}", Utils.TestConnection());
                    Console.WriteLine("Working  Directory   {0}", Utils.DirectoryExists(conf.WorkingDirectory));
                    Console.WriteLine("Executed Directory   {0}", Utils.DirectoryExists(conf.ExecutedDirectory));
                    Console.WriteLine("Rollback Directory   {0}", Utils.DirectoryExists(conf.RollbackDirectory));
                }
                else
                {
                    Console.WriteLine("Sometings is wrong to configration.\nPleace use vuber --help config");
                }
            }
            return 0;
        }
    }
}