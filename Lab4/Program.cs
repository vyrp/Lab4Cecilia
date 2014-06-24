using System;
using System.Linq;
using System.Reflection;

namespace Lab4
{
    class Program
    {
        public const int NUM_PROCESSORS = 4;
        public const int NUM_TRIALS = 4;
        public const int ASK_TIME = 10 * NUM_TRIALS;

        private static Processor[] processors;

        public static Processor[] Processors
        {
            get { return Array.AsReadOnly(processors).ToArray(); }
        }

        static void Main(string[] args)
        {
            Console.Title = "CES-33 - Lab4 - Balanceamento de carga";

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Lab4.exe <taskNumber> <Heuristic>");
                Console.ReadLine();
                return;
            }

            string heuristic = args[1];
            ConstructorInfo ctor = Type.GetType("Lab4." + heuristic).GetConstructor(new[] { typeof(Processor[]), typeof(int) });

            processors = new Processor[NUM_PROCESSORS];
            for (int i = 0; i < NUM_PROCESSORS; i++)
            {
                processors[i] = (Processor) ctor.Invoke(new object[] { processors, i });
            }
            Console.WriteLine("================ Heuristic: " + heuristic + " ================\n");

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
