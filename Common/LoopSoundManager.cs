using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Common
{
    public class LoopSound
    {
        public SoundEffectInstance instance;
        public int timeleft = 2;
        public void setVolume(float v)
        {
            if (!Main.dedServ)
            {
                instance.Volume = v * Main.soundVolume;
            }
        }
        public void setVolume_Dist(Vector2 center, float mindist, float maxdist, float volume = 1)
        {
            if (!Main.dedServ)
            {
                if (CEUtils.getDistance(center, Main.LocalPlayer.Center) > mindist)
                {
                    if (CEUtils.getDistance(center, Main.LocalPlayer.Center) > maxdist)
                    {
                        setVolume(0);
                    }
                    else
                    {
                        setVolume((1 - (float)(CEUtils.getDistance(center, Main.LocalPlayer.Center) - mindist) / (maxdist - mindist)) * volume);
                    }
                }
                else
                {
                    setVolume(volume);
                }
            }
        }
        public LoopSound(SoundEffect sf)
        {
            if (!Main.dedServ)
            {
                instance = sf.CreateInstance();
                instance.IsLooped = true;
            }
        }
        public void play()
        {
            if (!Main.dedServ && LoopSoundManager.sounds != null)
            {
                if (LoopSoundManager.sounds.Count < 5)
                {
                    instance.Play();
                    LoopSoundManager.sounds.Add(this);
                }
            }
        }
        public void stop()
        {
            if (!Main.dedServ)
            {
                instance.Stop();
            }
        }
    }
    public static class LoopSoundManager
    {
        public static List<LoopSound> sounds;
        public static void init()
        {
            sounds = new List<LoopSound>();
        }

        public static void unload()
        {
            if (sounds is not null)
            {
                foreach (var sound in sounds)
                {
                    sound.stop();
                }
            }
            sounds = null;
        }

        public static void update()
        {
            for (int i = sounds.Count - 1; i >= 0; i--)
            {
                if (sounds[i].timeleft-- <= 0)
                {
                    sounds[i].stop();
                    sounds.RemoveAt(i);
                }
            }
        }
    }
}
