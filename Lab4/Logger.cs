using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lab4
{
    class Logger
    {
        static private bool isValid;
        static private StreamWriter sw;

        static Logger()
        {
            isValid = true;
            sw = new StreamWriter("log.txt");
        }

        static public void Log(int fromProcessor, int toProcessor)
        {
            if (!isValid)
            {
                throw new InvalidOperationException("Logger isn\'t valid any more.");
            }
            sw.WriteLine(fromProcessor + " -> " + toProcessor);
        }

        static public void Dump()
        {
            if (!isValid)
            {
                throw new InvalidOperationException("Logger isn\'t valid any more.");
            }
            isValid = false;

            Console.WriteLine(" == Dump ==");
            sw.Dispose();
        }
    }
}
