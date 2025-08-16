using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Enums;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class AzafureChargeShield : ModItem
    {
        public const int ShieldSlamDamage = 45;
        public const float ShieldSlamKnockback = 6f;
        public const int ShieldSlamIFrames = 12;
        public static int DashDelay = 45;
        public float charge = 0;
        public float maxCharge = 3.6f;
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 54;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.defense = 4;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (charge < maxCharge)
            {
                charge += 1f / 300f;
            }

            if (charge >= 1 || player.Entropy().AzDash > 0)
            {
                modPlayer.DashID = AzafureShieldDash.ID;
                player.dashType = 0;
            }
            player.Entropy().AzafureChargeShieldItem = Item;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (charge < maxCharge)
            {
                CEUtils.DrawChargeBar(scale * 1.2f, position + new Vector2(0, 18) * scale, ((float)charge / maxCharge), (charge < 1) ? Color.DarkOrange : Color.Orange);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AerialiteBar>(5)
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddIngredient<DubiousPlating>(10)
                .AddIngredient<HellIndustrialComponents>(6)
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class AzafureShieldDash : PlayerDashEffect
    {

        public int Time;

        public bool PostHit;

        public new static string ID => "Azafure Charge Shield";

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player)
        {
            return 22f;
        }

        public override void OnDashEffects(Player player)
        {
            Time = 0;
            PostHit = false;
            player.Entropy().AzDash = 3;
            (player.Entropy().AzafureChargeShieldItem.ModItem as AzafureChargeShield).charge -= 1;
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            Time += 2;
            player.Entropy().AzDash = 3;
            if (Time > 32)
            {
                player.velocity.X *= 0.94f;
            }
            else
            {
                int sparkLifetime = Main.rand.Next(22, 32);
                float sparkScale = Main.rand.NextFloat(1f, 1.4f);

                Color sparkColor = Color.Lerp(Color.OrangeRed, Color.Firebrick, Main.rand.NextFloat(0, 1));
                {
                    LineParticle spark = new LineParticle(player.Center + new Vector2(0, 24), -player.velocity.RotatedBy(-0.2f * player.direction) * 0.4f, false, (int)(sparkLifetime), sparkScale, sparkColor);
                    LineParticle spark2 = new LineParticle(player.Center + new Vector2(0, -24), -player.velocity.RotatedBy(0.2f * player.direction) * 0.4f, false, (int)(sparkLifetime), sparkScale, sparkColor);

                    GeneralParticleHandler.SpawnParticle(spark);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                {
                    sparkLifetime = 12;
                    sparkColor = Color.LightGoldenrodYellow;
                    LineParticle spark = new LineParticle(player.Center + new Vector2(0, 16), -player.velocity.RotatedBy(0.2f * player.direction) * 0.4f, false, (int)(sparkLifetime), sparkScale, sparkColor);
                    LineParticle spark2 = new LineParticle(player.Center + new Vector2(0, -16), -player.velocity.RotatedBy(-0.2f * player.direction) * 0.4f, false, (int)(sparkLifetime), sparkScale, sparkColor);

                    GeneralParticleHandler.SpawnParticle(spark);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
            float num = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(2f, 2.5f, Time, clamped: true));
            for (int i = 0; i < 3; i++)
            {
                float f = player.velocity.ToRotation() + (float)Time / 5f;
                float num2 = (15f + (float)Math.Cos((float)Time / 3f) * 12f) * num;
                Dust dust = Dust.NewDustPerfect(player.Center - player.velocity * 2f + f.ToRotationVector2().RotatedBy((float)i / 5f * (MathF.PI * 2f)) * num2, Main.rand.NextBool(5) ? DustID.Torch : DustID.FlameBurst);
                dust.alpha = 220;
                dust.noGravity = true;
                dust.velocity = player.velocity * 0.8f;
                dust.scale = Main.rand.NextFloat(1.7f, 2f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                Dust dust2 = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) + player.velocity * 1.5f, Main.rand.NextBool(6) ? DustID.SparksMech : DustID.MinecartSpark, -player.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default(Color), Main.rand.NextFloat(1.7f, 1.9f));
                dust2.alpha = 170;
                dust2.noGravity = true;
                dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
            }

            dashSpeed = 18f;
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            if (player.Entropy().AzChargeShieldSteamTime <= 0)
            {
                player.Entropy().AzChargeShieldSteamTime = 32;
            }
            if (!PostHit)
            {
                player.Calamity().GeneralScreenShakePower = 6f;
                PostHit = true;
            }
            NPC target = npc;
            for (int i = 0; i < 16; i++)
            {
                Vector2 top = target.Center + player.velocity.RotatedBy(MathHelper.PiOver2).normalize() * Main.rand.NextFloat(-12, 12);
                Vector2 sparkVelocity2 = -player.velocity.RotateRandom(0.4f) * 0.44f * Main.rand.NextFloat(0.3f, 1f);
                int sparkLifetime2 = Main.rand.Next(24, 28);
                float sparkScale2 = Main.rand.NextFloat(0.6f, 1.4f);
                var sparkColor2 = Color.Lerp(Color.Goldenrod, Color.Yellow, Main.rand.NextFloat(0, 1));

                LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            for (int i = 0; i < 16; i++)
            {
                Vector2 top = target.Center;
                Vector2 sparkVelocity2 = -player.velocity.RotateRandom(0.6f) * 0.6f * Main.rand.NextFloat(0.4f, 1f);
                int sparkLifetime2 = Main.rand.Next(24, 28);
                float sparkScale2 = Main.rand.NextFloat(1f, 1.8f);
                Color sparkColor2 = Color.Lerp(Color.Red, Color.Firebrick, Main.rand.NextFloat(0, 1));
                AltSparkParticle spark = new AltSparkParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            int hitDirection = player.direction;
            if (player.velocity.X != 0f)
            {
                hitDirection = Math.Sign(player.velocity.X);
            }
            CEUtils.PlaySound("ExoHit" + Main.rand.Next(1, 5), Main.rand.NextFloat(0.8f, 1.2f), target.Center);
            hitContext.HitDirection = hitDirection;
            hitContext.PlayerImmunityFrames = 12;
            int num = AzafureChargeShield.ShieldSlamDamage;
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = player.ApplyArmorAccDamageBonusesTo(num);
            hitContext.BaseKnockback = 6f;
        }
    }
}
