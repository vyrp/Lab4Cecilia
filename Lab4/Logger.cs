//#define FILE

using System;
using System.Linq;
using System.Collections.Generic;
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
        static private int[] sentMessages = new int[Program.NUM_PROCESSORS];
        static private int[] receivedMessages = new int[Program.NUM_PROCESSORS];

        /* Constructor */

        static Logger()
        {
            isValid = true;
            numMessages = 0;

            for (int i = 0; i < Program.NUM_PROCESSORS; i++)
            {
                sentMessages[i] = 0;
                receivedMessages[i] = 0;
            }

#if FILE
            sw = new StreamWriter("log.txt");
#else
            sw = new StreamWriter(Console.OpenStandardOutput());
            sw.AutoFlush = true;
            Console.SetOut(sw);
#endif
        }

        /* Methods */

        public static void LogMessage(long tick, int fromProcessor, int toProcessor, bool accepted)
        {
            CheckValid();
            sw.WriteLine(string.Format("[{0:0000}] {1} -> {2}{3}", tick, fromProcessor, toProcessor, accepted ? "*" : " "));
            ++numMessages;
            ++sentMessages[fromProcessor];
            ++receivedMessages[toProcessor];
            if (accepted)
            {
                ++numAccepted;
            }
        }

        public static void LogTask(long tick, int processor, Task task, bool begin)
        {
            CheckValid();
            sw.WriteLine(string.Format(
                "[{0:0000}] {1} {2}({3}, {4}){5}",
                tick, processor, begin ? "<< " : "", task.Timestamp, task.Duration, begin ? "" : " >>"
            ));
        }

        public static void Result(long ticks)
        {
            CheckValid();
            isValid = false;

            Console.WriteLine("\n == Statistics ==\n");
            Console.WriteLine("    Ticks: " + ticks);
            Console.WriteLine("    Messages: " + numMessages);
            Console.WriteLine("    Accepted messages: " + numAccepted);

            Console.WriteLine("\n+-----------+-----------+---------------+");
            Console.WriteLine("| Processor | Sent msgs | Received msgs |");
            Console.WriteLine("+-----------+-----------+---------------+");
            for (int i = 0; i < Program.NUM_PROCESSORS; i++)
            {
                Console.WriteLine(string.Format("|     {0}     | {1,9} | {2,13} |", i, sentMessages[i], receivedMessages[i]));
                Console.WriteLine("+-----------+-----------+---------------+");
            }

            int sentMax = sentMessages.Max();
            int receivedMax = receivedMessages.Max();
            int sentMin = sentMessages.Min();
            int receivedMin = receivedMessages.Min();

            Console.WriteLine("\n    Maximum sent: " + sentMax + "; by " + string.Join(", ", sentMessages.FindAll(sentMax)));
            Console.WriteLine("    Minimum sent: " + sentMin + "; by " + string.Join(", ", sentMessages.FindAll(sentMin))); 
            Console.WriteLine("    Maximum received: " + receivedMax + "; by " + string.Join(", ", receivedMessages.FindAll(receivedMax)));
            Console.WriteLine("    Minimum received: " + receivedMin + "; by " + string.Join(", ", receivedMessages.FindAll(receivedMin)));

            sw.Dispose();
        }

        private static void CheckValid()
        {
            if (!isValid)
            {
                throw new InvalidOperationException(@"Logger isn't valid any more.");
            }
        }
    }

    static class ArrayExtesions
    {
        public static List<int> FindAll(this int[] array, int n)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == n)
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}
