using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    class GiveProcessor : Processor
    {

        protected bool[] availableProcessors = new bool[Program.NUM_PROCESSORS];

        public GiveProcessor(Processor[] processors, int processorIndex)
        {
            this.processors = processors;
            this.processorIndex = processorIndex;
        }

        public override void Update(long tick)
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
            if (trials > 0 && GetState() == State.Overloaded)
            {
                SendMessage(tick);
            }
            if (timeToAsk == 0 && GetState() == State.Overloaded)
            {
                trials = Program.NUM_TRIALS;
                InitAvailableProcessors();
                timeToAsk = Program.ASK_TIME;
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
                if (GetState() == State.Overloaded)
                {
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
            do
            {
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
    }
}
