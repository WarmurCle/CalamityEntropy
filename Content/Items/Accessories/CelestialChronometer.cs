using CalamityEntropy.Content.Items.Donator;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class CelestialChronometer : ModItem, IDonatorItem
    {
        public string DonatorName => "丰川祥子";
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
            Item.defense = 40;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Vector2 c = (player.Center + new Vector2(0, player.height / 2 - 2)) / 16;
            if (Main.rand.NextBool(10))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && TileLoader.CanPlace((int)c.X, (int)c.Y, 84) && Main.tile[(int)c.X, (int)c.Y + 1].HasTile)
                {
                    List<int> CanPlace = new();
                    int t = Main.tile[(int)c.X, (int)c.Y + 1].TileType;
                    bool cpls = (!((Main.tile[(int)c.X, (int)c.Y + 1]).Get<TileWallWireStateData>().Slope != SlopeType.Solid || (Main.tile[(int)c.X, (int)c.Y + 1]).Get<TileWallWireStateData>().IsHalfBlock));
                    if (cpls)
                    {
                        if (t == 0 || t == 59)
                        {
                            CanPlace.Add(2);
                        }
                        if (t == 2 || t == 109 || t == 477 || t == 492)
                        {
                            CanPlace.Add(0);
                        }
                        if (t == 23 || t == 661 || t == 199 || t == 662 || t == 15 || t == 203)
                        {
                            CanPlace.Add(3);
                        }
                        if (t == 57 || t == 633)
                        {
                            CanPlace.Add(5);
                        }
                        if (t == 53 || t == 234)
                        {
                            CanPlace.Add(4);
                        }
                        if (t == 60)
                        {
                            CanPlace.Add(1);
                        }
                        if (t == 147 || t == 161 || t == 163 || t == 164 || t == 200)
                        {
                            CanPlace.Add(7);
                        }
                        if (CanPlace.Count > 0)
                        {
                            short fx = (short)(18 * CanPlace[Main.rand.Next(CanPlace.Count)]);
                            var tl = CEUtils.PlaceTile((int)c.X, (int)c.Y, 83);
                            tl.Get<TileWallWireStateData>().TileFrameX = fx;
                            tl.Get<TileWallWireStateData>().TileFrameY = 0;
                        }
                    }
                }
            }
            player.Entropy().lifeRegenPerSec += 5;
            if(CEUtils.inWorld((int)c.X, (int)c.Y) && Main.tile[(int)c.X, (int)c.Y].HasTile)
            {
                int type = Main.tile[(int)c.X, (int)c.Y].TileType;
                if (type >= 82 && type <= 84)
                {
                    player.endurance += 0.2f;
                }
            }
            ContentSamples.ItemsByType[ModContent.ItemType<ChaliceOfTheBloodGod>()].ModItem.UpdateAccessory(player, hideVisual);
            ContentSamples.ItemsByType[ModContent.ItemType<TheAbsorber>()].ModItem.UpdateAccessory(player, hideVisual);
            ContentSamples.ItemsByType[ModContent.ItemType<Radiance>()].ModItem.UpdateAccessory(player, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<ChaliceOfTheBloodGod>().AddIngredient<TheAbsorber>().AddIngredient<Radiance>().AddIngredient(5295).AddIngredient<ShadowspecBar>(4).AddIngredient<AscendantSpiritEssence>(2).AddTile(TileID.WorkBenches).Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach(var t in tooltips)
            {
                if(t.Mod == "Terraria")
                {
                    if (t.Text.Contains("$"))
                    {
                        t.OverrideColor = Color.Lerp(Color.White, Main.DiscoColor, (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 10) * 0.5f + 0.5f));
                    }
                }
            }
        }
    }
}
