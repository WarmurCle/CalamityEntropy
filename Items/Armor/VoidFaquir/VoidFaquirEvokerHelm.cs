using CalamityEntropy.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Security.Policy;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Armor.VoidFaquir
{
    [AutoloadEquip(EquipType.Head)]
    public class VoidFaquirEvokerHelm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 10;
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VoidFaquirBodyArmor>() && legs.type == ModContent.ItemType<VoidFaquirCuises>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.GetArmorPenetration(DamageClass.Generic) += 20;
            player.Entropy().VFSet = true;

            
            
            player.Entropy().VFHelmSummoner = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidMonster>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<VoidMonster>(), 1800, 4, player.whoAmI);
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().summonerVF = true;
            player.GetDamage(DamageClass.Summon) += 0.9f;
            player.maxMinions += 8;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 14)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 6)
                .AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 6)
                .AddIngredient(ModContent.ItemType<TwistingNether>(), 8).Register();
        }
    }
}
