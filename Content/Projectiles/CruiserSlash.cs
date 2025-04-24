using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class CruiserSlash : ModProjectile
    {
        public bool sPlayerd = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            CalamityEntropy.checkProj.Add(Projectile);
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.light = 30f;
            Projectile.timeLeft = 88;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 1024;
        }
        public int ct = 0;
        public override void AI()
        {
            ct++;
            if (ct > 60)
            {
                if (!sPlayerd)
                {
                    sPlayerd = true;
                    SoundStyle s = new("CalamityEntropy/Assets/Sounds/swing" + Main.rand.Next(1, 4));
                    s.Volume = 1f;
                    s.Pitch = 0.8f;
                    SoundEngine.PlaySound(s, Projectile.Center);
                }
                if (Projectile.ai[2] < 6)
                {
                    Projectile.ai[0] += 100;
                }
                else
                {
                    Projectile.ai[1] = Projectile.ai[1] + (Projectile.ai[0] - Projectile.ai[1]) * 0.3f;
                }
                Projectile.ai[2]++;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ct > 60 && Util.Util.LineThroughRect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 380, Projectile.Center + Projectile.rotation.ToRotationVector2() * -380, targetHitbox, 12);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t1 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/lightball").Value;

            if (ct < 60)
            {
                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Texture2D t = Util.Util.getExtraTex("a_circle");
                Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, null, new Color(180, 180, 255) * ((float)ct / 60f) * 0.4f, Projectile.rotation, t.Size() / 2f, new Vector2(6.4f, 0.2f), SpriteEffects.None); ;
                sb.Draw(t1, Projectile.Center - Main.screenPosition, null, Color.DarkBlue * ((float)ct / 60f) * 0.3f, 0, new Vector2(t1.Width, t1.Height) / 2, 50f * (60 - ct) / 128, SpriteEffects.None, 0);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                
            }
            return false;
        }


    }


}