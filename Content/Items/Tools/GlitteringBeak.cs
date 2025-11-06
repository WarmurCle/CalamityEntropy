using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Tools
{
    public class GlitteringBeak : ModItem
    {
        private static int PickPower = 1000;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 70;
            Item.damage = 1000;
            Item.knockBack = 9f;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.pick = PickPower;
            Item.axe = PickPower / 5;
            Item.tileBoost = 120;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<AbyssalBlue>();
            Item.crit = 18;
            Item.useTurn = true;
        }

        public override bool AltFunctionUse(Player player) => true;
        public Vector2 mouseLast = Vector2.Zero;
        public Vector2 mousePos = Vector2.Zero;
        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 2)
                {
                    void KillCircleTile(Vector2 pos)
                    {
                        for (float i = 0; i <= 4; i += 0.8f)
                        {
                            for (float r = 0; r < 360; r += 15)
                            {
                                Point point = ((pos + (r + 5).ToRadians().ToRotationVector2() * i * 16) / 16f).ToPoint();
                                CEUtils.TryKillTileAndChest(point.X, point.Y, player);
                            }
                        }
                    }
                    for (float i = 0; i <= 1; i += 0.1f)
                    {
                        KillCircleTile(Vector2.Lerp(Main.MouseWorld, mouseLast, i));
                        EParticle.NewParticle(new HadCircle2() { CScale = 0.46f }, Vector2.Lerp(Main.MouseWorld, mouseLast, i), Vector2.Zero, Color.SkyBlue, 0.4f, 1, true, BlendState.Additive, 0);
                    }
                }
                else
                {
                    if (!Main.SmartCursorIsUsed)
                    {
                        void KillCircleTile(Vector2 pos)
                        {
                            Point point = (pos / 16f).ToPoint();
                            CEUtils.TryKillTileAndChest(point.X, point.Y, player);
                        }
                        int c = 0;
                        for (float i = 0; i <= 1; i += 0.01f)
                        {
                            KillCircleTile(Vector2.Lerp(Main.MouseWorld, mouseLast, i));
                            if (c++ % 10 == 0) EParticle.NewParticle(new HadCircle2() { CScale = 0.14f }, Vector2.Lerp(Main.MouseWorld, mouseLast, i), Vector2.Zero, Color.SkyBlue, 0.4f, 1, true, BlendState.Additive, 0);
                        }
                    }
                    else
                    {
                        if (Main.SmartCursorShowing)
                            EParticle.NewParticle(new HadCircle2() { CScale = 0.14f }, new Vector2(Main.SmartCursorX * 16 + 8, Main.SmartCursorY * 16 + 8), Vector2.Zero, Color.SkyBlue, 0.4f, 1, true, BlendState.Additive, 0);
                    }
                }
            }
            return base.UseItem(player);
        }
        public override void HoldItem(Player player)
        {
            mouseLast = mousePos;
            mousePos = Main.MouseWorld;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (Item.useStyle == ItemUseStyleID.Shoot)
            {
                TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "TileBoost");

                if (line != null)
                    line.Text = string.Empty;
            }
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("LunarPickaxe").
                AddIngredient<WyrmTooth>(5).
                AddTile<AbyssalAltarTile>().
                Register();
        }
    }
}
