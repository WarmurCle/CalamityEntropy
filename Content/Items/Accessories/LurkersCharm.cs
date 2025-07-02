using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class LurkersCharm : ModItem
    {
        public static float damage = 0.1f;
        public static float MoveSpeed = 0.05f;
        public static float endurance = 0.15f;
        public static float stealthGen = 0.12f;
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.defense = 4;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", damage.ToPercent());
            tooltips.Replace("[B]", damage.ToPercent());
            tooltips.Replace("[D]", endurance.ToPercent());
            tooltips.Replace("[C]", stealthGen.ToPercent());

        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RogueDamageClass>() += damage;
            player.Entropy().moveSpeed += MoveSpeed;
            player.Entropy().RogueStealthRegenMult += stealthGen;
            if(player.Calamity().rogueStealth > player.Calamity().rogueStealthMax * 0.9f)
            {
                player.endurance += endurance;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Magiluminescence)
                .AddIngredient(ItemID.GlowingMushroom, 8)
                .AddIngredient(ItemID.SoulofNight, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
