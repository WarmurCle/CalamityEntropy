using CalamityEntropy.Items;
using CalamityEntropy.Items.Accessories;
using CalamityEntropy.Items.Pets;
using CalamityEntropy.NPCs.Cruiser;
using CalamityMod;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items
{
    public class CruiserBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
			ItemID.Sets.BossBag[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.expert = true;
            Item.rare = ItemRarityID.Cyan;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
		}

        public override bool CanRightClick() => true;

		public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void PostUpdate()
		{
			CalamityMod.CalamityUtils.ForceItemIntoWorld(Item);
			Item.TreasureBagLightAndDust();
		}

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
			// Money
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<CruiserHead>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<VoidRelics>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<VoidElytra>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<VoidEcho>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<Silence>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<RuneSong>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<WingsOfHush>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<VoidAnnihilate>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<PhantomPlanetKillerEngine>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<VoidToy>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<VoidScales>(), new Fraction(1, 1), 58, 68);
            if (Main.masterMode && CalamityWorld.death)
            {
                itemLoot.Add(ModContent.ItemType<TheocracyPearlToy>(), new Fraction(1, 6));
            }
            itemLoot.Add(ModContent.ItemType<VoidMonolith>(), new Fraction(2, 5));
        }
    }
}
