using System;

namespace Lab4
{
    interface IProcessor
    {
        void Update(long tick);
        void Add(long tick, Task task);
        Task TopTask();
        int TaskCount { get; }
        bool IsRunning { get; }
        State GetState();
    }

    enum State { Available, Stable, Overloaded };
}
