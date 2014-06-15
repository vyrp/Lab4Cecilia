using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class GiveProcessor : IProcessor
    {
        protected IProcessor[] processors;

        public GiveProcessor(IProcessor[] processors)
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
