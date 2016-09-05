using CommandLine;

namespace Vuber.Models
{
    [Verb("config", HelpText = "Configure server connection string and working directory. Test configration or display current configrations.")]
    internal class Configration
    {
        [Option('c', "Connection String", HelpText = "Set SQL server connection string. SAMPLE: vucer config -c Server=yourserver;Database=yourDB;Trusted_Connection=True;")]
        public string ConnectionString { get; set; }

        [Option('d', "Working Directory", HelpText = @"Set working directory that save pending files on your computer. SAMPLE: vucer config -d c:\sql\migration")]
        public string WorkingDirectory { get; set; }

        [Option('m', "Executed Directory", HelpText = @"Set folder that after migration files where to move. SAMPLE: vucer config -m c:\sql\complated")]
        public string ExecutedDirectory { get; set; }

        [Option('r', "Rollback Directory", HelpText = @"Set folder that after failed migration files where to move. SAMPLE: vucer config -m c:\sql\Rollback")]
        public string RollbackDirectory { get; set; }

        [Option('l', "List", HelpText = @"List current configration")]
        public bool List { get; set; }

        [Option('t', "Test", HelpText = @"Test curent configration. Test SQL server, working directoryies.")]
        public bool Test { get; set; }
    }
}