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
            }
        }

        public int TaskCount
        {
            get
            {
                return queue.Count + (currentTask == null ? 0 : 1);
            }
        }

        public Task TopTask()
        {
            return queue.Dequeue();
        }

        public bool IsOverloaded
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsRunning
        {
            get { return currentTask != null; }
        }
    }
}
