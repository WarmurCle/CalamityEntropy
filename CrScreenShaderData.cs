using System;
using CalamityEntropy.NPCs.Cruiser;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public class CrScreenShaderData : ScreenShaderData
    {
        private int SCalIndex;

        public CrScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateSCalIndex()
        {
            int SCalType = ModContent.NPCType<CruiserHead>();
            if (SCalIndex >= 0 && Main.npc[SCalIndex].active && Main.npc[SCalIndex].type == SCalType)
            {
                return;
            }
            SCalIndex = NPC.FindFirstNPC(SCalType);
        }

        public override void Update(GameTime gameTime)
        {
            if (SCalIndex == -1)
            {
                UpdateSCalIndex();
                if (SCalIndex == -1)
                    Filters.Scene["CalamityEntropy:Cruiser"].Deactivate(Array.Empty<object>());
            }
        }

        public override void Apply()
        {
            UpdateSCalIndex();
            if (SCalIndex != -1)
            {
                UseTargetPosition(Main.npc[SCalIndex].Center);
                Filters.Scene["CalamityMod:SupremeCalamitas"].GetShader().UseColor(1f, 1f, 1f);
            }
            base.Apply();
        }
    }
}
