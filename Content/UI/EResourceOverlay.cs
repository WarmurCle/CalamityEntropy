using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.UI
{
    public class EResourceOverlay : ModResourceOverlay
    {
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();
        public string baseFolder = "CalamityEntropy/Content/UI/";

        public string LifeTexturePath()
        {
            string folder = $"{baseFolder}MoonShield";
            return folder;
        }
        public string ManaTexturePath()
        {
            if (Main.LocalPlayer.Entropy().enhancedMana > 0)
            {
                string folder = $"{baseFolder}AH";

                return folder;
            }
            else { return string.Empty; }
        }

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";


            if (LifeTexturePath() != string.Empty)
            {


                if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2 || CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
                {
                    if ((context.resourceNumber + 1) * 30 <= Main.LocalPlayer.Entropy().MagiShield)
                    {
                        context.texture = ModContent.Request<Texture2D>(LifeTexturePath() + "Heart");
                        if (Main.LocalPlayer.Entropy().MagiShield - (context.resourceNumber + 1) * 30 < 30)
                        {
                            float s = ((float)(Main.LocalPlayer.Entropy().MagiShield - (context.resourceNumber + 1) * 30)) / 30f;
                            context.scale = new Vector2(s, s);
                        }

                        context.Draw();
                    }
                    if (Main.LocalPlayer.Entropy().deusCoreBloodOut > 0)
                    {
                        if (context.resourceNumber > (Main.LocalPlayer.ConsumedLifeCrystals + 5f) * ((((float)Main.LocalPlayer.statLife) / ((float)Main.LocalPlayer.statLifeMax2)) - ((float)Main.LocalPlayer.Entropy().deusCoreBloodOut / (float)Main.LocalPlayer.statLifeMax2)))
                        {
                            context.texture = ModContent.Request<Texture2D>($"{baseFolder}Astr" + "Heart");
                            context.Draw();
                        }
                    }
                    if (context.resourceNumber == 0 && Main.LocalPlayer.Entropy().HolyShield)
                    {
                        context.texture = ModContent.Request<Texture2D>($"{baseFolder}mantle");
                        context.scale = new Vector2(2, 2);
                        context.origin += new Vector2(0.5f, 0.5f);
                        Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
                        context.Draw();
                    }

                }
                else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
                {
                    if ((context.resourceNumber + 1) * 30 <= Main.LocalPlayer.Entropy().MagiShield)
                    {
                        context.texture = ModContent.Request<Texture2D>(LifeTexturePath() + "Bar");
                        context.Draw();
                    }
                    if (Main.LocalPlayer.Entropy().deusCoreBloodOut > 0)
                    {
                        if (context.resourceNumber > (Main.LocalPlayer.ConsumedLifeCrystals + 5f) * ((((float)Main.LocalPlayer.statLife) / ((float)Main.LocalPlayer.statLifeMax2)) - ((float)Main.LocalPlayer.Entropy().deusCoreBloodOut / (float)Main.LocalPlayer.statLifeMax2)))
                        {
                            context.texture = ModContent.Request<Texture2D>($"{baseFolder}Astr" + "Bar");
                            context.Draw();
                        }
                    }
                    if (context.resourceNumber == 2 && Main.LocalPlayer.Entropy().HolyShield)
                    {
                        context.texture = ModContent.Request<Texture2D>($"{baseFolder}mantle");
                        context.scale = new Vector2(2, 2);
                        context.origin += new Vector2(0.5f, 0.5f);
                        Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
                        context.Draw();
                    }

                }
            }
            if (ManaTexturePath() != string.Empty && Main.LocalPlayer.Entropy().enhancedMana > 0)
            {
                if (asset == TextureAssets.Mana || CompareAssets(asset, fancyFolder + "Star_Fill"))
                {
                    if ((context.resourceNumber + 1) * 20 > Main.LocalPlayer.Entropy().manaNorm)
                    {
                        context.texture = ModContent.Request<Texture2D>(ManaTexturePath() + "Star");
                        context.Draw();
                    }
                }
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
