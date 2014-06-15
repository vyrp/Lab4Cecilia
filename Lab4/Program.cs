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

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Lab4.exe <taskNumber>");
                Console.ReadLine();
                return;
            }

            IProcessor[] processors = new IProcessor[NUM_PROCESSORS];
            for (int i = 0; i < NUM_PROCESSORS; i++)
            {
                processors[i] = new GiveProcessor(processors, i);
            }

            long tick;
            using (Generator generator = new Generator(args[0]))
            {
                bool running = true;
                for (tick = 0; running; ++tick)
                {
                    if (generator.HasTask)
                    {
                        Task task = generator.NextTask(tick);
                        if (task != null)
                        {
                            processors[task.Processor].Add(tick, task);
                        }
                    }
                    else
                    {
                        running = processors.Any(p => p.IsRunning);
                    }

                    Array.ForEach(processors, p => p.Update(tick));
                }
            }
            
            Logger.Result(tick);
            Console.ReadLine();
        }
    }
}
