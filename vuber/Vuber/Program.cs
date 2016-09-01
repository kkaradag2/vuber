using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                            .WithParsed<Configration>(opts => Configration(opts))
                            .WithParsed<info>(opts => info(opts))
                            .WithParsed<migration>(opts => migrate(opts))
                            .WithNotParsed(errs => Console.WriteLine(""));


        }

        private static object migrate(migration opts)
        {
            throw new NotImplementedException();
        }

        private static object info(info opts)
        {
            throw new NotImplementedException();
        }

        private static void Configration(Configration opts)
        {
            Console.WriteLine("test");
        }
    }
}
