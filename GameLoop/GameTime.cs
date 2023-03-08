using System;

namespace _2DGame
{
    public class GameTime
    {
        private float deltaTime;
        public float DeltaTime
        {
            get { return deltaTime * TimeScale; }
            set { deltaTime = value; }
        }

        public float TimeScale { get; set; }
        public float TotalTimeElapsed { get; private set; }

        public GameTime()
        {
            DeltaTime = 0f;
            TimeScale = 1f;
        }

        public void Update(float deltaTime, float totalTimeElapsed)
        {
            DeltaTime = deltaTime;
            TotalTimeElapsed = totalTimeElapsed;
        }
    }
}
