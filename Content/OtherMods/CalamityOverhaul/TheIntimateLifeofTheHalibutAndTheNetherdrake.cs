using CalamityOverhaul.Content.ADV;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.OtherMods.CalamityOverhaul
{
    /*[ExtendsFromMod("CalamityOverhaul")]
    [JITWhenModsEnabled("CalamityOverhaul")]
    internal class TheIntimateLifeofTheHalibutAndTheNetherdrake : ADVScenarioBase, ILocalizedModType
    {
        public override bool IsLoadingEnabled(Mod mod) {
            return ModLoader.HasMod("CalamityOverhaul");
        }

        protected override void Build() {
            *//*DialogueBoxBase.RegisterPortrait("比目鱼", ADVAsset.HelenADV);
            DialogueBoxBase.RegisterPortrait("始源妖龙", ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/PrimordialWyrmNPC_Head").Value);
            Add("比目鱼", "XXXXXXXXXXXXXXX");
            Add("始源妖龙", "XXXXXXXXXXXXXXX");*//*
        }

        public static void StartThis() {
            if (ModLoader.HasMod("CalamityOverhaul")) {
                ScenarioManager.Start<TheIntimateLifeofTheHalibutAndTheNetherdrake>();
            }
        }
    }*/
}
