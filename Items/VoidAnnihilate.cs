using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityEntropy.Projectiles.VoidBlade;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Placeables;
using CalamityEntropy.Projectiles;
using CalamityEntropy.Projectiles.TwistedTwin;
using CalamityMod;
using System.Collections.Generic;
namespace CalamityEntropy.Items
{
    public class VoidAnnihilate : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CEKeybinds.RetrieveVoidAnnihilateHotKey);

        public override void SetDefaults()
        {
            Item.damage = 3250;
            Item.crit = 8;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.noUseGraphic = true;
            Item.height = 64;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 12000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<VoidAnnihilateProj>();
            Item.shootSpeed = 8f;
            Item.ArmorPenetration = 50;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 5 * (1 + player.ownedProjectileCounts[ModContent.ProjectileType<TwistedTwinMinion>()]);
        }
        public override void AddRecipes()
        {
        }
    }
}