using Config.Net;
using System;

namespace Vuber.Configuration
{
    public class Settings: SettingsContainer
    {
        public readonly Option<string> ConnectionString;
        public readonly Option<string> WorkingDirectory;
        public readonly Option<string> ExecutedDirectory;
        public readonly Option<string> RollbackDirectory;        

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.UseJsonFile(string.Format(@"{0}\config.json", AppDomain.CurrentDomain.BaseDirectory));
        }
    }
}
