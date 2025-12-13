using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher
{
    public class Redemption : ModItem
    {
        public static int MaxStick => 8;
        public static int ExplodeRadius => 120;
        public float Charge = 0;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CharredMissleProj>(), BaseMissleProj.AmmoType, singleShotTime: 50, shotVelocity: 30f, hasAutoReuse: true);
            Item.width = 90;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 460;
            Item.knockBack = 4f;
            Item.UseSound = null;
            Item.value = Item.buyPrice(gold: 3); 
            Item.rare = ItemRarityID.Pink;
            Item.Entropy().tooltipStyle = 8;
            Item.Entropy().strokeColor = Color.DarkGreen;
            Item.Entropy().NameColor = Color.Orange;
            Item.Entropy().NameLightColor = Color.Orange * 0.4f;
            Item.channel = true;
        }

        #region Animations
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
            if (player.altFunctionUse != 2)
            {
                if (player.channel)
                {
                    player.itemTime = player.itemTimeMax;
                    player.itemAnimation = player.itemAnimationMax;
                    if (Charge < 1)
                    {
                        Charge += 0.04f;
                        if (Charge >= 1)
                        {
                            CEUtils.PlaySound("DeadSunShot", 2.5f, player.Center);
                            Charge = 1;
                        }
                    }
                }
                else
                {
                    if (Charge > 0)
                    {
                        if (Charge >= 1)
                        {
                            if (Main.myPlayer == player.whoAmI)
                            {
                                player.PickAmmo(Item, out int proj, out float speed, out int dmg, out float kb, out int ammo, false);
                                Shoot(player, (EntitySource_ItemUse_WithAmmo)player.GetSource_ItemUse_WithPotentialAmmo(Item, ammo), player.MountedCenter, (Main.MouseWorld - player.MountedCenter).normalize() * speed, proj, dmg, kb);
                            }
                        }
                        else
                        {
                            player.itemTime = 0;
                            player.itemAnimation = 0;
                        }
                        Charge = 0;
                    }
                }
            }
        }
        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem != Item)
                Charge = 0;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 76f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = -HoldoutOffset().Value;
            if (player.channel)
            {
                itemPosition -= itemRotation.ToRotationVector2() * Charge * 14;
                if (Charge < 1)
                    itemPosition += CEUtils.randomPointInCircle(4);
            }
            else if(!player.channel && player.itemTime > 0)
            {
                float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
                if(animProgress < 0.32f)
                    itemPosition += itemRotation.ToRotationVector2() * (float)Math.Pow((1 - animProgress / 0.32f), 2) * -32;
            }
            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (player.channel)
            {

            }
            else
            {
                if (animProgress < 0.5)
                    rotation += (-0.1f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            }
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-23f, -8f);
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AuricBar>(5)
                .AddIngredient<CosmiliteBar>(10)
                .AddIngredient<AscendantSpiritEssence>(15)
                .AddTile<CosmicAnvil>()
                .Register();
        }
        public static void OnKillAction(Projectile Projectile)
        {
            for (int i = 0; i < 128; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.YellowTorch);
                if(d < 6000)
                {
                    Main.dust[d].velocity = CEUtils.randomPointInCircle(15);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = 1.6f;
                }
            }
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if(player.altFunctionUse == 2)
            {
                Item.channel = false;
                Item.useTime = 80;
                Item.useAnimation = 80;
            }
            else
            {
                Item.channel = true;
                Item.useTime = 50;
                Item.useAnimation = 50;
            }
            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position += (new Vector2(54, -16) * new Vector2(1, player.direction)).RotatedBy(velocity.ToRotation());

            if (player.altFunctionUse == 2)
            {
                CEUtils.PlaySound("shotgun", Main.rand.NextFloat(2.2f, 2.4f), position, 6, 0.6f);
                Projectile.NewProjectile(source, position, velocity * 0.12f, ModContent.ProjectileType<RedemptionShootAlt>(), damage * 2, knockback * 4, player.whoAmI);
                return false;
            }
            if (!player.channel && Charge >= 1)
            {
                CEUtils.PlaySound("shotgun", Main.rand.NextFloat(2.2f, 2.4f), position, 6, 0.6f);
                List<int> buffs = new List<int>() { ModContent.BuffType<HolyFlames>(), ModContent.BuffType<GodSlayerInferno>(), ModContent.BuffType<Dragonfire>(), ModContent.BuffType<Plague>(), BuffID.Daybreak, ModContent.BuffType<SoulDisorder>() };

                for (int i = 0; i < 5; i++)
                {
                    int p = Projectile.NewProjectile(source, position, velocity * Main.rand.NextFloat(0.2f, 1), type, damage, knockback, player.whoAmI, MaxStick, ExplodeRadius);
                    p.ToProj().Entropy().applyBuffs.Add(buffs[Main.rand.Next(buffs.Count)]);
                    if (p.ToProj().ModProjectile is BaseMissleProj bmp)
                    {
                        bmp.winding += 0.6f;
                        bmp.Homing += 3.8f;
                        bmp.NoGrav = true;
                        bmp.MinVel = Item.shootSpeed;
                    }
                    p.ToProj().Entropy().OnKillActions += OnKillAction;
                    CEUtils.SyncProj(p);
                }
            }
            return false;
        }
    }
    public class RedemptionShootAlt : ModProjectile
    {
        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, 1);
            Projectile.width = Projectile.height = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.timeLeft = 16 * 60;
            Projectile.scale = 2f;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            if (Projectile.Distance(Main.LocalPlayer.Center) < 3200)
            {
                for (int i = 0; i < 6; i++)
                {
                    EParticle.NewParticle(new Smoke() { timeleftmax = 16, Lifetime = 16 }, Projectile.Center - Projectile.velocity * i / 6f, CEUtils.randomPointInCircle(0.5f), Color.Yellow, Main.rand.NextFloat(0.04f, 0.06f), 0.9f, true, BlendState.Additive, CEUtils.randomRot());
                    EParticle.NewParticle(new Smoke() { timeleftmax = 16, Lifetime = 16 }, Projectile.Center - Projectile.velocity * i / 6f, CEUtils.randomPointInCircle(0.5f), new Color(255, 255, 120), Main.rand.NextFloat(0.02f, 0.03f), 0.9f, true, BlendState.Additive, CEUtils.randomRot());
                }
            }
            if (Projectile.velocity.Length() < 24)
                Projectile.velocity *= 1.06f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            CEUtils.AddLight(Projectile.Center, Color.LightYellow);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.G = 255;
            lightColor.R = 255;
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.GetOwner().Heal(16);
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Yellow, 0.1f, 3f, 26));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Yellow, 4f, 1, true, BlendState.Additive, 0, 24);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 3f, 1, true, BlendState.Additive, 0, 24);
            CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.owner.ToPlayer(), Projectile.Center, Projectile.damage * 5, 250, Projectile.DamageType);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            CEUtils.PlaySound("explosionbig", 1.2f, Projectile.Center, 8, 0.5f);
            for (int i = 0; i < 300; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.YellowTorch);
                if (d < 6000)
                {
                    Main.dust[d].velocity = CEUtils.randomPointInCircle(32);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = Main.rand.NextFloat(1, 3);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            List<int> buffs = new List<int>() { ModContent.BuffType<HolyFlames>(), ModContent.BuffType<GodSlayerInferno>(), ModContent.BuffType<Dragonfire>(), ModContent.BuffType<Plague>(), BuffID.Daybreak, ModContent.BuffType<SoulDisorder>() };
            foreach (int i in buffs)
            {
                target.AddBuff(i, 5 * 60);
            }
        }
    }
}
