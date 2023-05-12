using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ShadowsOfTomorrow
{
    public class MusicManager
    {
        readonly List<SoundEffect> playedSoundEffects = new();
        readonly Random random = new ();
        private readonly List<SoundEffect> fastWalk;
        private readonly List<SoundEffect> slowWalk;
        Song activeSong = null;

        public static float MusicVolume { get => MediaPlayer.Volume; set => MediaPlayer.Volume = value; }

        public float SoundEffectsVolume { get; set; } = 0.1f;


        public MusicManager(Game1 game1) 
        {
            MusicVolume = 0.2f;
            fastWalk = new()
            {
                game1.Content.Load<SoundEffect>("Music/FastStep1"),
                game1.Content.Load<SoundEffect>("Music/FastStep2"),
                game1.Content.Load<SoundEffect>("Music/FastStep3"),
            };

            slowWalk = new()
            {
                game1.Content.Load<SoundEffect>("Music/SlowStep1"),
                game1.Content.Load<SoundEffect>("Music/SlowStep2"),
                game1.Content.Load<SoundEffect>("Music/SlowStep3"),
            };
        }

        public void Play(SoundEffect soundEffect)
        {
            if (!playedSoundEffects.Contains(soundEffect))
            {
                soundEffect.Play(volume: SoundEffectsVolume, pitch: 0, pan: 0);
                playedSoundEffects.Add(soundEffect);
            }
        }
        public void Play(SoundEffect soundEffect, bool repetable)
        {
            if (repetable)
                soundEffect.Play(volume: SoundEffectsVolume, pitch: 0, pan: 0);
        }

        double time = 0;
        const double interval = 0.6;

        public void Play(bool isFast, GameTime gameTime)
        {
            if (time + interval > gameTime.TotalGameTime.TotalSeconds)
                return;
            time = gameTime.TotalGameTime.TotalSeconds;

            int i = random.Next(0, 3);
            if (isFast)
                fastWalk[i].Play(volume: SoundEffectsVolume, pitch: 0, pan: 0);
            else
                slowWalk[i].Play(volume: SoundEffectsVolume, pitch: 0, pan: 0);
        }

        public void Play(Song song)
        {
            if (song == null || activeSong == song)
                return;
            if (activeSong != song)
                MediaPlayer.Stop();
            activeSong = song;
            MediaPlayer.Play(song);
        }

        internal void Stop()
        {
            activeSong = null;
            MediaPlayer.Stop();
        }

        internal void Reset()
        {
            activeSong = null;
        }
    }
}
