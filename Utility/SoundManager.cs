using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Utility
{
    public static class SoundManager
    {
        public const string SOUNDS_PATH = "./Data/Assets/Sounds/";
        public const string MUSIC_PATH = "./Data/Assets/Music/";

        public static Music CurrentTrack { get; private set; }
        public static Dictionary<string, Sound> Sounds { get; private set; }

        public static void PlayMusic()
        {
            if (CurrentTrack != null && CurrentTrack.Status != SoundStatus.Playing)
            {
                CurrentTrack.Play();
                CurrentTrack.Loop = true;
            }
        }

        public static void SetMusicVolume(uint newVolume)
        {
            CurrentTrack.Volume = newVolume;
        }

        public static void SetSoundVolume(uint newVolume)
        {
            if (Sounds != null)
            {
                foreach (var sound in Sounds)
                {
                    sound.Value.Volume = newVolume;
                }
            }
        }

        public static void SetCurrentTrack(string trackFilename)
        {
            if (CurrentTrack != null)
            {
                CurrentTrack.Dispose();
            }
            CurrentTrack = new Music(MUSIC_PATH + trackFilename);
        }

        public static void LoadSounds()
        {
            Sounds = new();

            Sounds = new Dictionary<string, Sound>
            {
                { "Jump", new Sound(new SoundBuffer(SOUNDS_PATH + "jump.wav")) },
                { "Land", new Sound(new SoundBuffer(SOUNDS_PATH + "land.wav")) },
                { "Shoot", new Sound(new SoundBuffer(SOUNDS_PATH + "shoot.wav")) },
                { "Hurt", new Sound(new SoundBuffer(SOUNDS_PATH + "hurt.wav")) },
                { "Step", new Sound (new SoundBuffer(SOUNDS_PATH + "step.wav")) }
            };
        }

        public static void PlaySound(string soundName) { Sounds[soundName].Play(); }
        public static void PauseSound(string soundName) { Sounds[soundName].Pause(); }
        public static void StopSound(string soundName) { Sounds[soundName].Stop(); }
    }
}
