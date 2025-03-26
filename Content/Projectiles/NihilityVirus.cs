using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class NihilityVirus : ModProjectile
    {
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
        public static void drawlightning(Vector2 start, Vector2 end, float width = 1, float lightSize = 1)
        {
            Texture2D lightning = Util.Util.getExtraTex("Lightning");
            Vector2 handPos = start;
            Main.spriteBatch.UseBlendState(BlendState.Additive, SamplerState.LinearWrap);
            Main.spriteBatch.Draw(lightning, handPos - Main.screenPosition, new Rectangle((int)(Main.GameUpdateCount * -16), 0, (int)(Util.Util.getDistance(handPos, end) / 2f), lightning.Height), new Color(160, 160, 160), (end - handPos).ToRotation(), new Vector2(0, lightning.Height / 2), new Vector2(2, 0.12f * width), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(lightning, handPos - Main.screenPosition, new Rectangle((int)(Main.GameUpdateCount * -36), 0, (int)(Util.Util.getDistance(handPos, end) / 2f), lightning.Height), new Color(120, 120, 176), (end - handPos).ToRotation(), new Vector2(0, lightning.Height / 2), new Vector2(2, 0.26f * width), SpriteEffects.FlipVertically, 0);
            Texture2D light = Util.Util.getExtraTex("lightball");
            Main.spriteBatch.Draw(light, end - Main.screenPosition, null, new Color(120, 120, 200), 0, light.Size() / 2, width * 0.14f * lightSize, SpriteEffects.None, 0);

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
            drawlightning(handPos, end, 1.6f, 3);
            foreach (NPC npc in targets)
            {
                drawlightning(Projectile.Center, npc.Center, 2.6f - (targets.Count * 0.225f));
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