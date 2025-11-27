using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.AzafureMissleLauncher
{
    public class AzafureTacticalRadio : ModItem, IAzafureEnhancable
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.damage = 80;
            Item.mana = 0;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MissileSilo>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.FindSentryRestingSpot(type, out int XPosition, out int YPosition, out int YOffset);
            YOffset -= 16;
            position = new Vector2((float)XPosition, (float)(YPosition - YOffset));
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, 20f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellIndustrialComponents>(8)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class MissileSilo : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 22;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }
        public int ShootDelay = 60;
        public int ShootCount = 0;
        public int ShootCooldown = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            var target_ = Projectile.FindMinionTarget(10000);
            if(target_ != null && target != target_)
            {
                Num = 0;
            }

            target = target_;
            if (target != null)
            {
                Num += Num < 1 ? 0.05f : 0;
                if (ShootCount <= 0)
                {
                    ShootDelay--;
                }
                else
                {
                    ShootCooldown--;
                    if (ShootCooldown <= 0)
                    {
                        ShootCooldown = 12;
                        ShootCount--;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -46), Vector2.UnitY * -78, ModContent.ProjectileType<AzafureRadioMissile>(), Projectile.damage, 2, Projectile.owner, target.Center.X, target.Center.Y);
                        }
                    }
                }
                targetPos = target.Center;
                if (ShootDelay <= 0)
                {
                    ShootDelay = 60;
                    ShootCount = player.AzafureEnhance() ? 3 : 2;
                }
            }
            else
            {
                Num -= Num > 0 ? 0.05f : 0;
                ShootCount = 0;
                ShootCooldown = 0;
                if (ShootDelay > 0)
                    ShootDelay--;
            }
            

        }
        public float Num = 0;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return false;
        }
        public NPC target = null;
        public Vector2 targetPos = Vector2.Zero;
        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, 0, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            Texture2D t1 = this.getTextureAlt("Target");
            Texture2D t2 = this.getTextureAlt("Target2");
            if(Num > 0)
            {
                Main.spriteBatch.Draw(t1, targetPos - Main.screenPosition, null, Color.White * Num, 0, t1.Size() / 2f, 1, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(t2, targetPos - Main.screenPosition, null, Color.White * Num, CEUtils.Parabola(Num * 0.5f, 3.141592f), t2.Size() / 2f, 1, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
    public class AzafureRadioMissile: ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.timeLeft = 49;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public NPC target = null;
        public Vector2 targetPos => new Vector2(Projectile.ai[0], Projectile.ai[1]);
        public override void AI()
        {
            if (target != null && !target.active)
                target = null;
            if (Projectile.ai[2] == 0)
            {
                target = CEUtils.FindTarget_HomingProj(Projectile, targetPos, 10000);
                if (Projectile.GetOwner().MinionAttackTargetNPC >= 0) {
                    var npc = Projectile.GetOwner().MinionAttackTargetNPC.ToNPC();
                    if(npc.active)
                    {
                        target = npc;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    Vector2 center = Projectile.Center - new Vector2(0, Projectile.velocity.Y / 2f);
                    EParticle.spawnNew(new LightAlt() { ScaleAdd = new Vector2(1f, 0) }, center, Vector2.Zero, Color.OrangeRed, 0.36f, 1, true, BlendState.Additive, 0, 18);
                    EParticle.spawnNew(new LightAlt() { ScaleAdd = new Vector2(1f, 0) }, center, Vector2.Zero, Color.White, 0.25f, 1, true, BlendState.Additive, 0, 18);
                }
                CEUtils.PlaySound("aprclaunch", Main.rand.NextFloat(1, 1.4f), Projectile.Center, 16);
                for (float i = 0.5f; i < 1; i += 0.025f)
                {
                    EParticle.NewParticle(new Smoke() { timeleftmax = 18, Lifetime = 18 }, Projectile.Center - Projectile.velocity * (1 - i), CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.05f, 0.07f), 0.5f, true, BlendState.Additive, CEUtils.randomRot());
                }
            }
            else
            {
                for (float i = 0; i < 1; i+=0.025f)
                {
                    EParticle.NewParticle(new Smoke() { timeleftmax = 18, Lifetime = 18 }, Projectile.Center - Projectile.velocity * (1 - i), CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.05f, 0.07f), 0.5f, true, BlendState.Additive, CEUtils.randomRot());
                }
            }
            Projectile.ai[2]++;
            if (Projectile.ai[2] == 25)
            {
                Vector2 pos = targetPos;
                if (target != null)
                {
                    pos = target.Center + target.velocity * 24;
                }
                Projectile.Center = pos + Projectile.velocity * 25;
                Projectile.velocity *= -1;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.PlaySound("explosionbig", 0.8f, Projectile.Center, 4, 1.4f);
            CEUtils.PlaySound("pulseBlast", 0.5f, Projectile.Center, 4, 1.4f);
            int lifetime = 20;
            Vector2 center = Projectile.Center;
            EParticle.spawnNew(new ERing() { LineWidth = 120 }, center, Vector2.Zero, new Color(255, 140, 140), 300, 1, true, BlendState.Additive, 0, 16);
            for(int i = 0; i < 2; i++)
            {
                EParticle.spawnNew(new LightAlt() { ScaleAdd = new Vector2(1, 0) }, center, Vector2.Zero, Color.Firebrick, 0.6f, 1f, true, BlendState.Additive, 0, lifetime);
                EParticle.spawnNew(new LightAlt() { ScaleAdd = new Vector2(1, 0) }, center, Vector2.Zero, Color.White, 0.3f, 1f, true, BlendState.Additive, 0, lifetime);
                EParticle.spawnNew(new LightAlt() { ScaleAdd = new Vector2(1, 0) }, center, Vector2.Zero, Color.Firebrick, 0.6f, 1, true, BlendState.Additive, MathHelper.PiOver2, lifetime);
                EParticle.spawnNew(new LightAlt() { ScaleAdd = new Vector2(1, 0) }, center, Vector2.Zero, Color.White, 0.3f, 1, true, BlendState.Additive, MathHelper.PiOver2, lifetime);
            }
            GeneralParticleHandler.SpawnParticle(new PulseRing(center, Vector2.Zero, Color.OrangeRed, 0.1f, 3f, lifetime));
            ScreenShaker.AddShake(new ScreenShaker.ScreenShake(Vector2.Zero, 6));
            void onhit(NPC target, NPC.HitInfo info, int damage)
            {
                target.AddBuff<MechanicalTrauma>(300);
            }
            ((CommonExplotionFriendly)CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.GetOwner(), Projectile.Center, Projectile.damage, 260, Projectile.DamageType).ModProjectile).onHitAction = onhit;
            ;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }
}
