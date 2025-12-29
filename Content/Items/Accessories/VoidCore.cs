using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Enums;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class VoidCore : ModItem
    {
        public const int ShieldSlamDamage = 800;
        public const float ShieldSlamKnockback = 8f;
        public const int ShieldSlamIFrames = 18;
        public static int DashDelay = 18;
        public float charge = 0;
        public static int MaxShield = 180;
        public static int ShieldRecharge = 16 * 60;
        public static float CritDamage = 0.16f;
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.Entropy().VoidShieldVisual = !hideVisual;
            player.Entropy().VoidCoreItem = Item;
            modPlayer.DashID = VoidCoreDash.ID;
            player.dashType = 0;
            player.AddCritDamage(DamageClass.Generic, CritDamage);
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().VoidShieldVisual = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[S]", MaxShield.ToString());
            tooltips.Replace("[C]", CritDamage.ToPercent().ToString());
            
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NihilityShell>()
                .AddIngredient<AzafureDriverCore>()
                .AddIngredient<RuinousSoul>(6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    public class VoidCoreDash : PlayerDashEffect
    {

        public int Time;

        public bool PostHit;

        public new static string ID => "Void Core";

        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player)
        {
            return 36f;
        }

        public override void OnDashEffects(Player player)
        {
            Time = 0;
            PostHit = false;
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            Time += 2;
            if (Time > 44)
            {
                player.velocity.X *= 0.96f;
            }
            else
            {

            }
            if (Time < 36)
            {
                float num = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(2f, 2.5f, Time, clamped: true));
                for (float i = 0; i < 1; i += 0.1f)
                {
                    EParticle.spawnNew(new GlowSpark(), CEUtils.randomPointInCircle(18) + player.Center - player.velocity * i, -player.velocity.RotatedByRandom(0.32f) * Main.rand.NextFloat(0.4f, 0.6f), Color.Lerp(new Color(100, 100, 255), Color.LightBlue, Main.rand.NextFloat()), Main.rand.NextFloat(0.1f, 0.14f), 1, true, BlendState.Additive, -player.velocity.ToRotation(), 16);
                }
                for (int i = 0; i < 6; i++)
                {
                    float f = player.velocity.ToRotation() + (float)Time / 5f;
                    float num2 = (15f + (float)Math.Cos((float)Time / 3f) * 12f) * num;
                    Dust dust = Dust.NewDustPerfect(player.Center - player.velocity * 2f + f.ToRotationVector2().RotatedBy((float)i / 5f * (MathF.PI * 2f)) * num2, Main.rand.NextBool(5) ? DustID.BlueTorch : DustID.CosmicCarKeys);
                    dust.alpha = 140;
                    dust.noGravity = true;
                    dust.velocity = player.velocity * -0.8f;
                    dust.scale = Main.rand.NextFloat(1.7f, 2f);
                    dust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                    Dust dust2 = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) + player.velocity * 1.5f, Main.rand.NextBool(6) ? DustID.SparksMech : DustID.MinecartSpark, -player.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default(Color), Main.rand.NextFloat(1.7f, 1.9f));
                    dust2.alpha = 140;
                    dust.scale = dust2.scale = 0.7f;
                    dust2.noGravity = true;
                    dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
                }
                for (int i = 0; i < 3; i++)
                {
                    GeneralParticleHandler.SpawnParticle(new LineParticle(CEUtils.randomPointInCircle(18) + player.Center - player.velocity * Main.rand.NextFloat(), -player.velocity * Main.rand.NextFloat(0.4f, 0.6f), false, 8, Main.rand.NextFloat(0.6f, 1), Color.LightBlue));
                }
                EParticle.spawnNew(new AbyssalLine() { xadd = 0.84f, lx = 0.84f }, player.Center - player.velocity, Vector2.Zero, Color.LightBlue, 1, 1, true, BlendState.Additive, player.velocity.ToRotation(), 26);
                dashSpeed = 32f;
            }
        }

        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
        {
            if (!PostHit)
            {
                player.Calamity().GeneralScreenShakePower = 6f;
                PostHit = true;
            }
            NPC target = npc;

            EParticle.NewParticle(new ShineParticle(), npc.Center, Vector2.Zero, Color.Blue, 1.4f, 1, true, BlendState.Additive, 0, 12);
            EParticle.NewParticle(new ShineParticle(), npc.Center, Vector2.Zero, Color.White, 0.8f, 1, true, BlendState.Additive, 0, 12);
            float r2 = player.velocity.ToRotation();
            float r = player.velocity.ToRotation();
            EParticle.spawnNew(new AbyssalLine() { xadd = 1.4f, lx = 3.2f }, npc.Center, Vector2.Zero, new Color(30, 10, 50), 1, 1, true, BlendState.NonPremultiplied, r, 30);
            EParticle.spawnNew(new AbyssalLine() { xadd = 1.4f, lx = 3.2f }, npc.Center, Vector2.Zero, new Color(30, 10, 50), 1, 1, true, BlendState.NonPremultiplied, r2, 30);

            EParticle.spawnNew(new AbyssalLine() { xadd = 1.36f, lx = 3.2f }, npc.Center, Vector2.Zero, new Color(80, 40, 120), 1, 1, true, BlendState.NonPremultiplied, r, 30);
            EParticle.spawnNew(new AbyssalLine() { xadd = 1.36f, lx = 3.2f }, npc.Center, Vector2.Zero, new Color(80, 40, 120), 1, 1, true, BlendState.NonPremultiplied, r2, 30);

            EParticle.spawnNew(new AbyssalLine() { xadd = 1.34f, lx = 3f }, npc.Center, Vector2.Zero, Color.LightBlue, 1, 1, true, BlendState.Additive, r, 36);
            EParticle.spawnNew(new AbyssalLine() { xadd = 1.34f, lx = 3f }, npc.Center, Vector2.Zero, Color.LightBlue, 1, 1, true, BlendState.Additive, r2, 36);

            CEUtils.PlaySound("amethyst_break", 1, npc.Center, 6, 0.6f);
            CEUtils.PlaySound("AntivoidDash", 1, npc.Center, 6, 0.6f);
            CEUtils.PlaySound("ExoHit" + Main.rand.Next(1, 5), Main.rand.NextFloat(1.6f, 1.9f), target.Center, 6, 0.3f);
            hitContext.HitDirection = Math.Sign(player.velocity.X);
            hitContext.PlayerImmunityFrames = 16;
            int num = VoidCore.ShieldSlamDamage;
            hitContext.damageClass = DamageClass.Melee;
            hitContext.BaseDamage = player.ApplyArmorAccDamageBonusesTo(num);
            hitContext.BaseKnockback = 6f;
        }
    }
}
