using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class MineBox : ModItem
    {
        public static int BaseDamage = 25;
        public static float Dmg = 0.05f;
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.accessory = true;
        }

        public static string ID = "MineBox";

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip(ID, !hideVisual);
            player.GetDamage(CEUtils.RogueDC) += Dmg;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[D]", Dmg.ToPercent());
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual(ID);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DubiousPlating>(8)
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddIngredient(ItemID.Bomb, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class BoobyMine : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC);
            Projectile.timeLeft = 90;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void AI()
        {
            Projectile.light = (1 - Projectile.timeLeft / 90f);
            if (Projectile.timeLeft == 1)
            {
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.Orange, new Vector2(2f, 2f), 0, 0.1f, 0.78f, 46);
                GeneralParticleHandler.SpawnParticle(pulse);
                CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, Color.OrangeRed, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.63f, 30);
                GeneralParticleHandler.SpawnParticle(explosion2);
                for (int i = 0; i < 32; i++)
                {
                    var spark = new AltSparkParticle(Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(3, 12), false, Main.rand.Next(22, 32), Main.rand.NextFloat(0.8f, 1.4f), Color.Lerp(Color.Red, Color.Orange, Main.rand.NextFloat()));
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                CEUtils.PlaySound("ofhit", 1, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/ExoTwinsEject"), Projectile.Center);
            }
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.GetOwner(), Projectile.Center, Projectile.damage, 116, Projectile.DamageType);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float light = (1 - Projectile.timeLeft / 90f);
            CEUtils.DrawGlow(Projectile.Center, Color.Orange * light * 1.4f, ((1 - light) + 0.2f) * 6f, true);

            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }
}
