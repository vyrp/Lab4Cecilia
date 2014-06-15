using System;
using System.Linq;

namespace Lab4
{
    class Program
    {
        const int NUM_PROCESSORS = 4;

        static void Main(string[] args)
        {
            Console.Title = "CES-33 - Lab4 - Balanceamento de carga";

            IProcessor[] processors = new IProcessor[NUM_PROCESSORS];
            for (int i = 0; i < NUM_PROCESSORS; i++)
            {
                processors[i] = new GiveProcessor(processors);
            }

            using (Generator generator = new Generator())
            {
                bool running = true;
                for (long tick = 0; running; ++tick)
                {
                    if (generator.HasTask)
                    {
                        Task task = generator.NextTask(tick);
                        if (task != null)
                        {
                            //processors[task.Processor].Add(task);
                            Console.WriteLine(task);
                        }
                    }
                    else
                    {
                        running = processors.Any(p => p.IsRunning);
                    }

                    Array.ForEach(processors, p => p.Update(tick));
                }
            }

            Logger.Dump();
            Console.ReadLine();
        }
    }
}
