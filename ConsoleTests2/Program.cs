using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests2
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<IPeriod> tiimeLine = new List<Period>
            {
                new Period { Start = DateTime.Today }
            };
        }
    }

    interface IPeriod
    {
        DateTime Start { get; set; }
    }

    class Period : IPeriod
    {
        public DateTime Start { get; set; }
    }
}
