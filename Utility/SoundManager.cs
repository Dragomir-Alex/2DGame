using SFML.Audio;
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

        public static void SetCurrentTrack(string trackFilename)
        {
            CurrentTrack = new Music(SoundManager.MUSIC_PATH + trackFilename);
        }
    }
}
