using CalamityMod;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class IceShardWing : BaseWings
    {
        public static float HorSpeed = 7.6f;
        public static float AccMul = 1;
        public static int wTime = 180;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(wTime, HorSpeed, AccMul, false, 20, 2.8f);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 12, 25);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.Calamity().donorItem = true;
        }
        public override float BonusAscentWhileFalling => 0.8f;
        public override float BonusAscentWhileRising => 0.1f;
        public override float RisingSpeedThreshold => 0.5f;
        public override float MaxAscentSpeed => 1.8f;
        public override float BaseAscent => 0.1f;
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SoulofFlight, 20).
                AddIngredient<CryonicBar>(5).
                AddIngredient(ItemID.PurificationPowder, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}