using CalamityEntropy.Content.NPCs.NihilityTwin;
using CalamityMod.Events;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class NihilityHorn : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 15;
        }
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noUseGraphic = true;
            Item.UseSound = CEUtils.GetSound("horn");
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();

        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<NihilityActeriophage>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<NihilityActeriophage>();
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, type);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<ExodiumCluster>(), 6).
                AddIngredient(ModContent.ItemType<Voidstone>(), 6).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}