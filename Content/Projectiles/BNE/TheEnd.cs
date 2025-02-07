using System;
using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Particle = CalamityEntropy.Content.Particles.Particle;

namespace CalamityEntropy.Content.Projectiles.BNE
{
    public class TheEnd : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rogueDC;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 260;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 80;
            Projectile.light = 1;
            Projectile.MaxUpdates = 7;
        }
        public bool playsound = true;
        public override void AI(){
            Vector2 top = Projectile.Center;
            Vector2 sparkVelocity2 = Projectile.velocity * -0.1f;
            int sparkLifetime2 = Main.rand.Next(8, 12);
            float sparkScale2 = Main.rand.NextFloat(0.6f, 1.2f);
            Color sparkColor2 = Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, Main.rand.NextFloat(0, 1));
            LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
            GeneralParticleHandler.SpawnParticle(spark);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (playsound)
            {
                playsound = false;
                TheBeginingAndTheEnd.playShootSound(Projectile.Center);
            }
            if (Projectile.Calamity().stealthStrike)
            {
                NPC target = Projectile.FindTargetWithinRange(3600);
                if (target != null)
                {
                    Projectile.ai[0] = target.whoAmI;
                }
                else
                {
                    Projectile.ai[0] = -1;
                }
                if (++Projectile.localAI[0] > 12 && target != null)
                {
                    Projectile.velocity += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.9f;
                    Projectile.velocity *= 0.95f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < (Projectile.Calamity().stealthStrike ? 6 : 1); i++)
            {
                EParticle.spawnNew(new AbyssalLine() { lx = 1.6f, xadd = 1.6f }, target.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, Util.Util.randomRot());
            }
            target.Entropy().StareOfAbyssTime = 12 * 60;
            target.Entropy().StareOfAbyssLevel = (int)MathHelper.Min(target.Entropy().StareOfAbyssLevel + (Projectile.Calamity().stealthStrike ? 6 : 1), 12);
            Util.Util.PlaySound("bne_hit", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center, 6);
            if (Projectile.Calamity().stealthStrike)
            {
                for(int i = 0; i < 8 + target.Entropy().StareOfAbyssLevel; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + Util.Util.randomRot().ToRotationVector2() * 26, Util.Util.randomRot().ToRotationVector2() * 26, ModContent.ProjectileType<SoulOfEcho>(), Projectile.damage / 4, Projectile.knockBack / 2, Projectile.owner);
                }
                target.Entropy().StareOfAbyssTime = 0;
                Util.Util.PlaySound("bne_hit2", 1, Projectile.Center);
            }
            else
            {
                if (target.Entropy().EclipsedImprintLevel > 0)
                {
                    for (int i = 0; i < target.Entropy().EclipsedImprintLevel; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Util.Util.randomRot().ToRotationVector2() * 26, ModContent.ProjectileType<SoulOfEclipse>(), 0, 0, Projectile.owner, Main.rand.Next(0, 80));
                    }
                    target.Entropy().EclipsedImprintTime = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }


    }

}