using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab4
{
    interface IProcessor
    {
        void Update(long tick);
        void Add(Task task);
        bool IsOverloaded { get; }
        bool IsRunning { get; }
    }
}
