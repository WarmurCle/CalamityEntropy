using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class LunarPlank : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.damage = 200;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.UseSound = CEUtils.GetSound("powerwhip");
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<LunarPlankThrow>();
            Item.shootSpeed = 18f;
            Item.DamageType = CEUtils.RogueDC;
        }

        public override float StealthDamageMultiplier => 1.4f;
        public override float StealthVelocityMultiplier => 1.6f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    p.ToProj().netUpdate = true;
                }
                return false;
            }
            return true;
        }
    }
    public class LunarPlankThrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = CEUtils.RogueDC;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/LunarPlank";
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 16)
            {
                Projectile.velocity.Y += 0.4f;
            }
            GeneralParticleHandler.SpawnParticle(new MediumMistParticle(Projectile.Center, Projectile.velocity * 0.4f + CEUtils.randomPointInCircle(6), Color.LightBlue, Color.SkyBlue, Main.rand.NextFloat(0.8f, 1.2f), 255 - Main.rand.Next(100), Main.rand.NextFloat(-0.2f, 0.2f)));
            Projectile.rotation += Projectile.velocity.X * 0.012f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                CEUtils.PlaySound("LunarPlankExplode", 1, Projectile.Center);

                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.LightBlue, new Vector2(2f, 2f), 0, 0.02f, 1.36f, 32);
                GeneralParticleHandler.SpawnParticle(pulse);
                for (int i = 0; i < 12; i++)
                {
                    EParticle.NewParticle(new StarTrailParticle(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(10, 20), Color.White, Main.rand.NextFloat(0.6f, 1.2f), 1, true, BlendState.Additive, 0, 50);
                }
                for (int i = 0; i < 36; i++)
                {
                    Vector2 ver = CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(-10, 10);
                    BasePRT particle = new PRT_Light(Projectile.Center, ver
                        , Main.rand.NextFloat(0.6f, 1.2f), new Color(220, 180, 255), 80, 0.15f);
                    PRTLoader.AddParticle(particle);
                }
                CEUtils.PlaySound(Main.rand.NextBool() ? "scholarStaffImpact" : "scholarStaffImpact2", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
                CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.GetOwner(), Projectile.Center, Projectile.damage, 200, Projectile.DamageType);
                for(int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(24, 40), ModContent.ProjectileType<StarblightRogue>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
                }
            }
            else
            {
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.LightBlue, new Vector2(2f, 2f), 0, 0.02f, 0.85f * 0.4f, 18);
                GeneralParticleHandler.SpawnParticle(pulse);
                for (int i = 0; i < 5; i++)
                {
                    EParticle.NewParticle(new StarTrailParticle(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(8, 14), Color.White, Main.rand.NextFloat(0.6f, 1.2f), 1, true, BlendState.Additive, 0);
                }
                CEUtils.PlaySound(Main.rand.NextBool() ? "scholarStaffImpact" : "scholarStaffImpact2", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
            }
        }
    }
}
