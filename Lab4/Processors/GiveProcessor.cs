using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    class GiveProcessor : IProcessor
    {
        protected IProcessor[] processors;
        protected int processorIndex;
        protected Queue<Task> queue = new Queue<Task>();
        protected Task currentTask = null;
        protected long endTime = 0;
        protected int trials = 0;
        protected bool[] availableProcessors = new bool[Program.NUM_PROCESSORS];
        protected Random random = new Random();

        public GiveProcessor(IProcessor[] processors, int processorIndex)
        {
            this.processors = processors;
            this.processorIndex = processorIndex;
        }

        public void Update(long tick)
        {
            if (endTime == tick)
            {
                if (queue.Count > 0)
                {
                    currentTask = queue.Dequeue();
                    endTime = tick + currentTask.Duration;
                }
                else
                {
                    currentTask = null;
                }
            }
            else if (TaskCount >= 2 && trials > 0)
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
            }
            else
            {
                queue.Enqueue(task);
                
                if (IsOverloaded)
                {
                    trials = Program.NUM_TRIALS;
                    InitAvailableProcessors();
                    SendMessage(tick);
                }
            }
        }

        private void SendMessage(long tick)
        {
            --trials;

            if (availableProcessors.All(p => !p))
            {
                InitAvailableProcessors();
            }

            int rnd;
            do {
                rnd = random.Next(Program.NUM_PROCESSORS);
            } while (!availableProcessors[rnd]);

            Logger.LogMessage(processorIndex, rnd);
            availableProcessors[rnd] = false;
            if (!processors[rnd].IsOverloaded)
            {
                Logger.AcceptMessage(rnd); 
                processors[rnd].Add(tick, TopTask());
                trials = 0;
            }
        }

        private void InitAvailableProcessors()
        {
            for (int i = 0; i < availableProcessors.Length; i++)
            {
                availableProcessors[i] = (i != processorIndex);
            }
        }

        public int TaskCount
        {
            get { return queue.Count + (currentTask == null ? 0 : 1); }
        }

        public Task TopTask()
        {
            return queue.Dequeue();
        }

        public bool IsOverloaded
        {
            get { return IsRunning; }
        }

        public bool IsRunning
        {
            get { return currentTask != null; }
        }
    }
}
