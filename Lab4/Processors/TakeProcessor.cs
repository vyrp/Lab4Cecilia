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
        protected int timeToAsk = 0;
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
            timeToAsk++;
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
            if (timeToAsk == Program.ASK_TIME && GetState() == State.Available)
            {
                trials = Program.NUM_TRIALS;
                InitOverloadedProcessors();
                timeToAsk = 0;
            }
            if (trials > 0 && GetState() == State.Available)
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
            bool accepts = false;
            if(processors[rnd].GetState() == State.Overloaded) accepts = true;
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

        public State GetState()
        {
            int totalTasks = 0;
            float overloadedInitialLimit;
            foreach (IProcessor processor in processors)
            {
                totalTasks += processor.TaskCount;
            }
            overloadedInitialLimit = Math.Max(((float)totalTasks)/((float)Program.NUM_PROCESSORS), 2.0f); 
            if((float)TaskCount > overloadedInitialLimit)
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
