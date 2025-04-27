using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ImpactParticle : EParticle
    {
        public override Texture2D texture => Utilities.Util.getExtraTex("Impact2");
        public override void onSpawn()
        {
            base.onSpawn();
            this.timeLeft = 120;
        }public float sadd = 0.1f;
        public override void update()
        {
            base.update();
            this.scale += sadd;
            sadd *= 0.9f;
            this.color *= 0.96f;

        }
    }
}
