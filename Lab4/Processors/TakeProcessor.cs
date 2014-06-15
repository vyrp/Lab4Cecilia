using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class TakeProcessor : IProcessor
    {
        protected IProcessor[] processors;

        public TakeProcessor(IProcessor[] processors)
        {
            this.processors = processors;
        }

        public void Update(long tick)
        {
            //throw new NotImplementedException();
        }

        public void Add(Task task)
        {
            throw new NotImplementedException();
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
