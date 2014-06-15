using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    class TakeProcessor : IProcessor
    {
        protected IProcessor[] processors;
        protected int processorIndex;

        public TakeProcessor(IProcessor[] processors, int processorIndex)
        {
            this.processors = processors;
            this.processorIndex = processorIndex;
        }

        public void Update(long tick)
        {
            throw new NotImplementedException();
        }

        public void Add(long tick, Task task)
        {
            throw new NotImplementedException();
        }
        
        public Task TopTask()
        {
            throw new NotImplementedException();
        }

        public int TaskCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsOverloaded
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsRunning
        {
            get { return false; }
        }
    }
}
