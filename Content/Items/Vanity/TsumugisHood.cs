using CalamityEntropy.Common;
using CalamityMod.Items.Accessories.Vanity;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Content.Items.Vanity
{
    public class TsumugisHood : ModItem, IVanitySkin
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"CalamityEntropy/Content/Items/Vanity/{Name}_Head", EquipType.Head, this);
                EquipLoader.AddEquipTexture(Mod, $"CalamityEntropy/Content/Items/Vanity/{Name}_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, $"CalamityEntropy/Content/Items/Vanity/{Name}_Legs", EquipType.Legs, this);
            }
        }

        bool NoHoodEnabled = false;
        public override void SaveData(TagCompound tag)
        {
            if(NoHoodEnabled)
                tag.Add("NoHoodEnabled", NoHoodEnabled);
        }
        public override void LoadData(TagCompound tag)
        {
            if(tag.ContainsKey("NoHoodEnabled"))
                NoHoodEnabled = tag.GetBool("NoHoodEnabled");
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(NoHoodEnabled);
        }
        public override void NetReceive(BinaryReader reader)
        {
            NoHoodEnabled = reader.ReadBoolean();
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            if (Main.keyState.PressingShift())
            {
                NoHoodEnabled = !NoHoodEnabled;
                Item.NetStateChanged();
            }
        }
        public override bool ConsumeItem(Player player)
        {
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[D]", Mod.GetLocalization($"Items.{Name}." + (NoHoodEnabled ? "Hide" : "Show")).Value);
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
            Item.width = 32;
            Item.height = 36;
            Item.accessory = true;
            Item.value = CalamityMod.Items.CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<VanityModPlayer>().vanityEquipped = Name;
            player.GetModPlayer<VanityModPlayer>().SpecialFlag = NoHoodEnabled ? 1 : 0;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<VanityModPlayer>().vanityEquipped = Name;
                player.GetModPlayer<VanityModPlayer>().SpecialFlag = NoHoodEnabled ? 1 : 0;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ArcaneCrystal)
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
