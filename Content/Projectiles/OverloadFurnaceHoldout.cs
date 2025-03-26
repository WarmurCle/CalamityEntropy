using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class OverloadFurnaceHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        int maxTime;
        public override void AI()
        {
            Player owner = Projectile.owner.ToPlayer();

            if (!owner.channel)
            {
                if (Projectile.ai[0] > 16)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 30, Projectile.velocity.SafeNormalize(Vector2.Zero) * 16, ModContent.ProjectileType<LightningBall>(), (int)(Projectile.damage * (Projectile.ai[0] >= maxTime ? 5f : 1)), Projectile.knockBack, Projectile.owner, 0, (Projectile.ai[0] >= maxTime ? 1 : 0));
                    }
                }
                Projectile.Kill();
                return;
            }

            maxTime = 60;

            if (Projectile.ai[0] < maxTime)
            {
                Projectile.ai[0] += 1 * owner.GetAttackSpeed(DamageClass.Magic) * (1 + owner.Entropy().WeaponBoost * 0.6f);
                if (Projectile.ai[0] >= maxTime)
                {
                    float a = 0;
                    for (int i = 0; i < 36; i++)
                    {
                        a += MathHelper.ToRadians(10);
                        SoundStyle s = SoundID.DD2_BetsyFireballShot;
                        SoundEngine.PlaySound(s, Projectile.Center);
                        Dust.NewDust(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 34 * Projectile.scale, 1, 1, DustID.Smoke, a.ToRotationVector2().X * 0.4f, a.ToRotationVector2().Y * 0.4f);
                    }
                }
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 nv = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.One);
                if (nv != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = nv;

            }
            if (Projectile.velocity.X > 0)
            {
                Projectile.direction = 1;
                owner.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
                owner.direction = -1;
            }
            Player player = Projectile.owner.ToPlayer();
            Projectile.Center = owner.MountedCenter + player.gfxOffY * Vector2.UnitY;
            owner.itemRotation = Projectile.rotation * Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public float counter = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            if (Projectile.ai[0] >= maxTime)
            {
                lightColor = Color.Lerp(lightColor, Color.Red, 0.5f + (float)Math.Cos((++counter) * 0.1f) * 0.5f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vector2> v2ss = new List<Vector2>() { new Vector2(-4, -4), new Vector2(-4, 4), new Vector2(4, -4), new Vector2(4, 4), new Vector2(4, 0), new Vector2(-4, 0), new Vector2(0, 4), new Vector2(0, -4) };
            Texture2D to = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/OverloadFurnaceOutline").Value;

            foreach (Vector2 v in v2ss)
            {
                Main.EntitySpriteDraw(to, Projectile.Center - Main.screenPosition, null, lightColor * (Projectile.ai[0] / (float)maxTime) * (Projectile.ai[0] >= maxTime ? 1f : 0.95f), Projectile.rotation + MathHelper.ToRadians(45), new Vector2(0, texture.Height), Projectile.scale, SpriteEffects.None);
            }


            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.ToRadians(45), new Vector2(0, texture.Height), Projectile.scale, SpriteEffects.None);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D light = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Glow").Value;
            Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 34 * Projectile.scale, null, (Projectile.ai[0] >= maxTime ? Color.Lerp(Color.White, Color.Red, (0.5f + (float)Math.Cos((counter) * 0.1f) * 0.5f)) : Color.White) * (Projectile.ai[0] / (float)maxTime), 0, light.Size() / 2, 0.2f * Projectile.scale * (1 + (float)Math.Cos((counter) * 0.1f) * 0.2f), SpriteEffects.None, 0);

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
    }

}