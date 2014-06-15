//#define FILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lab4
{
    class Logger
    {
        /* Fields */

        static private bool isValid;
        static private int numMessages;
        static private int numAccepted;
        static private StreamWriter sw;

        static Logger()
        {
            isValid = true;
            numMessages = 0;
#if FILE
            sw = new StreamWriter("log.txt");
#else
            sw = new StreamWriter(Console.OpenStandardOutput());
            sw.AutoFlush = true;
            Console.SetOut(sw);
#endif
        }

        static public void LogMessage(int fromProcessor, int toProcessor)
        {
            if (!isValid)
            {
                throw new InvalidOperationException("Logger isn\'t valid any more.");
            }
            sw.WriteLine(fromProcessor + " -> " + toProcessor);
            ++numMessages;
        }

        static public void AcceptMessage(int processor)
        {
            if (!isValid)
            {
                throw new InvalidOperationException("Logger isn\'t valid any more.");
            }
            sw.WriteLine(">> " + processor);
            ++numAccepted;
        }

        static public void Result()
        {
            if (!isValid)
            {
                throw new InvalidOperationException("Logger isn\'t valid any more.");
            }
            isValid = false;

            Console.WriteLine("\nResult:\n\tTotal number of messages: " + numMessages + "\n\tNumber of accepted messages: " + numAccepted);
            sw.Dispose();
        }
    }
}
