using CalamityEntropy.Content.NPCs.Apsychos;
using CalamityEntropy.Content.NPCs.Prophet;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class CursedRunestone : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 15;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Roar;
        }
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Apsychos>()) && !BossRushEvent.BossRushActive && player.ZoneUnderworldHeight;
        }

        public override bool? UseItem(Player player)
        {
            int type = ModContent.NPCType<Apsychos>();
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, type);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.HellstoneBar, 6)
                .AddIngredient(ItemID.CrimtaneBar, 4)
                .AddIngredient(ItemID.FallenStar)
                .AddIngredient<AncientBoneDust>()
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe().AddIngredient(ItemID.HellstoneBar, 6)
                .AddIngredient(ItemID.DemoniteBar, 4)
                .AddIngredient(ItemID.FallenStar)
                .AddIngredient<AncientBoneDust>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
