using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class NihilityVirus : ModProjectile
    {
        public List<LightningAdvanced> lightnings = new List<LightningAdvanced>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.56f;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.ArmorPenetration = 64;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (targets.Contains(target)) return null;
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<VoidVirus>(), 360);
            Util.Util.PlaySound("nvspark", Main.rand.NextFloat(0.6f, 1.4f), target.Center, 4, 1f);
        }
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        List<NPC> targets = new List<NPC>();
        public LoopSound sound = null;
        public override bool PreAI()
        {
            if (lightnings.Count == 0) { 
                for(int i = 0; i < 9; i++)
                {
                    lightnings.Add(new LightningAdvanced(Projectile.Center, Projectile.Center));
                }
            }
            return base.PreAI();
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            if (sound == null)
            {
                sound = new LoopSound(CalamityEntropy.ealaserSound2);
                sound.play();
            }
            sound.setVolume_Dist(Projectile.getOwner().Center, 200, 1600, 0.5f);
            sound.timeleft = 2;
            var player = Projectile.getOwner();
            if (player.channel)
            {
                Projectile.timeLeft = 4;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center) * 0.12f;
            }
            player.direction = Projectile.Center.X + Projectile.velocity.X > player.Center.X ? 1 : -1;
            player.itemTime = 9;
            player.itemAnimation = 9;
            player.itemRotation = ((Projectile.Center - player.Center) * player.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            if (Main.GameUpdateCount % 12 == 0)
            {
                player.manaRegenDelay = 30;
                if (!player.CheckMana(player.HeldItem.mana, true))
                {
                    Projectile.Kill();
                }
            }
            Projectile.netUpdate = true;

            Projectile.rotation += 0.16f * player.direction;
            Util.Util.recordOldPosAndRots(Projectile, ref odp, ref odr, 12);
            targets.Clear();
            var activenpcs = Main.ActiveNPCs;
            for (int i = 0; i < 8; i++)
            {
                float dist = 640 + player.Entropy().WeaponBoost * 200;
                NPC target = null;
                foreach (NPC npc in activenpcs)
                {
                    if (!targets.Contains(npc))
                    {
                        if (!npc.friendly && !npc.dontTakeDamage && npc.CanBeChasedBy(Projectile))
                        {
                            if (Util.Util.getDistance(npc.Center, Projectile.Center) < dist)
                            {
                                target = npc;
                                dist = Util.Util.getDistance(npc.Center, Projectile.Center);
                            }
                        }
                    }
                }
                if (target != null)
                {
                    targets.Add(target);
                }
            }
            if (targets.Count == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    float dist = 640 + player.Entropy().WeaponBoost * 200;
                    NPC target = null;
                    foreach (NPC npc in activenpcs)
                    {
                        if (!targets.Contains(npc))
                        {
                            if (!npc.friendly && !npc.dontTakeDamage)
                            {
                                if (Util.Util.getDistance(npc.Center, Projectile.Center) < dist)
                                {
                                    target = npc;
                                    dist = Util.Util.getDistance(npc.Center, Projectile.Center);
                                }
                            }
                        }
                    }
                    if (target != null)
                    {
                        targets.Add(target);
                    }
                }
            }
        }
        public void drawlightning(int index, float width = 1, float lightSize = 1)
        {
            var points = lightnings[index].GetPoints();
            Texture2D lightning = Util.Util.getExtraTex("Lightning");
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied, SamplerState.LinearWrap);
            {
                List<Vertex> ve = new List<Vertex>();
                Color b = Color.White;
                float p = -Main.GlobalTimeWrappedHourly * 2;
                for (int i = 1; i < points.Count; i++)
                {
                    ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 16 * Projectile.scale * lightSize,
                          new Vector3(p, 1, 1),
                          b));
                    ve.Add(new Vertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 16 * Projectile.scale * lightSize,
                          new Vector3(p, 0, 1),
                          b));
                    p += (Util.Util.getDistance(points[i], points[i - 1]) / lightning.Width) * 0.7f;
                }

                SpriteBatch sb = Main.spriteBatch;
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                if (ve.Count >= 3)
                {
                    gd.Textures[0] = lightning;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Texture2D light = Util.Util.getExtraTex("lightball");
            
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(light, points[points.Count - 1] - Main.screenPosition, null, new Color(120, 120, 200), 0, light.Size() / 2, width * 0.14f * lightSize, SpriteEffects.None, 0);

            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 1 + (8 - targets.Count) / 8f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 opos = Projectile.getOwner().MountedCenter + Projectile.getOwner().gfxOffY * Vector2.UnitY;
            Vector2 handPos = opos + (Projectile.Center - opos).SafeNormalize(Vector2.Zero) * 120 * Projectile.getOwner().HeldItem.scale;
            Vector2 end = Projectile.Center;
            lightnings[8].Update(handPos, Projectile.Center);
            drawlightning(8, 1.6f, 2);

            int lc = 0;
            foreach (NPC npc in targets)
            {
                if (Util.Util.getDistance(lightnings[lc].Point2, npc.Center) > 24)
                {
                    for (int i = 0; i < 48; i++)
                    {
                        lightnings[lc].Update(Projectile.Center, npc.Center);
                    }
                }
                lightnings[lc].Update(Projectile.Center, npc.Center);
                drawlightning(lc, 2.6f - (targets.Count * 0.225f));
                lc++;
            }
            Texture2D tex = Projectile.getTexture();
            Rectangle frame = Util.Util.GetCutTexRect(tex, 4, Projectile.frame, false);
            Vector2 origin = new Vector2(78, 80) / 2;
            float ap = 1f / (float)odp.Count;
            for (int i = 0; i < odp.Count; i++)
            {
                Main.spriteBatch.Draw(tex, odp[i] - Main.screenPosition, frame, Color.White * ap * 0.4f, odr[i], origin, 1, SpriteEffects.None, 0);
                ap += 1f / (float)odp.Count;
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }


}