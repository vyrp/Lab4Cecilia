using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4
{
    class GiveTakeProcessor : Processor
    {
        protected bool[] otherProcessors = new bool[Program.NUM_PROCESSORS];
        protected State previousState;

        public GiveTakeProcessor(Processor[] processors, int processorIndex)
        {
            this.processors = processors;
            this.processorIndex = processorIndex;
        }

        public override void Update(long tick)
        {
            if (endTime == tick)
            {
                Logger.LogTask(tick, processorIndex, currentTask, false);
                if (GetState() == State.Available)
                {
                    timeToAsk = 1;
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
            State state = GetState();
            if (timeToAsk == 0 && (state == State.Available || state == State.Overloaded))
            {
                trials = Program.NUM_TRIALS;
                InitProcessors();
                timeToAsk = Program.ASK_TIME;
                previousState = state;
            }
            else if (trials > 0 && state == previousState)
            //else if (trials > 0 && (state == State.Available || state == State.Overloaded))
            {
                SendMessage(tick, state);
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
                    timeToAsk = 1;
                }
            }
        }


        private void SendMessage(long tick, State state)
        {
            --trials;
            if (otherProcessors.All(p => !p))
            {
                InitProcessors();
            }

            int rnd;
            do
            {
                rnd = random.Next(Program.NUM_PROCESSORS);
            } while (!otherProcessors[rnd]);
            otherProcessors[rnd] = false;
            bool accepts = false;
            if (state == State.Available)
            {
                if (processors[rnd].GetState() == State.Overloaded) accepts = true;
            }
            else if (state == State.Overloaded)
            {
                if (processors[rnd].GetState() == State.Available) accepts = true;
            }

            Logger.LogMessage(tick, processorIndex, rnd, accepts);
            if (accepts)
            {
                if (state == State.Overloaded)
                {
                    processors[rnd].Add(tick, TopTask());
                    trials = 0;
                }
                else if (state == State.Available)
                {
                    Add(tick, processors[rnd].TopTask());
                    trials = 0;
                }
            }
        }

        private void InitProcessors()
        {
            for (int i = 0; i < otherProcessors.Length; i++)
            {
                otherProcessors[i] = (i != processorIndex);
            }
        }
    }
}
