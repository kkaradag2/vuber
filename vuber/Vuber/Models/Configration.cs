using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vuber.Models
{
      [Verb("config", HelpText = "Configure server connection string and working directory. Test configration or display current configrations.")]
    class Configration
    {
        [Option('c', "Connection", HelpText = "Set SQL server connection string. SAMPLE: vucer config -c Server=yourserver;Database=yourDB;Trusted_Connection=True;")]
        public string Server { get; set; }

        [Option('d', "Directory", HelpText = @"Set working directory that save pending files on your computer. SAMPLE: vucer config -d c:\sql\migration")]
        public string Directory { get; set; }

        [Option('m', "Move", HelpText = @"Set folder that after migration files where to move. SAMPLE: vucer config -m c:\sql\complated")]
        public string Move { get; set; }

        [Option('r', "Rollback", HelpText = @"Set folder that after failed migration files where to move. SAMPLE: vucer config -m c:\sql\Rollback")]
        public string Rollback { get; set; }

        [Option('l', "List", HelpText = @"List current configration")]
        public bool list { get; set; }

        [Option('t', "Test", HelpText = @"Test curent configration. Test SQL server, working directoryies.")]
        public bool test { get; set; }
    }
}
