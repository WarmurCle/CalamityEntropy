using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers
{
    public class HammerSoundID : ModSystem
    {
        private static string SoundPath => "CalamityEntropy/Assets/Sounds/";
        private SoundStyle HammerStrike1;
        private SoundStyle HammerStrike2;
        private SoundStyle HammerShoot1;
        private SoundStyle HammerShoot2;
        private SoundStyle HammerShoot3;
        public static List<SoundStyle> HammerTypes = [];
        public static List<SoundStyle> HammerStrike = [];
        public override void Load()
        {
            HammerStrike1 = new(SoundPath + "Smash1");
            HammerStrike2 = new(SoundPath + "Smash2");
            HammerShoot1 = new(SoundPath + "HammerShoot1");
            HammerShoot2 = new(SoundPath + "HammerShoot2");
            HammerShoot3 = new(SoundPath + "HammerShoot3");
            HammerTypes =
            [
                HammerShoot1,
                HammerShoot2,
                HammerShoot3
            ];
            HammerStrike =
            [
                HammerStrike1,
                HammerStrike2
            ];
        }
        public override void Unload()
        {
            HammerTypes = null;
            HammerStrike = null;
        }
    }
}