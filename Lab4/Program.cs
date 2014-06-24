﻿using Lab4.Processors;
using System;
using System.Linq;

namespace Lab4
{
    class Program
    {
        public const int NUM_PROCESSORS = 4;
        public const int NUM_TRIALS = 4;
        public const int ASK_TIME = 2 * NUM_PROCESSORS;

        static void Main(string[] args)
        {
            Console.Title = "CES-33 - Lab4 - Balanceamento de carga";

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Lab4.exe <taskNumber>");
                Console.ReadLine();
                return;
            }

            Processor[] processors = new Processor[NUM_PROCESSORS];
            for (int i = 0; i < NUM_PROCESSORS; i++)
            {
                processors[i] = new GiveTakeProcessor(processors, i);
            }
            Console.WriteLine("Heuristic: GiveProcessor\n");

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

                    Array.ForEach(processors, p => p.Update(tick));

                    if (!generator.HasTask)
                    {
                        running = processors.Any(p => p.IsRunning);
                    }
                }
            }
            
            Logger.Result(tick);
            Console.ReadLine();
        }
    }
}
