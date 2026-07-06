using CalamityEntropy.Content.Particles.CalamityPorts;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using InnoVault.PRT;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class MineBox : ModItem
    {
        public static int BaseDamage = 75;
        public static float Dmg = 0.07f;
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
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
                .AddRecipeGroup(CERecipeGroups.IronBar, 8)
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
                //矿井爆开,CalamityPorts的PulseRing/DetailedExplosion+AltSpark,数值没动
                PRTLoader.NewParticle<PRT_DirectionalPulseRing>(Projectile.Center, Vector2.Zero, Color.Orange, 0.1f).Configure(new Vector2(2f, 2f), 0, 0.78f, 46);
                PRTLoader.NewParticle<PRT_DetailedExplosionCal>(Projectile.Center, Vector2.Zero, Color.OrangeRed, 0f).Configure(Vector2.One, Main.rand.NextFloat(-5, 5), 0.63f, 30);
                for (int i = 0; i < 32; i++)
                {
                    PRTLoader.NewParticle<PRT_AltSpark>(Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(3, 12), Color.Lerp(Color.Red, Color.Orange, Main.rand.NextFloat()), Main.rand.NextFloat(0.8f, 1.4f)).Configure(false, Main.rand.Next(22, 32));
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
