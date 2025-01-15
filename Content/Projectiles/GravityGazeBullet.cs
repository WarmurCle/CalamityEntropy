using CalamityEntropy.Common;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class GravityGazeBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        float alpha = 0.7f;
        public override void AI(){
            if (s)
            {
                tail = Projectile.Center;
                Projectile.ai[1] = Main.rand.Next(0, 1024);
                s = false;
            }
            Projectile.velocity *= 0.98f;
            tail = tail + (Projectile.Center - tail) * 0.05f;
            if(Projectile.timeLeft < 30)
            {
                alpha -= 0.6f / 30f;
            }
        }

        Vector2 tail;
        public bool s = true;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Asset<Texture2D> texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Enchanted", AssetRequestMode.ImmediateLoad);
            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/Transform", AssetRequestMode.ImmediateLoad).Value;
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.Parameters["color"].SetValue(new Color(160, 80, 255, 255).ToVector4());
            shader.Parameters["strength"].SetValue(1.6f);
            shader.Parameters["baseColor"].SetValue((Color.Lerp(new Color(104 * 0.6f, 127 * 0.6f, 255 * 0.6f), new Color(238 * 0.6f, 244 * 0.6f, 213 * 0.6f), (float)Math.Cos(Main.GlobalTimeWrappedHourly * 6f + Projectile.ai[1])) * alpha).ToVector4());
            shader.CurrentTechnique.Passes["EnchantedPass"].Apply();
            Main.instance.GraphicsDevice.Textures[1] = texture.Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), new Vector2(tex.Width, tex.Height / 2), new Vector2(Util.Util.getDistance(Projectile.Center, tail) / tex.Width, 0.26f), SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
    

}