using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.OblivionThresher
{
    public class OblivionCruiserDash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        float mouthRot = 60f;
        public bool bite = false;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ArmorPenetration = 60;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 2;
        }
        public Vector2 spawnPos;
        public float spawnRot = 0;
        public float alphaPor = 1;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Center.getRectCentered(200 * Projectile.scale, 200 * Projectile.scale).Intersects(targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 90, 3.6f, 1000, 16);
        }
        float counter = 0;
        public override string Texture => CEUtils.WhiteTexPath;
        public override void AI()
        {
            counter++;
            Player player = Projectile.owner.ToPlayer();
            Projectile.rotation = Projectile.velocity.ToRotation();
            player.Center = Projectile.Center;
            player.velocity = Projectile.velocity * 0.2f;
            player.Entropy().immune = 10;
            mouthRot *= 0.88f;
            spawnParticles();
            int t = ModContent.ProjectileType<OblivionThresherShoot>();
            foreach(var p in Main.ActiveProjectiles)
            {
                if(p.owner == Projectile.owner && p.type == t)
                {
                    if(p.ModProjectile is OblivionThresherShoot ots)
                    {
                        if (p.ai[0] >= 1f && p.localAI[1] > 80 && p.Colliding(p.Hitbox, Projectile.Hitbox))
                        {
                            player.velocity = -Projectile.velocity * 0.2f;
                            p.Kill();
                            if (Projectile.owner == Main.myPlayer)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Projectile.velocity, ModContent.ProjectileType<OblivionThresherHoldout>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Projectile.velocity, ModContent.ProjectileType<OblivionThresherShootAlt>(), Projectile.damage * 6, Projectile.knockBack, Projectile.owner);
                            }
                            Projectile.Kill();
                            CEUtils.PlaySound("CastTriangles", 1, Projectile.Center);
                            return;
                        }
                    }
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 10;
            modifiers.FinalDamage *= 2;
        }
        public void spawnParticles()
        {
            for (int i = 0; i < 2; i++)
            {
                Particle p = new Particle();
                p.shape = 4;
                p.position = Projectile.Center - Projectile.rotation.ToRotationVector2() * 60;
                p.alpha = 0.9f * (1 - Projectile.timeLeft / 24f);
                p.ad = 0.013f;
                var r = Main.rand;
                p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                VoidParticles.particles.Add(p);
            }
            for (int i = 0; i < 2; i++)
            {
                Particle p = new Particle();
                p.shape = 4;
                p.position = Projectile.Center - Projectile.rotation.ToRotationVector2() * 60 - Projectile.velocity * 0.5f;
                p.alpha = 0.9f * (1 - Projectile.timeLeft / 24f);
                p.ad = 0.013f;
                var r = Main.rand;
                p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                VoidParticles.particles.Add(p);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Vector2 vtodraw = Projectile.Center;
            SpriteBatch spriteBatch = Main.spriteBatch;
            float alpha = 1;
            if (Projectile.timeLeft < 10)
            {
                alpha = (float)Projectile.timeLeft / 10f;
            }
            Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/Head2").Value;
            Texture2D j2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawUp2").Value;
            Texture2D j1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawDown2").Value;
            Vector2 joffset = new Vector2(60, 62);
            Vector2 ofs2 = joffset * new Vector2(1, -1);
            float roth = mouthRot * 0.8f;

            spriteBatch.Draw(j1, vtodraw - Main.screenPosition + joffset.RotatedBy(Projectile.rotation) * Projectile.scale, null, Color.White * alpha, Projectile.rotation + MathHelper.ToRadians(roth), new Vector2(40, 28), Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(j2, vtodraw - Main.screenPosition + ofs2.RotatedBy(Projectile.rotation) * Projectile.scale, null, Color.White * alpha, Projectile.rotation - MathHelper.ToRadians(roth), new Vector2(40, j2.Height - 28), Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, vtodraw - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(j1, vtodraw - Main.screenPosition + joffset.RotatedBy(Projectile.rotation) * Projectile.scale, null, Color.White * alpha, Projectile.rotation + MathHelper.ToRadians(roth), new Vector2(40, 28), Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(j2, vtodraw - Main.screenPosition + ofs2.RotatedBy(Projectile.rotation) * Projectile.scale, null, Color.White * alpha, Projectile.rotation - MathHelper.ToRadians(roth), new Vector2(40, j2.Height - 28), Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, vtodraw - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.ExitShaderRegion();
            return false;
        }
    }


}