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
    public class TransScreenShaderData : ScreenShaderData
    {

        public TransScreenShaderData(string passName)
            : base(passName)
        {
        }


        public override void Update(GameTime gameTime)
        {
        }

        public override void Apply()
        {
            base.Apply();
        }
    }
}
