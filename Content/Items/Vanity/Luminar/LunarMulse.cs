using CalamityEntropy.Content.Items.Donator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity.Luminar
{
    public class LunarMulse : ModItem, IDonatorItem
    {
        public string DonatorName => "玲瓏";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.accessory = true;
            Item.value = CalamityMod.Items.CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.vanity = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<LuminarMulsePlayer>().vanityEquipped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<LuminarMulsePlayer>().vanityEquipped = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LuminarRing>())
                .AddIngredient(ModContent.ItemType<LuminarDress>())
                .AddIngredient(ModContent.ItemType<LuminarTrousers>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }

    }

    public class LuminarMulsePlayer : ModPlayer
    {
        public bool vanityEquipped = false;

        public override void ResetEffects()
        {
            vanityEquipped = false;
        }

        public override void FrameEffects()
        {
            if (vanityEquipped)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "LuminarTrousers", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "LuminarDress", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "LuminarRing", EquipType.Head);
            }
        }
    }
}
