using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Vanity.Luminar;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.NPCs.Apsychos;
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
    public class ApsychosBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3; 
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 32;
            Item.height = 32;
            Item.expert = true;
            Item.rare = ItemRarityID.LightRed;
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
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Apsychos>()));

            itemLoot.Add(ModContent.ItemType<TectonicShard>(), 1, 36, 42);
            itemLoot.Add(ModContent.ItemType<GreatSwordofEmbers>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<AshesCore>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<ScorchingChakram>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<AshesBow>(), new Fraction(3, 5));
            itemLoot.Add(ModContent.ItemType<EmberBolt>(), new Fraction(3, 5));
        }
    }
}
