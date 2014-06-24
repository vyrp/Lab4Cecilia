using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    class TakeProcessor : Processor
    {
        protected bool[] overloadedProcessors = new bool[Program.NUM_PROCESSORS];

        public TakeProcessor(Processor[] processors, int processorIndex) : base(processors, processorIndex) { }

        public override void Update(long tick)
        {
            UpdateRunningTime();

            if (endTime == tick)
            {
                Logger.LogTask(tick, processorIndex, currentTask, false);
                if (GetState() == State.Available)
                {
                    timeToAsk = 0;                    
                }
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
            if (timeToAsk == 0 && GetState() == State.Available)
            {
                trials = Program.NUM_TRIALS;
                InitOverloadedProcessors();
                timeToAsk = Program.ASK_TIME;
            }
            if (trials > 0 && GetState() == State.Available)
            {
                SendMessage(tick);
            }
            --timeToAsk;
        }

        public override void Add(long tick, Task task)
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
    }
}
