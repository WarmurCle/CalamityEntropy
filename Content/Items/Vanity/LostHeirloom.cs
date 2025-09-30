using CalamityEntropy.Content.Items.Donator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity
{
    public class LostHeirloom : ModItem, IDonatorItem
    {
        public string DonatorName => "a3a4";

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/llHead", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/llBody", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/llLegs", EquipType.Legs, this);
            }
        }

        public override void SetStaticDefaults()
        {

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.accessory = true;
            Item.value = CalamityMod.Items.CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<LostHeirloomPlayer>().vanityEquipped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<LostHeirloomPlayer>().vanityEquipped = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(315).AddIngredient(313).AddIngredient(316).AddIngredient(318).AddIngredient(314).AddIngredient(2358).AddIngredient(317)
                .AddIngredient(ItemID.ManaCrystal)
                .AddIngredient(ItemID.FallenStar)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LostHeirloomPlayer : ModPlayer
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
                Player.legs = EquipLoader.GetEquipSlot(Mod, "LostHeirloom", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "LostHeirloom", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "LostHeirloom", EquipType.Head);

            }
        }
    }
}
