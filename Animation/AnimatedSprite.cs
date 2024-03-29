﻿using System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;
using System.Diagnostics;

namespace _2DGame.Animation
{
    public class AnimatedSprite : Sprite
    {
        private readonly RenderTarget renderTarget;
        private RenderStates renderStates;
        private readonly int frameWidth;
        private readonly int frameHeight;
        private readonly int firstFrame;
        private readonly int lastFrame;
        private int currentFrame;
        private float interval, clock;
        private bool isAnimated, isLooped;

        public AnimatedSprite(Texture texture, int frameWidth, int frameHeight, int FPS, RenderTarget renderTarget, RenderStates renderStates, int firstFrame = 0, int lastFrame = 0, bool isAnimated = true, bool isLooped = true) : base(texture)
        {
            this.renderTarget = renderTarget;
            this.renderStates = renderStates;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.firstFrame = firstFrame;
            this.lastFrame = lastFrame;
            this.isAnimated = isAnimated;
            this.isLooped = isLooped;

            clock = 0;
            interval = 1f / FPS;
            currentFrame = firstFrame;

            TextureRect = GetFramePosition(currentFrame);
            Origin = new SFML.System.Vector2f(frameWidth / 2, frameHeight / 2);
        }

        public AnimatedSprite(AnimatedSprite animatedSprite) : base(animatedSprite.Texture)
        {
            renderTarget = animatedSprite.renderTarget;
            renderStates = animatedSprite.renderStates;
            frameWidth = animatedSprite.frameWidth;
            frameHeight = animatedSprite.frameHeight;
            currentFrame = animatedSprite.currentFrame;
            firstFrame = animatedSprite.firstFrame;
            lastFrame = animatedSprite.lastFrame;
            interval = animatedSprite.interval;
            clock = animatedSprite.clock;
            isAnimated = animatedSprite.isAnimated;
            isLooped = animatedSprite.isLooped;

            clock = 0;
            currentFrame = firstFrame;

            TextureRect = GetFramePosition(currentFrame);
            Origin = new SFML.System.Vector2f(frameWidth / 2, frameHeight / 2);
        }

        public IntRect GetFramePosition(int frame)
        {
            int count = (int)Texture.Size.X / frameWidth;
            int xPos = frame % count;
            int yPos = frame / count;

            IntRect Position = new IntRect(frameWidth * xPos, frameHeight * yPos, frameWidth, frameHeight);
            return Position;
        }

        public void Update(float deltaTime, bool flipped)
        {
            clock += deltaTime;

            if (isAnimated && clock >= interval)
            {
                TextureRect = GetFramePosition(currentFrame);
                if (currentFrame < lastFrame)
                    currentFrame++;
                else
                    currentFrame = firstFrame;
                clock = 0;
            }

            if (!isLooped && currentFrame == lastFrame)
            {
                isAnimated = false;
            }

            if (flipped) Scale = new SFML.System.Vector2f(-1f, 1f);
            else Scale = new SFML.System.Vector2f(1f, 1f);

            Draw(renderTarget, renderStates);
        }

        public void Play()
        {
            isAnimated = true;
        }

        public void PlayWithoutLoop()
        {
            if (isLooped) { isAnimated = true; }
            else if (currentFrame == lastFrame) { isAnimated = false; return; }
            isAnimated = true;
        }

        public void Pause()
        {
            isAnimated = false;
        }

        public void Reset()
        {
            isAnimated = false;
            currentFrame = 0;
            TextureRect = new IntRect(0, 0, frameWidth, frameHeight);
        }

        public void Restart()
        {
            isAnimated = false;
            currentFrame = firstFrame;
            TextureRect = GetFramePosition(currentFrame);
        }

        public void SetFrame(int Frame)
        {
            currentFrame = Frame;
            isAnimated = true;
            isLooped = false;
        }

        public int GetCurrentFrame() { return currentFrame; }
        public int GetLastFrame() { return lastFrame; }
        public bool IsAnimated() { return isAnimated; }
        public bool IsFinished() { return (currentFrame == lastFrame && !isLooped); }
        public bool IsLoopFinished() { return (currentFrame == lastFrame && isLooped); }
        public void SetFPS(int FPS) { interval = 1f / FPS; }

        public bool NotEqual(AnimatedSprite animatedSprite)
        {
            return ( Texture != animatedSprite.Texture ||
                renderTarget != animatedSprite.renderTarget ||
                frameWidth != animatedSprite.frameWidth ||
                frameHeight != animatedSprite.frameHeight ||
                firstFrame != animatedSprite.firstFrame ||
                lastFrame != animatedSprite.lastFrame ||
                interval != animatedSprite.interval ||
                isLooped != animatedSprite.isLooped);
        }
    }
}