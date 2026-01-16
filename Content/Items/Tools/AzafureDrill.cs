using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Tools
{
    public class AzafureDrill : ModItem, IAzafureEnhancable
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 18;
            Item.damage = 7;
            Item.ArmorPenetration = 5;
            Item.knockBack = 0f;
            Item.useTime = 6;
            Item.useAnimation = 25;
            Item.pick = 70;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.tileBoost = -1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HellIndustrialComponents>(4).
                AddIngredient<DubiousPlating>(6).
                AddRecipeGroup(RecipeGroupID.IronBar, 6).
                AddTile(TileID.Anvils).
                Register();
        }

        public override void HoldItem(Player player)
        {
            Item.pick = player.AzafureEnhance() ? 100 : 70;
            Item.tileBoost = player.AzafureEnhance() ? 3 : -1;
            player.Calamity().mouseWorldListener = true;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;
            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 6f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2));

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }
        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }
}
