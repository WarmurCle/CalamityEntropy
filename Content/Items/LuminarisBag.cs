﻿using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Vanity.Luminar;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.NPCs.LuminarisMoth;
using CalamityMod;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class LuminarisBag : ModItem
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
            Item.rare = ItemRarityID.Pink;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
        }

        public override bool CanRightClick() => true;

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.6f);

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
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Luminaris>()));

            itemLoot.Add(ModContent.ItemType<StarlitPiercer>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<Luminar>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<StarSootInjector>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<PhantomLightWing>(), new Fraction(5, 5));
            itemLoot.Add(ModContent.ItemType<LunarPlank>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<StarblightSoot>(), 1, 52, 74);
            var normalOnly = itemLoot.DefineConditionalDropSet(() => Main.rand.NextBool(4));
            {
                normalOnly.Add(ModContent.ItemType<LuminarRing>());
                normalOnly.Add(ModContent.ItemType<LuminarDress>());
                normalOnly.Add(ModContent.ItemType<LuminarTrousers>());
            }

        }
    }
}
