using CalamityEntropy;
using CalamityEntropy.Content.Items.Armor.AzafureT3;
using CalamityEntropy.Content.NPCs.Acropolis;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.AzafureT3
{
    public class AcropolisBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            CEUtils.FriendlySetDefaults(Projectile, DamageClass.Generic, true, 3);
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 8; i++)
            {
                EParticle.NewParticle(new Smoke() { timeleftmax = 26, Lifetime = 26 }, Projectile.Center + Projectile.velocity * (i / 8f), CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.02f, 0.04f), 0.5f, true, BlendState.Additive, CEUtils.randomRot());
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 60, 60), Projectile.rotation - MathHelper.PiOver2, tex.Size() / 2f, Projectile.scale * new Vector2(1, 1f), SpriteEffects.None, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits < 2)
            {
                CEUtils.PlaySound("pulseBlast", 0.8f, Projectile.Center, 6, 0.4f);
                GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Firebrick, 0.1f, 0.7f, 8));
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Firebrick, 1.6f, 1, true, BlendState.Additive, 0, 16);
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 1f, 1, true, BlendState.Additive, 0, 16);
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                CEUtils.PlaySound("AcrHit", Main.rand.NextFloat(0.7f, 1.3f), Projectile.Center);
                for (int i = 0; i < 6; i++)
                    GeneralParticleHandler.SpawnParticle(new LineParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(), false, 12, Main.rand.NextFloat(0.6f, 1.2f), new Color(255, 100, 100)));
            }
        }
    }
    public class AcropolisSlash : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            CEUtils.FriendlySetDefaults(Projectile, DamageClass.Generic, false, -1);
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 10;
        }
        public override void AI()
        {
            var mp = Projectile.GetOwner().GetModPlayer<AcropolisArmorPlayer>();
            Projectile.Center = mp.cannon.TopPos;
            Projectile.rotation = mp.cannon.Seg2Rot;
            if (Main.myPlayer == Projectile.owner)
                if (mp.SlashP == 0)
                    Projectile.Kill();
            if(oldPos.Count == 0)
            {
                oldPos.Add(Projectile.Center);
                oldRot.Add(Projectile.rotation);
            }
            else
            {
                var p = oldPos[oldPos.Count - 1];
                var r = oldRot[oldRot.Count - 1];
                for (float i = 0.1f; i <= 1; i+=0.1f)
                {
                    oldPos.Add(Vector2.Lerp(p, Projectile.Center, i));
                    oldRot.Add(CEUtils.RotateTowardsAngle(r, Projectile.rotation, i, false));
                }
            }

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            List<ColoredVertex> ve = new List<ColoredVertex>();

            for (int i = 0; i < oldRot.Count; i++)
            {
                Color b = Color.Lerp(new Color(255, 60, 60), new Color(255, 195, 195), (float)i / (float)oldRot.Count) * 0.8f;
                ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(190, 0).RotatedBy(oldRot[i])),
                      new Vector3(i / (float)oldRot.Count, 1, 1),
                      b));
                ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(0, 0).RotatedBy(oldRot[i])),
                      new Vector3(i / (float)oldRot.Count, 0, 1),
                      b));
            }

            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = CEUtils.getExtraTex("MotionTrail2");
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            Main.spriteBatch.ExitShaderRegion();
            Texture2D c = CEUtils.getExtraTex("Glow2");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for(float i = 0; i < 1; i += 0.02f)
            {
                float ml = 170;
                Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * (ml * i - 10);
                Main.spriteBatch.Draw(c, pos + CEUtils.randomPointInCircle(4) - Main.screenPosition, null, new Color(255, 50, 50), Projectile.rotation, c.Size() / 2f, new Vector2(0.6f, 0.04f), SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 210, targetHitbox, 80);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("slice", Main.rand.NextFloat(0.8f, 1.2f), target.Center);
        }
        public List<Vector2> oldPos = new();
        public List<float> oldRot = new();
    }
    public class AcropolisHarpoon: ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Armor/AzafureT3/Harpoon";
        public override void SetDefaults()
        {
            CEUtils.FriendlySetDefaults(Projectile, DamageClass.Generic, false, -1);
            Projectile.width = Projectile.height = 14;
            Projectile.timeLeft = 10;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public int HookNPC = -1;
        public bool HookHold = true;
        public bool HookTile = false;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HookHold);
            writer.Write(HookNPC);
            writer.Write(HookTile);
            writer.Write(back);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HookHold = reader.ReadBoolean();
            HookNPC = reader.ReadInt32();
            HookTile = reader.ReadBoolean();
            back = reader.ReadBoolean();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (HookNPC == -1 && !HookTile)
            {
                CEUtils.PlaySound("ExoHit1", 1, Projectile.Center);
                HookNPC = target.whoAmI;
            }
        }
        public override void AI()
        {
            if (Projectile.GetOwner().TryGetModPlayer<AcropolisArmorPlayer>(out var mp) && mp.harpoon != null)
            {
                Projectile.timeLeft = 5;
                if (Main.myPlayer == Projectile.owner)
                {
                    if (!Projectile.GetOwner().GetModPlayer<AcropolisArmorPlayer>().ControlHook)
                    {
                        HookHold = false;
                        HookNPC = -2;
                        CEUtils.SyncProj(Projectile.whoAmI);
                    }
                }
                if (!HookHold)
                {
                    HookNPC = -2;
                }
                for (float i = 0; i < 1; i += 0.05f)
                {
                    Projectile.position += Projectile.velocity / 20f;
                    if (!HookTile && HookNPC <= -1 && !back)
                    {
                        if (!CEUtils.isAir(Projectile.Center, true))
                        {
                            HookNPC = -2;
                            HookTile = true;
                            Projectile.velocity *= 0;
                            CEUtils.PlaySound("ExoHit1", 1, Projectile.Center);
                            CEUtils.SyncProj(Projectile.whoAmI);
                        }
                    }
                }
                if (back)
                {
                    if (HookTile || HookNPC >= -1)
                    {
                        HookTile = false;
                        HookNPC = -2;
                        CEUtils.SyncProj(Projectile.whoAmI);
                    }
                }
                Vector2 top = Projectile.GetOwner().GetModPlayer<AcropolisArmorPlayer>().harpoon.TopPos;
                if (HookTile)
                {
                    Projectile.GetOwner().Entropy().FallSpeedUP = 3;
                    Projectile.GetOwner().velocity = (Projectile.Center - Projectile.GetOwner().Center).normalize() * 30;
                    if (CEUtils.getDistance(Projectile.Center, Projectile.GetOwner().Center) < 60)
                    {
                        Projectile.Kill();
                    }
                    Projectile.GetOwner().Entropy().NoPlatformCollide = 6;
                }
                else if (HookNPC >= 0)
                {
                    Projectile.velocity *= 0;
                    if (!HookNPC.ToNPC().active)
                    {
                        HookNPC = -2;
                    }
                    else
                    {
                        Projectile.GetOwner().Entropy().FallSpeedUP = 3;
                        Projectile.Center = HookNPC.ToNPC().Center;
                        mp.harpoon.PointAPos(Projectile.Center, 1);

                        if (HookNPC.ToNPC().boss)
                        {
                            Projectile.GetOwner().velocity = (Projectile.Center - Projectile.GetOwner().Center).normalize() * 40;
                            if (CEUtils.getDistance(Projectile.Center, Projectile.GetOwner().Center) < 160)
                            {
                                Projectile.GetOwner().velocity *= -0.1f;
                                Projectile.Kill();
                                Projectile.GetOwner().Entropy().immune = 12;
                            }
                        }
                        else
                        {
                            HookNPC.ToNPC().velocity = (top - HookNPC.ToNPC().Center).normalize() * 40;
                            
                            if (CEUtils.getDistance(Projectile.Center, top) < 60)
                            {
                                HookNPC.ToNPC().velocity *= 0;
                                mp.harpoon.PointAPos(Projectile.GetOwner().Calamity().mouseWorld, 1);
                                Projectile.rotation = mp.harpoon.Seg2Rot;
                                HookNPC.ToNPC().Center = mp.harpoon.TopPos + mp.harpoon.Seg2Rot.ToRotationVector2() * 16;

                            }
                        }
                    }
                }
                else
                {
                    if (Projectile.ai[2] > 17)
                        back = true;
                    if (back)
                    {
                        Projectile.velocity *= 0.9f;
                        Projectile.velocity += (top - Projectile.Center).normalize() * 6;
                        if (CEUtils.getDistance(Projectile.Center, top) < Projectile.velocity.Length() * 1.1f + 8)
                        {
                            Projectile.Kill();
                        }
                    }
                }
                Projectile.rotation = (Projectile.Center - top).ToRotation();
                if (Projectile.ai[2] == 0)
                {
                    CEUtils.PlaySound("chainsawHit", 1, Projectile.Center);
                }
                Projectile.ai[2]++;
                
                if (Projectile.GetOwner().controlJump && !mp.lastJump)
                    Projectile.Kill();

                if (Projectile.active && HookNPC < 0)
                    mp.harpoon.PointAPos(Projectile.Center, 1);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center - Projectile.velocity, targetHitbox, 32);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                modifiers.SourceDamage *= 0.1f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.GetOwner().TryGetModPlayer<AcropolisArmorPlayer>(out var mp) && mp.harpoon != null)
            {
                int s = 12;
                CEUtils.drawChain(Projectile.Center, mp.harpoon.TopPos, s, "CalamityEntropy/Content/Items/Armor/AzafureT3/Chain");
                Texture2D tex = Projectile.GetTexture();
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, 1, SpriteEffects.None);
            }
            return false;
        }
        public bool back = false;
    }
}
