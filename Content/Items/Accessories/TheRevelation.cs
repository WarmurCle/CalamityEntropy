using CalamityEntropy.Utilities;
using CalamityMod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class TheRevelation : ModItem
    {
        public static int Damage = 50; public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(150, 7f, 1.9f);
        }
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().revelation = true; if (player.controlJump && player.wingTime > 0f && player.jump == 0)
            {
                bool hovering = player.TryingToHoverDown && !player.merman;
                if (hovering)
                {
                    player.velocity.Y *= 0.2f;
                    if (player.velocity.Y > -2f && player.velocity.Y < 2f)
                    {
                        player.velocity.Y = 0.105f * (1.9f / 1.75f);
                    }
                    player.wingTime += 0.6f;
                }
            }
            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
        }
    }
}
