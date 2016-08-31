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

            Console.WriteLine(config.ConnectionString);
            Console.ReadKey();

        }
    }
}
