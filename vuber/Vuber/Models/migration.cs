using CommandLine;

namespace Vuber.Models
{
    [Verb("migrate", HelpText = "Migrate pending files to database.")]
    internal class Migration
    {
        [Option('m', "Migrate Special Folder", HelpText = "Migrate only entered folder. Sample: vuber migrate -m DPL-100")]
        public string MigrateDirectory { get; set; }

        [Option('e', "Exculute Special Folder to migrate", HelpText = "Exclude declareted folder to migrate. Sample: vuver migrate -e DPL-105")]
        public string ExculudeDirectory { get; set; }

    }
}