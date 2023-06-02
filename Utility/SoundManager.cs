using _2DGame.Entities.Players;
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

        public const float MIN_DISTANCE = 300f;
        public const float ATTENUATION = 5f;

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
            Listener.GlobalVolume = newVolume;
            if (Sounds != null)
            {
                foreach (var sound in Sounds)
                {
                    sound.Value.Volume = newVolume;
                }
            }
        }

        public static void SetListenerPosition(Player player)
        {
            Listener.Position = new SFML.System.Vector3f(player.Position.X, 0f, player.Position.Y);
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
                { "Jump", new Sound(new SoundBuffer(SOUNDS_PATH + "jump.wav")) { RelativeToListener = true } },
                { "Land", new Sound(new SoundBuffer(SOUNDS_PATH + "land.wav")) { RelativeToListener = true } },
                { "Shoot", new Sound(new SoundBuffer(SOUNDS_PATH + "shoot.wav")) { RelativeToListener = true } },
                { "Poof", new Sound(new SoundBuffer(SOUNDS_PATH + "poof.wav")) { RelativeToListener = true } },
                { "Hurt", new Sound(new SoundBuffer(SOUNDS_PATH + "hurt.wav")) { RelativeToListener = true } },
                { "Step", new Sound(new SoundBuffer(SOUNDS_PATH + "step.wav")) { RelativeToListener = true } },
                { "Low Step", new Sound(new SoundBuffer(SOUNDS_PATH + "step.wav")) { Pitch = 0.5f, RelativeToListener = true } },
                { "Collect Gem", new Sound (new SoundBuffer(SOUNDS_PATH + "collect_gem.wav")) { RelativeToListener = true } },
                { "Hiss", new Sound (new SoundBuffer(SOUNDS_PATH + "hiss.wav")) { RelativeToListener = true } },
                { "Low Hiss", new Sound (new SoundBuffer(SOUNDS_PATH + "hiss.wav")) { Pitch = 0.7f, RelativeToListener = true } },
                { "Flap", new Sound (new SoundBuffer(SOUNDS_PATH + "flap.wav")) { RelativeToListener = true } },
                { "Bite", new Sound (new SoundBuffer(SOUNDS_PATH + "bite.wav")) { RelativeToListener = true } },
                { "Heal", new Sound (new SoundBuffer(SOUNDS_PATH + "heal.wav")) { RelativeToListener = true } },
                { "Slap", new Sound (new SoundBuffer(SOUNDS_PATH + "slap.wav")) { RelativeToListener = true } },
                { "Growl", new Sound (new SoundBuffer(SOUNDS_PATH + "growl.wav")) { RelativeToListener = true } },
                { "Low Growl", new Sound (new SoundBuffer(SOUNDS_PATH + "growl.wav")) { Pitch = 0.7f, RelativeToListener = true} }
            };
        }

        public static void PlaySound(string soundName) { Sounds[soundName].Play(); }
        public static void Play2DSound(string soundName, float xPosition, float yPosition)
        {
            Sound _2DSound = new Sound(Sounds[soundName]);
            _2DSound.Position = new SFML.System.Vector3f(xPosition, 0f, yPosition);
            _2DSound.RelativeToListener = false;
            _2DSound.MinDistance = MIN_DISTANCE;
            _2DSound.Attenuation = ATTENUATION;
            _2DSound.Play();
        }

        public static void PlaySoundOnce(string soundName) { if (Sounds[soundName].Status == SoundStatus.Stopped) Sounds[soundName].Play(); }
        public static void PauseSound(string soundName) { Sounds[soundName].Pause(); }
        public static void StopSound(string soundName) { Sounds[soundName].Stop(); }
    }
}
