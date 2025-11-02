using CalamityEntropy.Content.Items.Donator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityEntropy.Common;

namespace CalamityEntropy.Content.Items.Vanity
{
    public class PineappleDog : ModItem, IDonatorItem, IVanitySkin
    {
        public string DonatorName => "Nienie";

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/pdhead", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/pdbody", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Vanity/pdlegs", EquipType.Legs, this);
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
            player.GetModPlayer<PineappleDogPlayer>().vanityEquipped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<PineappleDogPlayer>().vanityEquipped = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Pineapple, 1)
                .AddIngredient(ItemID.JungleRose, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class PineappleDogPlayer : ModPlayer
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
                Player.legs = EquipLoader.GetEquipSlot(Mod, "PineappleDog", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "PineappleDog", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "PineappleDog", EquipType.Head);

            }
        }
    }
}
