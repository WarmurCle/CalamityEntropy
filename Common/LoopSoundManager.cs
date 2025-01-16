using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalamityEntropy.Common
{
    public class LoopSound {
        public SoundEffectInstance instance;
        public int timeleft = 2;
        public LoopSound(SoundEffect sf)
        {
            instance = sf.CreateInstance();
            instance.IsLooped = true;
        }
        public void play()
        {
            if (LoopSoundManager.sounds.Count < 4)
            {
                instance.Play();
                LoopSoundManager.sounds.Add(this);
            }
        }
        public void stop()
        {
            instance.Stop();
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
            foreach (var sound in sounds)
            {
                sound.stop();
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
