using Microsoft.Xna.Framework;
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
            Projectile.timeLeft = 110;
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
            if (ct == 60)
            {
                foreach (Player pl in Main.player)
                {
                    if (Util.Util.getDistance(Projectile.Center, pl.Center) < 3600)
                    {
                        Projectile.rotation = (pl.Center - Projectile.Center).ToRotation();
                        break;
                    }
                }
                if (Main.rand.Next(0, 2) == 0)
                {
                    Projectile.rotation += (float)Math.PI;
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ct > 60 && Util.Util.LineThroughRect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 420, Projectile.Center + Projectile.rotation.ToRotationVector2() * -420, targetHitbox, 12, 8); ;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t1 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/lightball").Value;

            if (ct < 60)
            {
                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                sb.Draw(t1, Projectile.Center - Main.screenPosition, null, Color.DarkBlue * ((float)ct / 60f), 0, new Vector2(t1.Width, t1.Height) / 2, 50f * (60 - ct) / 128, SpriteEffects.None, 0);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }
            return false;
        }


    }


}