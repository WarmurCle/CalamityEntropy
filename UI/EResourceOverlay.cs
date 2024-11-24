using System.Collections.Generic;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.UI
{
    public class EResourceOverlay : ModResourceOverlay
    {
        // Most of this is taken from ExampleMod. See that for additional explanations.
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();
        public string baseFolder = "CalamityEntropy/UI/";

        // Determines which health UI to draw based on player upgrades.
        public string LifeTexturePath()
        {
            string folder = $"{baseFolder}MoonShield";
            return folder;
        }
        public string ManaTexturePath()
        {
            if (Main.LocalPlayer.Entropy().ArchmagesMirror)
            {
                string folder = $"{baseFolder}AH";

                return folder;
            }
            else { return string.Empty; }
        }

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;
            // Vanilla texture paths
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            
            if (LifeTexturePath() != string.Empty)
            {


                // Draw hearts for Classic and Fancy
                if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2 || CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
                {
                    if ((context.resourceNumber + 1) * 30 <= Main.LocalPlayer.Entropy().MagiShield)
                    {
                        context.texture = ModContent.Request<Texture2D>(LifeTexturePath() + "Heart");
                        if (Main.LocalPlayer.Entropy().MagiShield - (context.resourceNumber + 1) * 30 < 30){
                            float s = ((float)(Main.LocalPlayer.Entropy().MagiShield - (context.resourceNumber + 1) * 30)) / 30f;
                            context.scale = new Vector2(s, s);
                        }
                        context.Draw();
                    }
                }
                // Draw health bars
                else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
                {
                    if ((context.resourceNumber + 1) * 30 <= Main.LocalPlayer.Entropy().MagiShield)
                    {
                        context.texture = ModContent.Request<Texture2D>(LifeTexturePath() + "Bar");
                        context.Draw();
                    }
                }
            }
            if (ManaTexturePath() != string.Empty && Main.LocalPlayer.Entropy().ArchmagesMirror)
            {
                if (asset == TextureAssets.Mana || CompareAssets(asset, fancyFolder + "Star_Fill"))
                {
                    if ((context.resourceNumber + 1) * 20 > Main.LocalPlayer.Entropy().manaNorm)
                    {
                        context.texture = ModContent.Request<Texture2D>(ManaTexturePath() + "Star");
                        context.Draw();
                    }
                }
                // Draw mana bars
                else if (CompareAssets(asset, barsFolder + "MP_Fill"))
                {
                    if ((context.resourceNumber + 1) * 20 > Main.LocalPlayer.Entropy().manaNorm)
                    {
                        context.texture = ModContent.Request<Texture2D>(ManaTexturePath() + "Bar");
                        context.Draw();
                    }
                }
            }
        }

        private bool CompareAssets(Asset<Texture2D> currentAsset, string compareAssetPath)
        {
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return currentAsset == asset;
        }
    }
}
