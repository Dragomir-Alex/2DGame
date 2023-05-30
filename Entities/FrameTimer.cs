using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Entities
{
    public class FrameTimer
    {
        public uint StartTime { get; private set; }
        public uint CurrentTime { get; private set; }
        public bool IsRunning { get; private set; }

        public FrameTimer(uint startTime)
        {
            this.StartTime = startTime;
            Reset();
        }

        public void Start() { IsRunning = true; }

        public void Update()
        {
            if (IsRunning)
            {
                CurrentTime--;
                if (CurrentTime == 0)
                {
                    IsRunning = false;
                }
            }
        }

        public void Reset()
        {
            CurrentTime = StartTime;
            IsRunning = false;
        }

        public bool IsFinished() { return CurrentTime == 0; }
    }
}
