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
        public static float damage = 0.12f;
        public static float MoveSpeed = 0.10f;
        public static float endurance = 0.10f;
        public static float stealthGen = 0.10f;
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
            tooltips.Replace("[B]", MoveSpeed.ToPercent());
            tooltips.Replace("[D]", endurance.ToPercent());
            tooltips.Replace("[C]", stealthGen.ToPercent());

        }
        public static string ID = "LurkersCharm";

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RogueDamageClass>() += damage;
            player.Entropy().moveSpeed += MoveSpeed;
            player.Entropy().RogueStealthRegenMult += stealthGen;
            player.Entropy().addEquip(ID, !hideVisual);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Magiluminescence)
                .AddIngredient<RogueEmblem>(1)
                .AddIngredient(ItemID.SoulofNight, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
