using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4
{
    abstract class Processor
    {
        protected Processor[] processors;
        protected int processorIndex;
        protected long endTime = -1;
        protected int timeToAsk = Program.ASK_TIME;
        protected Queue<Task> queue = new Queue<Task>();
        protected Task currentTask = null;
        protected int trials = 0;
        protected Random random = new Random();
        
        public abstract void Update(long tick);
        public abstract void Add(long tick, Task task);

        public bool IsRunning
        {
            get { return currentTask != null; }
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
            foreach (Processor processor in processors)
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
    }

    enum State { Available, Stable, Overloaded };
}
