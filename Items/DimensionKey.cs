using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityEntropy.Buffs;
using CalamityEntropy.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.Audio;
using CalamityMod.Rarities;
using CalamityMod.Items;
using SubworldLibrary;
namespace CalamityEntropy.Items
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
            SoundStyle s = new("CalamityEntropy/Sounds/vmspawn");
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
            if (!SubworldSystem.IsActive<DimDungeon.DimDungeon>())
            {
                SubworldSystem.Enter<DimDungeon.DimDungeon>();
                return true;
            }
            return false;
        }
    }
}