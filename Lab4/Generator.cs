using System;
using System.IO;

namespace Lab4
{
    class Generator : IDisposable
    {
        /* Fields */

        private StreamReader sr;
        private Task lastTask;

        /* Properties */

        public bool HasTask
        {
            get
            {
                return !sr.EndOfStream || lastTask != null;
            }
        }

        /* Constructor */

        public Generator(string taskNumber)
        {
            sr = new StreamReader(@"Tasks\tasks" + taskNumber + ".txt");
            Console.WriteLine(sr.ReadLine());
            lastTask = null;
        }

        /* Methods */

        public Task NextTask(long tick)
        {
            if (lastTask == null)
            {
                string[] line = sr.ReadLine().Split(' ');
                lastTask = new Task(int.Parse(line[0]), long.Parse(line[1]), long.Parse(line[2]));
            }

            Task result = null;
            if (lastTask.Timestamp == tick)
            {
                result = lastTask;
                lastTask = null;
            }

            return result;
        }

        public void Dispose()
        {
            sr.Dispose();
        }
    }
}
