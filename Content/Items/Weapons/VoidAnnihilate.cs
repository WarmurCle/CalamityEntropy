using System.Collections.Generic;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class VoidAnnihilate : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CEKeybinds.RetrieveVoidAnnihilateHotKey);

        public override void SetDefaults()
        {
            Item.damage = 1500;
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
            return player.ownedProjectileCounts[Item.shoot] < (4 + player.Entropy().WeaponBoost) * (1 + player.ownedProjectileCounts[ModContent.ProjectileType<TwistedTwinMinion>()]);
        }
        public override void AddRecipes()
        {
        }
    }
}