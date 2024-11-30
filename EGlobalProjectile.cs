using CalamityEntropy.Buffs;
using CalamityEntropy.Items;
using CalamityEntropy.Projectiles;
using CalamityEntropy.Projectiles.HBProj;
using CalamityEntropy.Projectiles.SamsaraCasket;
using CalamityEntropy.Projectiles.TwistedTwin;
using CalamityEntropy.Projectiles.VoidEchoProj;
using CalamityEntropy.Util;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy
{
    public class EGlobalProjectile : GlobalProjectile
    {
        public bool Lightning = false;
        public List<Vector2> odp = new List<Vector2>();
        public List<Vector2> odp2 = new List<Vector2>();
        public List<float> odr = new List<float>();
        public float counter = 0;
        public int AtlasItemType = 0;
        public int AtlasItemStack = 0;
        public bool DI = false;
        public bool gh = false;
        public int ghcounter = 0;
        public int ttindex = -1;
        public int OnProj = -1;
        public int flagTT = 0;
        public Vector2 playerPosL;
        public Vector2 playerMPosL;
        public bool daTarget = false;
        public int maxDmgUps = 0;
        public float dmgUp = 0.05f;
        public bool GWBow = false;
        public int dmgupcount = 10;
        public int vddirection = 1;
        public override GlobalProjectile Clone(Projectile from, Projectile to)
        {
            var p = to.Entropy();
            p.DI = DI;
            p.gh = gh;
            p.ttindex = ttindex;
            p.flagTT = flagTT;
            p.daTarget = daTarget;
            p.maxDmgUps = maxDmgUps;
            p.dmgUp = dmgUp;
            p.GWBow = GWBow;
            p.dmgupcount = dmgupcount;
            p.counter = counter;
            p.withGrav = withGrav;
            return p;
        }
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return true;
        }
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(projectile.Entropy().OnProj);
            binaryWriter.Write(projectile.Entropy().ttindex);
            binaryWriter.Write(projectile.Entropy().GWBow);
            binaryWriter.Write(projectile.Entropy().withGrav);
            binaryWriter.Write(vdtype);
            binaryWriter.Write(vddirection);
            binaryWriter.Write(rpBow);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            projectile.Entropy().OnProj = binaryReader.ReadInt32();
            projectile.Entropy().ttindex = binaryReader.ReadInt32();
            projectile.Entropy().GWBow = binaryReader.ReadBoolean();
            projectile.Entropy().withGrav = binaryReader.ReadBoolean();
            vdtype = binaryReader.ReadInt32();
            vddirection = binaryReader.ReadInt32();
            rpBow = binaryReader.ReadBoolean();
        }
        public override bool InstancePerEntity => true;
        public override void SetDefaults(Projectile entity)
        {
            if (entity.Entropy().Lightning)
            {
                entity.penetrate = 5;
            }
        }
        public bool netsnc = true;
        public static bool checkHoldOut = true;
        
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent s)
            {
                if (s.Entity is Player player)
                {
                    projectile.velocity *= player.Entropy().shootSpeed;
                }
            }
            if (projectile.friendly && projectile.owner != -1)
            {
                if (projectile.owner.ToPlayer().Entropy().VFHelmRanged)
                {
                    maxDmgUps = 10;
                }
            }
            if (source is EntitySource_ItemUse && checkHoldOut && projectile.owner == Main.myPlayer && (projectile.ModProjectile is BaseIdleHoldoutProjectile || projectile.type == ModContent.ProjectileType<VoidEchoProj>() || projectile.type == ModContent.ProjectileType<HB>() || projectile.type == ModContent.ProjectileType<GhostdomWhisperHoldout>() || projectile.type == ModContent.ProjectileType<RailPulseBowProjectile>() || projectile.type == ModContent.ProjectileType<SamsaraCasketProj>()))
            {
                checkHoldOut = false;
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active && p.type == ModContent.ProjectileType<TwistedTwinMinion>() && p.owner == Main.myPlayer) {
                        int phd = Projectile.NewProjectile(Main.LocalPlayer.GetSource_ItemUse(Main.LocalPlayer.HeldItem), p.Center, Vector2.Zero, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                        Projectile ph = phd.ToProj();
                        ph.scale *= 0.8f;
                        ph.Entropy().OnProj = p.whoAmI;
                        ph.Entropy().ttindex = p.whoAmI;
                        ph.netUpdate = true;
                        Projectile projts = ph;
                        ph.damage = (int)(ph.damage * TwistedTwinMinion.damageMul);
                        if (!projts.usesLocalNPCImmunity)
                        {
                            projts.usesLocalNPCImmunity = true;
                            projts.localNPCHitCooldown = 12;
                        }
                    }
                }
                checkHoldOut = true;
            }
            if (source is EntitySource_Parent ps)
            {
                if (ps.Entity is Projectile pj)
                {
                    
                    if (((Projectile)ps.Entity).Entropy().DI)
                    {
                        projectile.friendly = ((Projectile)ps.Entity).friendly;
                        projectile.hostile = ((Projectile)ps.Entity).hostile;
                    }
                    if (((Projectile)ps.Entity).Entropy().gh)
                    {
                        if (!projectile.minion && projectile.damage > 0)
                        {
                            projectile.Entropy().gh = true;
                        }
                    }
                    if (pj.Entropy().ttindex != -1)
                    {
                        projectile.Entropy().ttindex = pj.Entropy().ttindex;
                        int type = projectile.type;
                        
                        if (projectile.velocity.Length() > 1)
                        {
                            if (!(type == 493) && !(type == 494) && !(type == 150) && !(type == 151) && !(type == 152) && !(type == ModContent.ProjectileType<AtlantisSpear>())) {
                                //projectile.Center = projectile.Entropy().ttindex.ToProj().Center;
                            }
                        }
                        
                    }
                    projectile.Entropy().flagTT = pj.Entropy().flagTT + 1;  
                }
                if (ps.Entity is Player plr)
                {
                    if (plr.Entropy().twinSpawnIndex != -1)
                    {
                        if (projectile.type != ModContent.ProjectileType<TwistedTwinMinion>())
                        {
                            projectile.scale *= 0.8f;
                            projectile.Entropy().OnProj = plr.Entropy().twinSpawnIndex;
                            projectile.Entropy().ttindex = plr.Entropy().twinSpawnIndex;
                            projectile.netUpdate = true;
                            
                            Projectile projts = projectile;
                            if (!projts.usesLocalNPCImmunity)
                            {
                                projts.usesLocalNPCImmunity = true;
                                projts.localNPCHitCooldown = 12;
                            }
                        }
                    }
                    
                    if (plr.GetModPlayer<EModPlayer>().Godhead)
                    {
                        if (!projectile.minion && projectile.damage > 0)
                        {
                            projectile.Entropy().gh = true;
                        }
                    }
                    if (plr.HasBuff(ModContent.BuffType<SoyMilkBuff>()))
                    {
                        projectile.extraUpdates = (projectile.extraUpdates + 1) * 2 - 1;
                    }
                }
                
            }

        }
        public override bool ShouldUpdatePosition(Projectile projectile)
        {
            if (projectile.Entropy().daTarget)
            {
                return false;
            }
            return base.ShouldUpdatePosition(projectile);
        }

        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (vdtype >= 0 || projectile.ModProjectile is GodSlayerRocketProjectile)
            {
                return false;
            }
            return base.CanHitPlayer(projectile, target);
        }

        public override bool PreAI(Projectile projectile)
        {
            if (projectile.Entropy().vdtype >= 0 || projectile.ModProjectile is GodSlayerRocketProjectile)
            {
                projectile.hostile = false;
                projectile.friendly = true;
            }
            if (vdtype == 0)
            {
                projectile.velocity = projectile.velocity.RotatedBy(Math.Cos((counter + MathHelper.Pi * 0.5f) * 0.2f) * (float)vddirection * 0.18f);
                projectile.rotation = projectile.velocity.ToRotation();
            }
            if (withGrav)
            {
                projectile.velocity.Y += 0.3f;
            }
            dmgupcount--;
            if (maxDmgUps > 0 && dmgupcount <= 0 && projectile.DamageType == DamageClass.Ranged)
            {
                dmgupcount = 24;
                maxDmgUps--;
                projectile.damage = (int)(Math.Ceiling(projectile.damage * dmgUp)) + (projectile.damage);
            }
            if (GWBow)
            {
                if (Main.rand.NextBool(2))
                {
                    Vector2 direction = new Vector2(-1, 0).RotatedBy(projectile.velocity.ToRotation());
                    Vector2 smokeSpeed = direction.RotatedByRandom(MathHelper.PiOver4 * 0.1f) * Main.rand.NextFloat(10f, 30f) * 0.9f;
                    CalamityMod.Particles.Particle smoke = new HeavySmokeParticle(projectile.Center + direction * 46f, smokeSpeed + projectile.velocity, Color.Lerp(Color.Purple, Color.Indigo, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f)), 30, Main.rand.NextFloat(0.6f, 1.2f), 0.8f, 0, false, 0, true);
                    GeneralParticleHandler.SpawnParticle(smoke);

                    if (Main.rand.NextBool(3))
                    {
                        CalamityMod.Particles.Particle smokeGlow = new HeavySmokeParticle(projectile.Center + direction * 46f, smokeSpeed + projectile.velocity, Main.hslToRgb(0.85f, 1, 0.8f), 20, Main.rand.NextFloat(0.4f, 0.7f), 0.8f, 0.01f, true, 0.01f, true);
                        GeneralParticleHandler.SpawnParticle(smokeGlow);
                    }
                }
                NPC target = projectile.FindTargetWithinRange(1000, false);
                if (target != null && counter > 15)
                {
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0).RotatedBy(Util.Util.rotatedToAngle(projectile.velocity.ToRotation(), (target.Center - projectile.Center).ToRotation(), 0.12f * projectile.velocity.Length(), true));
                }
            }
            if (projectile.Entropy().daTarget)
            {
                return false;
            }
            if (projectile.Entropy().OnProj >= 0)
            {
                if (netsnc)
                {
                    projectile.netUpdate = true;
                    netsnc = false;
                }
                playerPosL = projectile.owner.ToPlayer().Center;
                projectile.owner.ToPlayer().Center = projectile.Entropy().OnProj.ToProj().Center;
            }
            projectile.Entropy().counter++;
            projectile.Entropy().odp.Add(projectile.Center);
            projectile.Entropy().odp2.Add(projectile.Center);
            projectile.Entropy().odr.Add(projectile.rotation);
            if (projectile.Entropy().odp.Count > 7)
            {
                projectile.Entropy().odp.RemoveAt(0);
                projectile.Entropy().odr.RemoveAt(0);
            }
            if (projectile.Entropy().odp2.Count > 22)
            {
                projectile.Entropy().odp2.RemoveAt(0);
            }
            if (projectile.arrow)
            {
                if (projectile.owner.ToPlayer().HeldItem.type == ModContent.ItemType<Kinanition>())
                {
                    projectile.Entropy().Lightning = true;
                }
            }
            if (projectile.Entropy().Lightning)
            {
                if (projectile.Entropy().counter % 18 == 0 && projectile.owner == Main.myPlayer) {
                    int p = Projectile.NewProjectile(projectile.owner.ToPlayer().GetSource_FromAI(), projectile.Center + projectile.velocity * 1.4f, Vector2.Zero, ModContent.ProjectileType<Lcircle>(), 0, 0, projectile.owner);
                    p.ToProj().rotation = projectile.velocity.ToRotation();
                }
                Lighting.AddLight(projectile.Center, 0.7f, 0.7f, 0.9f);
                if (projectile.penetrate < 5)
                {
                    NPC target = projectile.FindTargetWithinRange(1800, false);
                    if (target != null)
                    {
                        projectile.velocity += (target.Center - projectile.Center).ToRotation().ToRotationVector2() * 1.8f;
                        projectile.velocity *= 0.998f;
                    }
                }
            }
            if (projectile.Entropy().gh)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    foreach (NPC n in Main.npc)
                    {
                        if (!n.friendly && !n.dontTakeDamage)
                        {
                            float rsize = (projectile.width + projectile.height) / 2 * 6;
                            if (rsize < 128)
                            {
                                rsize = 128;
                            }
                            if (rsize > 600)
                            {
                                rsize = 600;
                            }
                            if (CircleIntersectsRectangle(projectile.Center, rsize / 2, n.Hitbox))
                            {
                                int c = projectile.Entropy().ghcounter;
                                if (c % 3 == 0)
                                {
                                    int ydf = n.defense;
                                    n.defense =(int)MathHelper.Min(projectile.damage / 5 / 2, n.defense);
                                    Main.LocalPlayer.ApplyDamageToNPC(n, projectile.damage / 5, 0, 0, false, DamageClass.Generic, false);
                                    n.defense = ydf;
                                }
                                projectile.Entropy().ghcounter++;
                            }
                        }
                    }
                }
            }
            
            return true;
        }




        public override void PostAI(Projectile projectile)
        {
            if (projectile.Entropy().OnProj >= 0)
            {
                projectile.owner.ToPlayer().Center = playerPosL;
            }
            else
            {
                if (projectile.owner == Main.myPlayer)
                {
                    ModContent.GetInstance<EModSys>().LastPlayerPos = projectile.owner.ToPlayer().Center;
                }
            }
        }
        public static bool CircleIntersectsRectangle(Vector2 circleCenter, float radius, Rectangle rectangle)
        {
            float nearestX = Math.Max(rectangle.Left, Math.Min(circleCenter.X, rectangle.Right));
            float nearestY = Math.Max(rectangle.Top, Math.Min(circleCenter.Y, rectangle.Bottom));

            float deltaX = circleCenter.X - nearestX;
            float deltaY = circleCenter.Y - nearestY;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance <= radius;
        }
        public Vector2 lmc;
        public Vector2 lastCenter;
        public bool withGrav = false;
        public int vdtype = -1;
        public bool rpBow = false;

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (projectile.ModProjectile != null)
            {
                projectile.ModProjectile.PostDraw(lightColor);
            }
            if (projectile.Entropy().OnProj >= 0)
            {
                projectile.owner.ToPlayer().Center = lastCenter;
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.Entropy().OnProj >= 0)
            {
                lastCenter = projectile.owner.ToPlayer().Center;
                projectile.owner.ToPlayer().Center = projectile.Entropy().OnProj.ToProj().Center;
            }
            Texture2D tx;
            if (projectile.Entropy().DI)
            {
                lightColor = new Color(230, 230, 150);
            }
            if (projectile.Entropy().daTarget)
            {
                lightColor = Color.Black;
            }
            if (projectile.Entropy().gh && projectile.owner.ToPlayer().Entropy().GodHeadVisual)
            {
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Extra/godhead").Value;
                float rsize = (projectile.width + projectile.height) / 2 * 6;
                if (rsize < 128)
                {
                    rsize = 128;
                }
                if (rsize > 600)
                {
                    rsize = 600;
                }
                SpriteBatch sb = Main.spriteBatch;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(tx, projectile.Center - Main.screenPosition, null, new Color(248, 246, 165) * 0.4f, 0, tx.Size() / 2, rsize / 64, SpriteEffects.None, 0);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                
            }
            if (projectile.Entropy().Lightning) {
                float size = 4;
                float sizej = size / odp2.Count;
                Color cl = new Color(250, 250, 255);
                for (int i = odp2.Count - 1; i >= 1; i--)
                {
                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, this.odp2[i], this.odp2[i - 1], cl * ((float)i / (float)odp2.Count), size);
                    size -= sizej;
                }
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Extra/LightningArrow").Value;
                float x = 0f;
                for (int i = 0; i < projectile.Entropy().odp.Count; i++)
                {
                    Main.spriteBatch.Draw(tx, projectile.Entropy().odp[i] - Main.screenPosition, null, Color.White * x * 0.6f, projectile.Entropy().odr[i], new Vector2(tx.Width, tx.Height) / 2, projectile.scale, SpriteEffects.None, 0);
                    x += 1 / 7f;
                }
                Main.spriteBatch.Draw(tx, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, projectile.scale, SpriteEffects.None, 0);

                return false;
            }
            if (vdtype >= 0)
            {
                Color color = Color.Purple;
                if (vdtype == 1)
                {
                    color = Color.IndianRed;
                }
                if (vdtype == 2)
                {
                    color = Color.DeepSkyBlue;
                }
                if (vdtype == 3)
                {
                    color = Color.Black;
                }
                if (vdtype == 4)
                {
                    color = Color.DarkRed;
                }
                float size = 5;
                float sizej = size / odp2.Count;
                for (int i = odp2.Count - 1; i >= 1; i--)
                {
                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Extra/white").Value, this.odp2[i], this.odp2[i - 1], color * ((float)i / (float)odp2.Count), size);
                    size -= sizej;
                }

            }
            if (rpBow)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                lightColor = Color.White;
                Texture2D txx = TextureAssets.Projectile[projectile.type].Value;
                float rot = 0;
                for (int i = 0; i < 8; i++)
                {
                    Main.spriteBatch.Draw(txx, projectile.position + rot.ToRotationVector2() * 2 - Main.screenPosition, null, lightColor, projectile.rotation, new Vector2(txx.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);
                    rot += MathHelper.Pi * 2f / 8f;
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(txx, projectile.position - Main.screenPosition, null, lightColor, projectile.rotation, new Vector2(txx.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);

                return false;
            }
            return true;
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (vdtype == 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.position, projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(180)).RotatedByRandom(35) * 16, projectile.type, ((int)(projectile.damage * 0.7f)), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                }
            }
            if (rpBow && Main.myPlayer == projectile.owner)
            {
                for(int i = 0; i < 3; i++) {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, new Vector2(30, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2), ModContent.ProjectileType<Lightning>(), (int)(projectile.damage * 0.3f), 4, projectile.owner, 0, 0, (Main.rand.NextBool(8) ? 1 : 0));
                }
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (vdtype == 3)
            {
                EGlobalNPC.AddVoidTouch(target, 240, 10, 800, 16);
            }
            if (GWBow)
            {
                EGlobalNPC.AddVoidTouch(target, 160, 6, 600, 20);
            }
            if (projectile.Entropy().Lightning && projectile.penetrate >= 5)
            {
                projectile.velocity *= 1.4f;
            }
            if (projectile.owner != -1 && projectile.friendly)
            {
                EModPlayer plr = projectile.owner.ToPlayer().Entropy();
                if (plr.holyMoonlight && plr.HMRegenCd <= 0)
                {
                    if (plr.MagiShield <= 0)
                    {
                        plr.HMRegenCd = 40;
                        projectile.owner.ToPlayer().Heal(projectile.owner.ToPlayer().statManaMax2 / 100);
                    }
                }
                if (plr.VFHelmMagic)
                {
                    var player = projectile.owner.ToPlayer();
                    if (player.HasBuff(BuffID.ManaSickness))
                    {
                        for (int i = 0; i < player.buffType.Length; i++)
                        {
                            if (player.buffType[i] == BuffID.ManaSickness)
                            {
                                player.buffTime[i] -= 30;
                                if (player.buffTime[i] < 0)
                                {
                                    player.buffTime[i] = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
