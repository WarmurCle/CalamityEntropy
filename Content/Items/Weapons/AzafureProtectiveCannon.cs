using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureProtectiveCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Summon;
            Item.width = 44;
            Item.height = 38;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.knockBack = 2;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<AzafureProtectiveCannonShot>();
            Item.shootSpeed = 2f;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.mana = 10;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }
        public static int Cooldown = 20 * 60;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            damage *= 16;
            player.AddCooldown(ProtectiveCannonCooldown.ID, Cooldown);
            position = Main.MouseWorld + new Vector2(-600, -2000);
            Vector2 vel = (Main.MouseWorld - position) / 10f;
            Projectile.NewProjectile(source, position, vel, type, damage, knockback, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasCooldown(ProtectiveCannonCooldown.ID);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 6)
                .AddRecipeGroup(RecipeGroupID.IronBar, 6)
                .AddIngredient(ItemID.Wire, 12)
                .AddIngredient<MysteriousCircuitry>()
                .AddIngredient<HellIndustrialComponents>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class AzafureProtectiveCannonShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 70;
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.localAI[0] > 60;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                CEUtils.PlaySound("Alarm", 1, Projectile.GetOwner().Center);
                EParticle.spawnNew(new APRCAlarm(), new Vector2(Projectile.ai[0], Projectile.ai[1]), Vector2.Zero, Color.White, 1, 1, true, BlendState.AlphaBlend, 0, Projectile.timeLeft / Projectile.MaxUpdates);
            }
            if (Projectile.localAI[0] == 60)
                CEUtils.PlaySound("aprclaunch", 1, Projectile.GetOwner().Center);

            if (ShouldUpdatePosition())
            {
                for (float i = 0; i < 1; i += 0.1f)
                {
                    EParticle.NewParticle(new Smoke() { timeleftmax = 26, Lifetime = 26 }, Projectile.Center - Projectile.velocity * i, CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.06f, 0.08f), 0.6f, true, BlendState.Additive, CEUtils.randomRot());
                    EParticle.NewParticle(new EMediumSmoke(), Projectile.Center + Projectile.velocity * i, new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.2f, 0.2f)), Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()), Main.rand.NextFloat(0.8f, 1.4f), 1, true, BlendState.AlphaBlend, CEUtils.randomRot());
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.PlaySound("explosionbig", 1, Projectile.Center, 4, 1.4f);
            CEUtils.PlaySound("pulseBlast", 0.6f, Projectile.Center, 4, 1.4f);
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Firebrick, 0.1f, 3.2f, 20));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Firebrick, 7.6f, 1, true, BlendState.Additive, 0, 24);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 5.4f, 1, true, BlendState.Additive, 0, 24);
            if (Projectile.owner == Main.myPlayer)
            {
                CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.owner.ToPlayer(), Projectile.Center, Projectile.damage, 278, Projectile.DamageType);
            }
            for (int i = 0; i < 64; i++)
            {
                var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Firework_Yellow);
                d.scale = 0.8f;
                d.velocity = CEUtils.randomPointInCircle(20);
                d.position += d.velocity * 4;
            }
            CEUtils.SetShake(Projectile.Center, 12, 6000);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if(!ShouldUpdatePosition())
            {
                return false;
            }
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }
}
