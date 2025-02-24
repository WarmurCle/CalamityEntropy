using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.VoidFaquir
{
    [AutoloadEquip(EquipType.Body)]
    public class VoidFaquirBodyArmor : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityEntropy/Content/Items/Armor/VoidFaquir/VoidFaquirBodyArmor_Back", EquipType.Back, this);
            }
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 34;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 44;
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Magic) += 5;
            player.GetArmorPenetration(DamageClass.Generic) += 5;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 18)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 8)
                .AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4)
                .AddIngredient(ModContent.ItemType<TwistingNether>(), 10).Register();
        }
    }
}
