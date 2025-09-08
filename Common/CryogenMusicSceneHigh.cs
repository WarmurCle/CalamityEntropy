using CalamityMod.NPCs.Cryogen;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class CryogenMusicSceneHigh : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => (SceneEffectPriority)10;

        public override int NPCType => ModContent.NPCType<Cryogen>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("Cryogen");
        public override int VanillaMusic => MusicID.FrostMoon;
        public override int OtherworldMusic => MusicID.OtherworldlyIce;
    }
}
