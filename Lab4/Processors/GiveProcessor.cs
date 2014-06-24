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
        protected long endTime = -1;
        protected int timeToAsk = Program.ASK_TIME;
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
            else if (trials > 0 && GetState() == State.Overloaded)
            {
                SendMessage(tick);
            }
            else if (timeToAsk == 0 && GetState() == State.Overloaded)
            {
                trials = Program.NUM_TRIALS;
                InitAvailableProcessors();
                timeToAsk = Program.ASK_TIME;
            }
            --timeToAsk;
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
                if (GetState() == State.Overloaded)
                {
                    trials = Program.NUM_TRIALS;
                    InitAvailableProcessors();
                    timeToAsk = 0;
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

            availableProcessors[rnd] = false;
            bool accepts = false;
            if (processors[rnd].GetState() == State.Available) accepts = true;
            Logger.LogMessage(tick, processorIndex, rnd, accepts); 
            if (accepts)
            {
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

        public State GetState()
        {
            int totalTasks = 0;
            float overloadedInitialLimit;
            foreach (IProcessor processor in processors)
            {
                totalTasks += processor.TaskCount;
            }
            overloadedInitialLimit = Math.Max(((float)totalTasks) / ((float)Program.NUM_PROCESSORS), 2.0f);
            if ((float)TaskCount > overloadedInitialLimit)
            {
                return State.Overloaded;
            }
            else if ((float)TaskCount <= overloadedInitialLimit / 2.0f)
            {
                return State.Available;
            }
            else
            {
                return State.Stable;
            }
        }


        public bool IsRunning
        {
            get { return currentTask != null; }
        }
    }
}
