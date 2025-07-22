using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Content.Items.Donator
{
    public class ScorchingShoot : ModItem, IDonatorItem
    {
        public string DonatorName => "OVASA";
        public static int HitCount = 0;
        public override bool RangedPrefix()
        {
            return true;
        }
        public override void SetDefaults()
        {
            Item.width = 136;
            Item.height = 52;
            Item.damage = 1800;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.UseSound = CEUtils.GetSound("gunshot_large");
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 32f;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = 4;
            Item.Calamity().canFirePointBlankShots = true;
            Item.ArmorPenetration = 200;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit += 38;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -24);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            HitCount++;
            var p = Projectile.NewProjectile(source, position, velocity, type, damage * (HitCount > 6 ? 4 : 1), knockback, player.whoAmI).ToProj();
            CEUtils.SetShake(position, 4);
            p.GetGlobalProjectile<ScorchingGProj>().Active = true;

            if(HitCount > 5)
            {
                HitCount = 0;
                p.GetGlobalProjectile<ScorchingGProj>().Enhanced = true;
                Projectile.NewProjectile(source, position, velocity.normalize().RotatedBy(player.direction > 0 ? -1.9f : 1.9f) * 18, ModContent.ProjectileType<ScorchingShell>(), 0, 0, player.whoAmI);
                CEUtils.PlaySound("sniper_rifle", 1, position);
            }

            return false;
        }
        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 18f + new Vector2(0, 14);
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-10, 18);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (-0.4f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

            if (animProgress > 0.5f)
            {
                float backArmRotation = rotation + 0.52f * player.direction;

                Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                player.SetCompositeArmBack(true, stretch, backArmRotation);
            }

        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AngelicShotgun>()
                .AddIngredient<Auralis>()
                .AddIngredient<AuricBar>(5)
                .AddIngredient<DarksunFragment>(20)
                .AddTile<CosmicAnvil>()
                .Register();
        }
    }
    public class ScorchingGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Active = false;
        public bool Enhanced = false;
        public TrailGunShot trail = null;
        public bool flag = true;
        public override void AI(Projectile projectile)
        {
            if (Active && Enhanced)
            {
                if(flag)
                {
                    flag = false;
                    projectile.MaxUpdates *= 2;
                    projectile.penetrate = -1;
                }
                if (trail == null)
                {
                    trail = new TrailGunShot();
                    EParticle.spawnNew(trail, projectile.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.AlphaBlend);
                }
                else
                {
                    trail.Position = projectile.Center + projectile.velocity * projectile.MaxUpdates;
                    trail.Lifetime = 60;
                }
            }
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if(trail != null)
            {
                trail.Lifetime = 0;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Enhanced)
            {
                target.AddBuff(ModContent.BuffType<Dragonfire>(), 400);
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Active);
            binaryWriter.Write(Enhanced);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            Active = binaryReader.ReadBoolean();
            Enhanced = binaryReader.ReadBoolean();
        }
    }
}
