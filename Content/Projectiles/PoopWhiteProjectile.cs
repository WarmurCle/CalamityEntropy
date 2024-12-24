using System;
using System.Collections.Generic;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopWhiteProjectile : PoopProj
    {
        public override void AI()
        {
            base.AI();
            foreach(Player p in Main.ActivePlayers)
            {
                if (Util.Util.getDistance(Projectile.Center, p.Center) < 64 * Projectile.scale * 2f)
                {
                    p.Entropy().holyGroundTime = 2;
                }
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix); ;

            Texture2D light = Util.Util.getExtraTex("godhead");
            Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, Color.White * 0.3f, 0, light.Size() / 2, 4 * Projectile.scale, SpriteEffects.None, 0); ;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix); ;

        }

        public override int dustType => DustID.HallowedTorch;
    }

}