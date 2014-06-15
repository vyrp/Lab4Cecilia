using System;

namespace Lab4
{
    class Task
    {
        /* Fields */

        private readonly int processor;
        private readonly long timestamp;
        private readonly long duration;

        /* Properties */

        public int Processor
        {
            get { return processor; }
        }

        public long Timestamp
        {
            get { return timestamp; }
        }

        public long Duration
        {
            get { return duration; }
        }
        
        /* Constructor */

        public Task(int processor, long timestamp, long duration)
        {
            this.processor = processor;
            this.timestamp = timestamp;
            this.duration = duration;
        }

        /* Methods */

        public override string ToString()
        {
            return processor + " " + timestamp + " " + duration;
        }
    }
}
