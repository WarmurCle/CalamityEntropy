using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Items.Weapons.GrassSword;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace CalamityEntropy.Content.Items.Donator
{
    public class TlipocasScythe : RogueWeapon, IDonatorItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public string DonatorName => "Kino";
        public override float StealthDamageMultiplier => 4f;
        public override float StealthVelocityMultiplier => 1f;
        public override float StealthKnockbackMultiplier => 2f;
        public static int GetLevel()
        {
            //return Main.LocalPlayer.inventory[9].stack;
            int Level = 0;
            bool flag = true;
            void Check(bool f)
            {
                if (f && flag)
                {
                    Level++;
                }
                else
                {
                    flag = false;
                }
            }

            //16
            Check(NPC.downedBoss1);
            Check(NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind);
            Check(DownedBossSystem.downedSlimeGod);
            Check(Main.hardMode);
            Check(DownedBossSystem.downedBrimstoneElemental);
            Check(DownedBossSystem.downedCalamitasClone);
            Check(EDownedBosses.downedProphet);
            Check(DownedBossSystem.downedRavager);
            Check(NPC.downedAncientCultist);
            Check(NPC.downedMoonlord);
            Check(DownedBossSystem.downedSignus);
            Check(DownedBossSystem.downedPolterghast);
            Check(DownedBossSystem.downedDoG);
            Check(EDownedBosses.downedCruiser);
            Check(DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs);
            Check(DownedBossSystem.downedPrimordialWyrm);

            return Level;

        }
        public override void UpdateInventory(Player player)
        {
            if (throwType == -1)
                throwType = ModContent.ProjectileType<TlipocasScytheThrow>();
            Item.useTime = Item.useAnimation = 44 - GetLevel();
        }
        public override void HoldItem(Player player)
        {
            UpdateInventory(player);
        }
        public override void SetDefaults()
        {
            Item.width = 132;
            Item.height = 116;
            Item.damage = 22;
            Item.DamageType = CEUtils.RogueDC;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.ArmorPenetration = 32;
            Item.shoot = ModContent.ProjectileType<TlipocasScytheHeld>();
            Item.Entropy().tooltipStyle = 3;
            Item.Entropy().NameColor = new Color(160, 0, 0);
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(90, 0, 0);
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
            Item.Entropy().Legend = true;
        }
        public int swing = 0;
        public static int throwType = -1;
        public override bool AllowPrefix(int pre)
        {
            return false;
        }
        
        public static bool AllowDash() { return NPC.downedBoss1; }
        public static bool DashImmune() { return DownedBossSystem.downedSlimeGod; }
        public static bool AllowThrow() { return NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind; }
        public static bool AllowSpin() { return EDownedBosses.downedProphet; }
        public static bool DashUpgrade() { return DownedBossSystem.downedSignus; }
        public static bool AllowRevive() { return DownedBossSystem.downedYharon; }
        public static bool AllowVoidEmpowerment() { return EDownedBosses.downedNihilityTwin; }
        public override bool AltFunctionUse(Player player)
        {
            if (AllowThrow())
            {
                return true;
            }
            return false;
        }
        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.HasBuff<VoidEmpowerment>())
            {
                damage = (int)(damage * 1.25f);
            }
            if (player.altFunctionUse == 2)
            {
                velocity *= 0.46f;
                type = throwType;
            }
            if (AllowDash() && player.controlUp && !player.HasCooldown(TlipocasScytheSlashCooldown.ID))
            {
                player.AddCooldown(TlipocasScytheSlashCooldown.ID, 7 * 60);
                Projectile.NewProjectile(source, position, velocity.normalize() * 1000 * (DashUpgrade() ? 1.33f : 1), ModContent.ProjectileType<TSSlash>(), damage * 5, knockback, player.whoAmI, swing == 0 ? 1 : -1);
            }
            else
            {
                if (player.ownedProjectileCounts[throwType] > 0)
                    return false;
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, swing == 0 ? 1 : -1);
                if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                swing = 1 - swing;
            }
            return false;
        }

        public override void AddRecipes()
        {
        }
    }
    
    public class TSSlash : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC);
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("slice", 1, target.Center);
            EParticle.NewParticle(new MultiSlash() { xadd = 1f, lx = 1f, endColor = Color.Red, spawnColor = Color.IndianRed }, target.Center, Vector2.Zero, Color.IndianRed, 1, 1, true, BlendState.Additive, 0);
            if(TlipocasScythe.DashUpgrade())
            {
                target.AddBuff<MarkedforDeath>(20 * 60);
                target.AddBuff<WhisperingDeath>(20 * 60);
            }
        }
        public bool MovePlayer = true;
        public override void AI()
        {
            if(TlipocasScythe.DashImmune())
            {
                Projectile.GetOwner().Entropy().immune = 4;
            }
            if (Projectile.localAI[1]++ == 0)
            {
                CEUtils.PlaySound("AbyssalBladeLaunch", 1, Projectile.Center);
            }
            Player player = Projectile.GetOwner();
            if (MovePlayer)
            {
                Vector2 odp = player.Center;
                player.Center = Vector2.Lerp(Projectile.Center + Projectile.velocity, Projectile.Center, Projectile.timeLeft / 10f);
                if (CEUtils.IsPlayerStuck(player))
                {
                    MovePlayer = false;
                    player.Center = odp;
                }
            }
            if (Projectile.timeLeft == 10)
            {
                player.Entropy().screenShift = 1;
                player.Entropy().screenPos = player.Center;
                Vector2 top = Projectile.Center;
                Vector2 sparkVelocity2 = Projectile.velocity * 0.08f;
                Vector2 rd = Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2);
                int sparkLifetime2 = 24;
                float sparkScale2 = 1.5f;
                for (float i = 0; i < 1; i += 0.01f)
                {
                    Color sparkColor2 = Color.Lerp(Color.Red, Color.DarkRed, Main.rand.NextFloat(0, 1));
                    var spark = new AltSparkParticle(top + CEUtils.randomPointInCircle(32), sparkVelocity2 * Main.rand.NextFloat(), false, (int)(sparkLifetime2), sparkScale2 * Main.rand.NextFloat(0.6f, 1), sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }

        public override bool ShouldUpdatePosition() { return false; }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity, targetHitbox, 32);
        }
    }

    public class TlipocasScytheHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/TlipocasScythe";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.light = 0.2f;
            Projectile.MaxUpdates = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public List<float> oldRots = new List<float>();
        public List<float> oldScale = new List<float>();
        public float counter = 0;
        public bool flagS = true;
        public bool flag = true;
        public bool Canhit = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 1 && TlipocasScythe.AllowVoidEmpowerment())
            {
                int VETime = EDownedBosses.downedCruiser ? 30 : 15;
                VETime *= 60;
                Projectile.GetOwner().AddBuff(ModContent.BuffType<VoidEmpowerment>(), VETime);
            }
            if (target.townNPC && target.life <= 0)
            {
                SoundEngine.PlaySound(PerforatorHive.DeathSound, target.Center);
                Item.NewItem(target.GetSource_Death(), target.getRect(), new Item(ItemID.SilverCoin, 10));
            }
            if (flagS)
            {
                if(TlipocasScythe.AllowSpin() && Projectile.Calamity().stealthStrike)
                {
                    Projectile.GetOwner().Heal(DownedBossSystem.downedPolterghast ? 30 : 15);
                }
                CEUtils.PlaySound("voidseekershort", 1, target.Center, 6, CEUtils.WeapSound);
                flagS = false;
                if (!target.Organic())
                {
                    CEUtils.PlaySound("metalhit", Main.rand.NextFloat(0.8f, 1.2f) / Projectile.ai[1], target.Center, 6);
                }
            }
            if (DownedBossSystem.downedRavager)
            {
                if (DownedBossSystem.downedProvidence)
                {
                    target.AddBuff<Shred>(60 * 5);
                }
                else
                {
                    target.AddBuff<HeavyBleeding>(60 * 5);
                }
            }
            else
            {
                target.AddBuff<BurningBlood>(60 * 5);
            }
            Color impactColor = Color.Red;
            float impactParticleScale = Main.rand.NextFloat(1.4f, 1.6f);

            SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.LawnGreen, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);


            float sparkCount = 8 + TlipocasScythe.GetLevel();
            for (int i = 0; i < sparkCount; i++)
            {
                float p = Main.rand.NextFloat();
                Vector2 sparkVelocity2 = (target.Center - Projectile.Center).normalize().RotatedByRandom(p * 0.4f) * Main.rand.NextFloat(6, 20 * (2 - p)) * (1 + TlipocasScythe.GetLevel() * 0.1f);
                int sparkLifetime2 = (int)((2 - p) * 16);
                float sparkScale2 = 0.6f + (1 - p);
                sparkScale2 *= (1 + Bramblecleave.GetLevel() * 0.06f);
                Color sparkColor2 = Color.Lerp(Color.DarkRed, Color.IndianRed, p);
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : Color.DarkRed);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 120, 120), 1, 1, true, BlendState.Additive, 0, 6);

        }
        public override bool? CanHitNPC(NPC target)
        {
            return Canhit;
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            float progress = (counter / (player.itemAnimationMax * Projectile.MaxUpdates));
            counter++;
            if(counter > Projectile.MaxUpdates * 4)
            {
                player.heldProj = Projectile.whoAmI;
            }
            int dir = (int)Projectile.ai[0] * Math.Sign(Projectile.velocity.X);

            float ySc = 0.6f;
            ProjScale = 1.4f + TlipocasScythe.GetLevel() * 0.04f;
            ProjScale *= (1 + Projectile.ai[1] * 0.5f);
            if (Projectile.ai[1] == 1)
            {
                if (progress < 0.36f)
                {
                    counter = (int)(0.36f * player.itemAnimationMax * Projectile.MaxUpdates + 1);
                }
            }
            if(Projectile.Calamity().stealthStrike)
            {
                ySc = 0.34f;
                ProjScale *= 2f;
            }
            float r = 3.6f;
            float r1 = 0.5f;
            float r2 = 0.6f;
            float pn = 0.36f;
            if(progress >= pn && flag)
            {
                Canhit = true;
                flag = false;
                CEUtils.PlaySound("scytheswing", Main.rand.NextFloat(1.6f, 1.8f), Projectile.Center, 4, CEUtils.WeapSound);
            }
            if(progress < pn)
            {
                Projectile.rotation = (-r/2f - CEUtils.GetRepeatedCosFromZeroToOne(progress / pn, 2) * r1) * dir;
            }
            else 
            {
                Projectile.rotation = (-r/2f - r1 + CEUtils.GetRepeatedParaFromZeroToOne((progress - pn) / (1 - pn), 4) * (r + r2)) * dir;
            }
            scale = (Projectile.rotation.ToRotationVector2() * new Vector2(1, ySc)).Length();
            Projectile.rotation = (Projectile.rotation.ToRotationVector2() * new Vector2(1, ySc)).ToRotation() + Projectile.velocity.ToRotation();
            if(progress > 1)
            {
                Projectile.Kill();
            }
            player.SetHandRotWithDir(Projectile.rotation, Math.Sign(Projectile.velocity.X));
            oldScale.Add(scale);
            oldRots.Add(Projectile.rotation);
            if(oldRots.Count > 50)
            {
                oldRots.RemoveAt(0);
                oldScale.RemoveAt(0);
            }
            Projectile.Center = player.MountedCenter + new Vector2(player.direction * -12, 0);

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 146 * scale * ProjScale, targetHitbox, (int)(36 * ProjScale));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D trail = CEUtils.getExtraTex("StreakGoop");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);

            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(168 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(90 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue((Color.Firebrick).ToVector4());
                    shader.Parameters["color2"].SetValue((Color.OrangeRed).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            int dir = (int)(Projectile.ai[0]) * Math.Sign(Projectile.velocity.X);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * ProjScale * scale, effect);

   
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        
        public float alpha = 1;
        public float ProjScale = 1;
        public float scale = 1;
    }
    public class TlipocasScytheThrow : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/TlipocasScythe";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.light = 0.2f;
            Projectile.MaxUpdates = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 64;
        }

        public List<Vector2> oldPos = new List<Vector2>();
        public List<float> oldRots = new List<float>();
        public List<float> oldScale = new List<float>();
        public float counter = 0;
        public bool flagS = true;
        public bool flag = true;
        public bool StickOnMouse = false;
        public bool RightLast = true;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Projectile.numHits == 1)
            {
                if (TlipocasScythe.AllowSpin() && Projectile.Calamity().stealthStrike)
                {
                    Projectile.GetOwner().Heal(DownedBossSystem.downedPolterghast ? 30 : 15);
                }
            }
            if(counter < 16 * 10)
            {
                counter = 16 * 10;
            }
            CEUtils.PlaySound("voidseekershort", 1, target.Center, 6, CEUtils.WeapSound * 0.4f);
            if (!target.Organic())
            {
                CEUtils.PlaySound("metalhit", Main.rand.NextFloat(0.8f, 1.2f) / Projectile.ai[1], target.Center, 6, CEUtils.WeapSound * 0.4f);
            }
            if (DownedBossSystem.downedRavager)
            {
                if (DownedBossSystem.downedProvidence)
                {
                    target.AddBuff<Shred>(60 * 5);
                }
                else
                {
                    target.AddBuff<HeavyBleeding>(60 * 5);
                }
            }
            else
            {
                target.AddBuff<BurningBlood>(60 * 5);
            }
            Color impactColor = Color.Red;
            float impactParticleScale = Main.rand.NextFloat(1.4f, 1.6f);

            SparkleParticle impactParticle = new SparkleParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f), Vector2.Zero, impactColor, Color.LawnGreen, impactParticleScale, 8, 0, 2.5f);
            GeneralParticleHandler.SpawnParticle(impactParticle);


            float sparkCount = 14 + TlipocasScythe.GetLevel();
            for (int i = 0; i < sparkCount; i++)
            {
                float p = Main.rand.NextFloat();
                Vector2 sparkVelocity2 = CEUtils.randomRot().ToRotationVector2().RotatedByRandom(p * 0.4f) * Main.rand.NextFloat(6, 20 * (2 - p)) * (1 + TlipocasScythe.GetLevel() * 0.1f);
                int sparkLifetime2 = (int)((2 - p) * 16);
                float sparkScale2 = 0.6f + (1 - p);
                sparkScale2 *= (1 + Bramblecleave.GetLevel() * 0.06f);
                Color sparkColor2 = Color.Lerp(Color.DarkRed, Color.IndianRed, p);
                if (Main.rand.NextBool())
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, (int)(sparkLifetime2 * (1.2f)), sparkScale2 * (1.4f), sparkColor2);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : Color.DarkRed);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 120, 120), 1, 1, true, BlendState.Additive, 0, 6);

        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StickOnMouse);
            writer.Write(counter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StickOnMouse = reader.ReadBoolean();
            counter = reader.ReadSingle();
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            ProjScale = 1.4f + TlipocasScythe.GetLevel() * 0.02f;
            counter++;
            if(counter > 16 * (StickOnMouse ? 66 : 16))
            {
                Projectile.velocity *= 0.996f;
                Projectile.velocity += (player.Center - Projectile.Center).normalize() * 0.05f;
                if(CEUtils.getDistance(Projectile.Center, player.Center) < Projectile.velocity.Length() + 64)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (StickOnMouse)
                {
                    Projectile.velocity *= 0.98f;
                    Projectile.velocity += (player.Calamity().mouseWorld - Projectile.Center).normalize() * 0.1f;
                }
            }
            player.Calamity().mouseWorldListener = true;

            Projectile.rotation += 0.04f;
            oldScale.Add(1);
            oldPos.Add(Projectile.Center);
            oldRots.Add(Projectile.rotation);
            if(oldRots.Count > 16 * 6)
            {
                oldRots.RemoveAt(0);
                oldScale.RemoveAt(0);
                oldPos.RemoveAt(0);
            }
            if(Main.myPlayer == Projectile.owner)
            {
                if(Main.mouseLeft && !player.HasCooldown(TeleportSlashCooldown.ID))
                {
                    player.AddCooldown(TeleportSlashCooldown.ID, 15 * 60);
                    player.Entropy().screenShift = 1f;
                    player.Entropy().screenPos = player.Center;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, (Projectile.Center - player.Center).normalize() * 16, ModContent.ProjectileType<TlipocasScytheHeld>(), Projectile.damage * 16, Projectile.knockBack, player.whoAmI, 1, 1);
                    if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].Calamity().stealthStrike = true;
                    }
                    Projectile.Kill();
                    player.Center = Projectile.Center;
                }
                if(!StickOnMouse && Main.mouseRight && TlipocasScythe.AllowSpin() && !RightLast)
                {
                    counter = 0;
                    StickOnMouse = true;
                    CEUtils.SyncProj(Projectile.whoAmI);
                }
                RightLast = Main.mouseRight;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Center.getRectCentered((int)(130 * ProjScale), (int)(130 * ProjScale)).Intersects(targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D trail = CEUtils.getExtraTex("StreakGoop");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);

            {
                for (int i = 0; i < oldRots.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(84 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(50 * Projectile.scale * oldScale[i] * ProjScale, 0).RotatedBy(oldRots[i])),
                          new Vector3((i) / ((float)oldRots.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue((Color.Firebrick).ToVector4());
                    shader.Parameters["color2"].SetValue((Color.OrangeRed).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            Vector2 origin = tex.Size() / 2f;
            float rot = Projectile.rotation;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * ProjScale * scale, SpriteEffects.None);


            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public float alpha = 1;
        public float ProjScale = 1;
        public float scale = 1;
    }
}
