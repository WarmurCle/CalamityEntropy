using CalamityMod.NPCs.Cryogen;
using CalamityMod.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class CryogenMusicSceneHigh : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => (SceneEffectPriority)10;

        public override int NPCType => ModContent.NPCType<Cryogen>();
        public override int? MusicModMusic => ModContent.GetInstance<CalamityMod.CalamityMod>().GetMusicFromMusicMod("Cryogen");
        public override int VanillaMusic => MusicID.FrostMoon;
        public override int OtherworldMusic => MusicID.OtherworldlyIce;
    }
}
