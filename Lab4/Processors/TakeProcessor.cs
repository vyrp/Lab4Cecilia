using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    class TakeProcessor : IProcessor
    {
        protected IProcessor[] processors;
        protected int processorIndex;
        protected long endTime = -1;
        protected Queue<Task> queue = new Queue<Task>();
        protected Task currentTask = null;
        protected int trials = 0;
        protected bool[] overloadedProcessors = new bool[Program.NUM_PROCESSORS];
        protected Random random = new Random();

        public TakeProcessor(IProcessor[] processors, int processorIndex)
        {
            this.processors = processors;
            this.processorIndex = processorIndex;
        }

        public void Update(long tick)
        {
            if (endTime == tick)
            {
                Logger.LogTask(tick, processorIndex, currentTask, false);
                if (queue.Count > 0)
                {
                    currentTask = queue.Dequeue();
                    endTime = tick + currentTask.Duration;
                    Logger.LogTask(tick, processorIndex, currentTask, true);
                }
                else
                {
                    currentTask = null;
                }
            }
            if (IsAvailable && trials == 0)
            {
                trials = Program.NUM_TRIALS;
                InitOverloadedProcessors();
            }
            if (IsAvailable && trials > 0)
            {
                SendMessage(tick);
            }
        }

        public void Add(long tick, Task task)
        {
            if (currentTask == null)
            {
                currentTask = task;
                endTime = tick + currentTask.Duration;
                Logger.LogTask(tick, processorIndex, currentTask, true);
            }
            else
            {
                queue.Enqueue(task);
            }
        }

        private void SendMessage(long tick)
        {
            --trials;

            if (overloadedProcessors.All(p => !p))
            {
                InitOverloadedProcessors();
            }

            int rnd;
            do
            {
                rnd = random.Next(Program.NUM_PROCESSORS);
            } while (!overloadedProcessors[rnd]);
            overloadedProcessors[rnd] = false;
            bool accepts = processors[rnd].IsOverloaded;
            Logger.LogMessage(tick, processorIndex, rnd, accepts);
            if (accepts)
            {
                Add(tick, processors[rnd].TopTask());
                trials = 0;
            }
        }

        private void InitOverloadedProcessors()
        {
            for (int i = 0; i < overloadedProcessors.Length; i++)
            {
                overloadedProcessors[i] = (i != processorIndex);
            }
        }

        public Task TopTask()
        {
            return queue.Dequeue();
        }

        public int TaskCount
        {
            get { return queue.Count + (currentTask == null ? 0 : 1); }   
        }

        public bool IsOverloaded
        {
            get { return TaskCount >= 2; }
        }

        public bool IsAvailable
        {
            get { return !IsRunning; }
        }

        public bool IsRunning
        {
            get { return currentTask != null; }
        }
    }
}
