using System;
using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.DimDungeon;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using CalamityEntropy.Content.Projectiles.HBProj;
using CalamityEntropy.Content.Projectiles.SamsaraCasket;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.Projectiles.VoidEchoProj;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace CalamityEntropy.Common
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
        public int OnProj { get { return ttindex; } set { ttindex = value; } }
        public int flagTT = 0;
        public Vector2 playerPosL;
        public Vector2 playerMPosL;
        public bool daTarget = false;
        public int maxDmgUps = 0;
        public float dmgUp = 0.05f;
        public bool GWBow = false;
        public int dmgupcount = 10;
        public int vddirection = 1;
        public bool ToFriendly = false;
        public bool BarrenHoming = false;
        public bool EventideShot = false;

        public override GlobalProjectile Clone(Projectile from, Projectile to)
        {
            var p = to.Entropy();
            p.EventideShot = EventideShot;
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
            p.ToFriendly = ToFriendly;
            
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
            binaryWriter.Write(ToFriendly);
            binaryWriter.Write(EventideShot);
            binaryWriter.Write(projectile.Calamity().stealthStrike);
            binaryWriter.Write(zypArrow);
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
            ToFriendly = binaryReader.ReadBoolean();
            EventideShot = binaryReader.ReadBoolean();
            projectile.Calamity().stealthStrike = binaryReader.ReadBoolean();
            zypArrow = binaryReader.ReadBoolean();
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
        public bool dmgUpFrd = true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (Main.gameMenu)
            {
                return;
            }
            if (projectile.friendly)
            {
                if (projectile.owner.ToPlayer().Entropy().BarrenCard)
                {
                    if (projectile.DamageType == Util.CUtil.rogueDC)
                    {
                        BarrenHoming = true;
                    }
                }
            }
            if (source is EntitySource_Parent s)
            {
                if (s.Entity is Player player)
                {
                    projectile.velocity *= player.Entropy().shootSpeed;
                    
                }
                if (s.Entity is NPC np)
                {
                    ToFriendly = np.Entropy().ToFriendly;

                }
                if (s.Entity is Projectile pj)
                {
                    ToFriendly = pj.Entropy().ToFriendly;

                }
                if (ToFriendly)
                {
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = 14;
                    projectile.friendly = true;
                    projectile.hostile = false;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projectile.whoAmI);
                    }
                }
            }
            if (projectile.friendly)
            {
                if (projectile.ModProjectile is CrystylCrusherRay && SubworldSystem.IsActive<DimDungeonSubworld>())
                {
                    projectile.Kill();
                }
                if (projectile.friendly && projectile.owner >= 0)
                {
                    if (projectile.owner.ToPlayer().Entropy().VFHelmRanged)
                    {
                        maxDmgUps = 10;
                        dmgupcount = 16 * projectile.extraUpdates;
                    }
                }
                if (source is EntitySource_ItemUse && checkHoldOut && projectile.friendly && projectile.owner == Main.myPlayer && (projectile.ModProjectile is BaseIdleHoldoutProjectile || projectile.type == ModContent.ProjectileType<VoidEchoProj>() || projectile.type == ModContent.ProjectileType<HB>() || projectile.type == ModContent.ProjectileType<GhostdomWhisperHoldout>() || projectile.type == ModContent.ProjectileType<RailPulseBowProjectile>() || projectile.type == ModContent.ProjectileType<SamsaraCasketProj>() || projectile.type == ModContent.ProjectileType<OblivionHoldout>() || projectile.type == ModContent.ProjectileType<HadopelagicEchoIIProj>()))
                {
                    checkHoldOut = false;
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.active && p.type == ModContent.ProjectileType<TwistedTwinMinion>() && p.owner == Main.myPlayer)
                        {

                            int phd = Projectile.NewProjectile(Main.LocalPlayer.GetSource_ItemUse(Main.LocalPlayer.HeldItem), p.Center, Vector2.Zero, projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                            Projectile ph = phd.ToProj();
                            ph.scale *= 0.8f;
                            ph.Entropy().ttindex = p.identity;
                            p.netUpdate = true;
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
                                ttindex = plr.Entropy().twinSpawnIndex;
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
                            projectile.extraUpdates = (projectile.extraUpdates + 1) * 3 - 1;
                        }
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
        public Vector2? plrOldPos = null;
        public Vector2? plrOldVel = null;
        
        public override bool PreAI(Projectile projectile)
        {
            if (zypArrow)
            {
                NPC target = projectile.FindTargetWithinRange(360, false);
                if(target != null && counter > 12)
                {
                    projectile.velocity += (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * 1.6f;
                    projectile.velocity *= 0.92f;
                }
            }
            if((projectile.ModProjectile is ExobladeProj || projectile.type == ProjectileID.LastPrismLaser || projectile.type == ProjectileID.LastPrism) && projectile.owner.ToPlayer().Entropy().WeaponBoost > 0)
            {
                if(counter % 2 == 0)
                {
                    projectile.extraUpdates += projectile.owner.ToPlayer().Entropy().WeaponBoost;
                }
                else
                {
                    projectile.extraUpdates -= projectile.owner.ToPlayer().Entropy().WeaponBoost;
                }
            }
            if (BarrenHoming)
            {
                NPC target = projectile.FindTargetWithinRange(Math.Max(projectile.width, projectile.height) + 800, projectile.tileCollide);
                if (target != null)
                {
                    float homingSpeed = 0.46f;
                    projectile.velocity += (target.Center - projectile.Center).SafeNormalize(Vector2.Zero) * homingSpeed;
                    projectile.velocity *= 1 - homingSpeed * (projectile.tileCollide ? 0.05f : 0.02f);
                }
            }
            if (ToFriendly)
            {
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 14;
                projectile.friendly = true;
                projectile.hostile = false;
            }
            if (dmgUpFrd && ToFriendly)
            {
                dmgUpFrd = false;
                projectile.damage *= EGlobalNPC.TamedDmgMul;
                projectile.originalDamage *= EGlobalNPC.TamedDmgMul;
            }
            if (ToFriendly)
            {
                /*NPC t = null;
                float dist = 4600;
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && !n.dontTakeDamage)
                    {
                        if (Util.Util.getDistance(n.Center, projectile.Center) < dist)
                        {
                            t = n;
                            dist = Util.Util.getDistance(n.Center, projectile.Center);
                        }
                    }
                }
                if (t == null)
                {
                }
                else
                {
                    plrOldPos = Main.player[0].position;
                    plrOldVel = Main.player[0].velocity;
                    Main.player[0].Center = t.Center;
                    Main.player[0].velocity = t.velocity;
                }*/
            }
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
                dmgupcount = 24 * projectile.extraUpdates;
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
                    gwHoming += (6 - gwHoming) * 0.0004f;
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0).RotatedBy(Util.Util.rotatedToAngle(projectile.velocity.ToRotation(), (target.Center - projectile.Center).ToRotation(), gwHoming * projectile.velocity.Length(), true));
                }
            }
            if (projectile.Entropy().daTarget)
            {
                return false;
            }
            if (projectile.Entropy().ttindex >= 0)
            {
                if (netsnc)
                {
                    projectile.netUpdate = true;
                    netsnc = false;
                }
                playerPosL = projectile.owner.ToPlayer().Center;
                projectile.owner.ToPlayer().Center = ttindex.ToProj_Identity().Center;
            }
            projectile.Entropy().counter++;
            projectile.Entropy().odp.Add(projectile.Center);
            projectile.Entropy().odp2.Add(projectile.Center);
            projectile.Entropy().odr.Add(projectile.rotation);
            if (projectile.Entropy().odp.Count > (zypArrow ? 24 : 7))
            {
                projectile.Entropy().odp.RemoveAt(0);
                projectile.Entropy().odr.RemoveAt(0);
            }
            if (projectile.Entropy().odp2.Count > 22)
            {
                projectile.Entropy().odp2.RemoveAt(0);
            }
            if (projectile.arrow && projectile.owner >= 0)
            {
                if (projectile.owner.ToPlayer().HeldItem.type == ModContent.ItemType<Kinanition>())
                {
                    projectile.Entropy().Lightning = true;
                }
            }
            if (projectile.Entropy().Lightning && projectile.owner >= 0)
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
                    foreach (NPC n in Main.ActiveNPCs)
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
                                if (c % 4 == 0)
                                {
                                    int ydf = n.defense;
                                    n.defense =(int)MathHelper.Min(projectile.damage / 8 / 2 - 26, n.defense);
                                    Main.LocalPlayer.ApplyDamageToNPC(n, projectile.damage / 8, 0, 0, false, DamageClass.Generic, false);
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


        public bool evRu = true;

        public override void PostAI(Projectile projectile)
        {
            if (evRu && EventideShot)
            {
                evRu = false;
                projectile.extraUpdates = (1 + projectile.extraUpdates) * 6 - 1;
            }
            if (plrOldPos.HasValue)
            {
                Main.player[0].position = plrOldPos.Value;
                plrOldPos = null;
            }
            if (plrOldVel.HasValue)
            {
                Main.player[0].velocity = plrOldVel.Value;
                plrOldVel = null;
            }
            if (projectile.owner >= 0)
            {
                if (projectile.Entropy().ttindex >= 0)
                {
                    projectile.owner.ToPlayer().Center = playerPosL;
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
        public float gwHoming = 0.06f;

        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (projectile.ModProjectile != null)
            {
                projectile.ModProjectile.PostDraw(lightColor);
            }
            if (ttindex >= 0 && projectile.owner >= 0 && lastCenter != Vector2.Zero)
            {
                projectile.owner.ToPlayer().Center = lastCenter;
            }
        }
        public static float GetEventideDamageMultiplier(float radian, float maxR, float maxDmgMul)
        {
            if (radian >= maxR)
            {
                return 1f;
            }

            float ratio = radian / maxR;

            ratio = Math.Max(ratio, 0.0001f); 

            return Math.Min(maxDmgMul, (float)(1 / Math.Log(ratio + 1)));
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (EventideShot)
            {
                float maxR = MathHelper.ToRadians(60);
                float maxDmgMul = 2;
                
                float r = Util.Util.GetAngleBetweenVectors(projectile.velocity, (target.Center - projectile.Center));
                if(r < MathHelper.ToRadians(16))
                {
                    modifiers.SetCrit();
                }
                modifiers.SourceDamage *= GetEventideDamageMultiplier(r,  maxR, maxDmgMul);
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            this.projectile = projectile;
            if (ttindex >= 0 && projectile.friendly)
            {
                lastCenter = projectile.owner.ToPlayer().Center;
                projectile.owner.ToPlayer().Center = ttindex.ToProj_Identity().Center;
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
            if (projectile.Entropy().gh && projectile.friendly && projectile.owner >= 0 && projectile.owner.ToPlayer().Entropy().GodHeadVisual)
            {
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/godhead").Value;
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
                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, this.odp2[i], this.odp2[i - 1], cl * ((float)i / (float)odp2.Count), size);
                    size -= sizej;
                }
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/LightningArrow").Value;
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
                    Util.Util.drawLine(Main.spriteBatch, ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value, this.odp2[i], this.odp2[i - 1], color * ((float)i / (float)odp2.Count), size);
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
                    Main.spriteBatch.Draw(txx, projectile.Center + rot.ToRotationVector2() * 2 - Main.screenPosition, null, lightColor, projectile.rotation, new Vector2(txx.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);
                    rot += MathHelper.Pi * 2f / 8f;
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(txx, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, new Vector2(txx.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);

                return false;
            }
            if (zypArrow)
            {
                odp.Add(projectile.Center);
                odp.Reverse();
                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveRenderer.RenderTrail(odp, new(WidthFunction_Zyp, ColorFunction_Zyp, (_) => projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 30);
                odp.Reverse();
                odp.RemoveAt(odp.Count - 1);
                Texture2D txx = Util.Util.getExtraTex("WyrmArrow");
                
                Main.spriteBatch.Draw(txx, projectile.Center + new Vector2(0, 8) - Main.screenPosition + projectile.velocity.SafeNormalize(Vector2.UnitX) * 18, null, lightColor, projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(txx.Width / 2, 0), projectile.scale, (projectile.velocity.X < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally), 0);

                return false;
            }
            return true;
        }
        public Projectile projectile;
        internal Color ColorFunction_Zyp(float completionRatio)
        {
            float fadeToEnd = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos(-Main.GlobalTimeWrappedHourly * 3f) * 0.5f + 0.5f);
            float fadeOpacity = Utils.GetLerpValue(1f, 0.64f, completionRatio, true) * projectile.Opacity;
            Color colorHue = Color.LightSkyBlue;

            Color endColor = Color.Lerp(colorHue, Color.Turquoise, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * fadeOpacity;
        }

        internal float WidthFunction_Zyp(float completionRatio)
        {
            float expansionCompletion = (float)Math.Pow(1 - completionRatio, 3);
            return MathHelper.Lerp(0f, 12 * projectile.scale * projectile.Opacity, expansionCompletion);
        }

        public bool zypArrow = false;
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (projectile.friendly)
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
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, new Vector2(30, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2), ModContent.ProjectileType<Lightning>(), (int)(projectile.damage * 0.3f), 4, projectile.owner, 0, 0, (Main.rand.NextBool(8) ? 1 : 0));
                    }
                }
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.DamageType.CountsAsClass(DamageClass.Throwing) && projectile.Calamity().stealthStrike)
            {
                if (projectile.TryGetOwner(out var owner))
                {
                    if (owner.Entropy().MariviniumSet)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<WaterExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
                        }
                    }
                }
            }
            BarrenHoming = false;
            if (projectile.ModProjectile is MagnusBeam || projectile.ModProjectile is LunicBeam)
            {
                if (projectile.owner.ToPlayer().Entropy().WeaponBoost > 0)
                {
                    target.Entropy().applyMarkedOfDeath = 480 + projectile.owner.ToPlayer().Entropy().WeaponBoost * 260;
                }
            }
            if (vdtype == 3)
            {
                EGlobalNPC.AddVoidTouch(target, 240, 10, 800, 16);
            }
            if (GWBow)
            {
                EGlobalNPC.AddVoidTouch(target, 160, 6, 600, 20);
            }
            if (EventideShot)
            {
                float r = Util.Util.GetAngleBetweenVectors(projectile.velocity, (target.Center - projectile.Center));
                if(r < MathHelper.ToRadians(15))
                {
                    target.AddBuff(ModContent.BuffType<VoidVirus>(), 320);
                    Util.Util.PlaySound("voidseekercrit", 1, projectile.Center);
                    EGlobalNPC.AddVoidTouch(target, 160, 10, 800, 10);
                    projectile.owner.ToPlayer().Heal(16);
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center , Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), 0, 0, projectile.owner);
                }
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
                if (plr.VFHelmMagic && projectile.owner >= 0)
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
