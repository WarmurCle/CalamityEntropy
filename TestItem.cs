using CalamityEntropy.Content.Particles;
using CalamityMod.Items;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    internal class TestItem : ModItem
    {
        public override string Texture => "CalamityEntropy/icon";

        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 80;
            Item.damage = 9999;
            Item.DamageType = DamageClass.Default;
            Item.useAnimation = Item.useTime = 13;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateInventory(Player player)
        {

        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }

        public override void HoldItem(Player player)
        {
        }

        public override bool? UseItem(Player player)
        {
            var prt = PRTLoader.NewParticle(new AbyssalLine(), player.Center, Vector2.Zero, Color.White);
            prt.Rotation = CEUtils.randomRot();
            return true;
        }
    }
}
