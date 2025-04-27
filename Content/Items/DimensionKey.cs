using CalamityEntropy.Utilities;
using CalamityMod.Items;
using CalamityMod.Rarities;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class DimensionKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {

            Item.width = 64;
            Item.height = 64;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            SoundStyle s = new("CalamityEntropy/Assets/Sounds/vmspawn");
            s.Volume = 0.6f;
            s.Pitch = 1f;
            Item.UseSound = s;
            Item.noMelee = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(20, 26, 92);
            Item.Entropy().tooltipStyle = 4;
            Item.Entropy().NameColor = new Color(60, 80, 140);
            Item.Entropy().Legend = true;
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
        }
        public override bool? UseItem(Player player)
        {
            if (!SubworldSystem.IsActive<DimDungeon.DimDungeonSubworld>())
            {
                SubworldSystem.Enter<DimDungeon.DimDungeonSubworld>();
                return true;
            }
            return false;
        }
    }
}