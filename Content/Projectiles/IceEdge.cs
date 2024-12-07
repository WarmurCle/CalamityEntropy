using System.Collections.Generic;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class IceEdge : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        bool htd = false;
        float exps = 0;
        int frame = 1;
        int framejc = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 110;
        }

        public override void AI(){
            
            Projectile.ai[0]++;
            if (htd)
            {
                if (odp.Count > 0)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                odp.Add(Projectile.Center);
                odr.Add(Projectile.rotation);
                if (odp.Count > 9)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }
                Projectile.velocity = new Vector2(28, 0).RotatedBy(Projectile.rotation);
            }
            if (exps > 0)
            {
                exps++;
                framejc++;
                if (framejc > 2) {
                    framejc = 0;
                    frame++;
                    if (frame > 5)
                    {
                        frame = 5;
                     
                    }
                    if (frame < 1)
                    {
                        frame = 1;
                    }
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (base.Colliding(projHitbox, targetHitbox) == null)
            {
                return base.Colliding(projHitbox, targetHitbox);
            }
            return (!htd) && (bool)base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!htd)
            {
                target.AddBuff(ModContent.BuffType<GlacialState>(), 600);
                target.AddBuff(BuffID.Frostburn, 1080);
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<CosmicFreeze>(), 600);
                target.immune[Projectile.owner] = 0;
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                base.OnHitNPC(target, hit, damageDone);
                Projectile.timeLeft = 15;
                htd = true;
                exps = 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (exps > 0)
            {
                if (htd)
                {
                    Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/exp" + frame.ToString()).Value;


                    Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition + new Vector2(0, -50), null, Color.White, 0, new Vector2(tx.Height, tx.Width) / 2, 1, SpriteEffects.None, 0);
                    return false;
                }
            }
            else
            {
                Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/IceEdge").Value;
                float x = 0f;
                for (int i = 0; i < odp.Count; i++)
                {
                    Main.spriteBatch.Draw(tx, odp[i] - Main.screenPosition, null, Color.White * x * 0.6f, odr[i], new Vector2(tx.Width, tx.Height) / 2, 1, SpriteEffects.None, 0);
                    x += 1 / 10f;
                }
                
            }
            return true;
        }

    }

}