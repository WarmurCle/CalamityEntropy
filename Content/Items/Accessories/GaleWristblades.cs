using CalamityMod;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class GaleWristblades : ModItem
    {
        public static int BaseDamage = 12;
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public static string ID = "GaleWristblades";

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip(ID, !hideVisual);
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual(ID);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldBar, 8)
                .AddIngredient(ItemID.CloudinaBottle)
                .AddIngredient(ItemID.Chain, 4)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.PlatinumBar, 8)
                .AddIngredient(ItemID.CloudinaBottle)
                .AddIngredient(ItemID.Chain, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class WristTornado : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC);
            Projectile.timeLeft = 120;
            Projectile.width = 64;
            Projectile.height = 128;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Projectile.Opacity = Projectile.timeLeft > 30 ? 1f : Projectile.timeLeft / 30f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Texture2D tex = Projectile.GetTexture();
            for (float i = 0; i <= 1; i += 0.01f)
            {
                Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0, -44 + i * 128) - Main.screenPosition, null, Color.White * (1.01f - i) * Projectile.Opacity, Main.GlobalTimeWrappedHourly * 10 + i * 4, tex.Size() / 2f, (1.02f - i), SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
