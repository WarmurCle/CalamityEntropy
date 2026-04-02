using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class OdinsRefuge : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 86;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.accessory = true;
            Item.defense = 24;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().holyMantle = true;
            ModContent.GetInstance<AsgardianAegis>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<RampartofDeities>().UpdateAccessory(player, hideVisual);

            //Panic Necklace effect if enabled
            player.panic = panicNecklaceEnabled;
        }
        #region Toggleable Panic Necklace

        bool panicNecklaceEnabled = true;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("[TOGGLE]", this.GetLocalizedValue("ToggleEffect" + (panicNecklaceEnabled ? "On" : "Off")));
        }
        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            panicNecklaceEnabled = !panicNecklaceEnabled;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        public override void SaveData(TagCompound tag)
        {
            tag.Add("panic", panicNecklaceEnabled);
        }

        public override void LoadData(TagCompound tag)
        {
            panicNecklaceEnabled = tag.GetBool("panic");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(panicNecklaceEnabled);
        }

        public override void NetReceive(BinaryReader reader)
        {
            panicNecklaceEnabled = reader.ReadBoolean();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryDot(spriteBatch, position, new Vector2(16, 16) * Main.inventoryScale, panicNecklaceEnabled);
        }
        #endregion
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<AsgardianAegis>(), 1).
                AddIngredient(ModContent.ItemType<RampartofDeities>(), 1).
                AddIngredient(ModContent.ItemType<HolyMantle>(), 1).
                AddIngredient(ModContent.ItemType<VoidBar>(), 10).
                AddTile(ModContent.TileType<VoidWellTile>()).
                Register();
        }
    }
}
