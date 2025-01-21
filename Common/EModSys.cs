using System;
using System.Collections.Generic;
using CalamityEntropy.Content.DimDungeon;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Skies;
using CalamityEntropy.Content.UI;
using CalamityEntropy.Util;
using CalamityMod.Items.Placeables.FurnitureAuric;
using CalamityMod.NPCs.SlimeGod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityEntropy.Common
{
    public class EModSys : ModSystem
    {
        public float counter = 0;
        public override void Load()
        {
        }
        Texture2D markTex { get { return Util.Util.getExtraTex("EvMark"); } }
        public override void PostDrawTiles()
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<EventideSniper>())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if(!npc.dontTakeDamage && !npc.friendly)
                    {
                        Main.spriteBatch.Draw(markTex, npc.Center - Main.screenPosition, null, Color.White, 0, markTex.Size() / 2, 1, SpriteEffects.None, 0f);
                    }
                }
                Main.spriteBatch.End();
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            noItemUse = false;
            counter += 1f;
            if (ArmorForgingStationUI.Visible)
            {
                CalamityEntropy.Instance.userInterface?.Update(gameTime);
            }
            
        }
        public static bool noItemUse = false;
        public override void PostUpdateDusts()
        {
            CalamityEntropy.blackMaskTime--;
            PixelParticle.Update();
            VoidParticles.Update();
            FloatParticles.Update();
            CalamityEntropy.cutScreen += CalamityEntropy.cutScreenVel;
            if(CalamityEntropy.cutScreen > 0)
            {
                CalamityEntropy.cutScreenVel -= 1.16f;
            }
            if(CalamityEntropy.cutScreen < 0)
            {
                CalamityEntropy.cutScreen = 0;
                CalamityEntropy.cutScreenVel = 0;
            }
        }
        public bool prd = true;
        public bool mi = false;
        public bool escLast = true;
        public bool rCtrlLast = false;
        public bool eowLast = false;
        public int eowMaxLife = 0;
        public bool slimeGodLast = false;
        public int slimeGodMaxLife = 0;
        public override void PostUpdatePlayers()
        {
            if (ModContent.GetInstance<RepMusicScene>().IsSceneEffectActive(Main.LocalPlayer))
            {
                Main.musicFade[MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/RepBossTrack")] = 1;
            }
            if(CalamityEntropy.noMusTime > 0)
            {
                CalamityEntropy.noMusTime--;
                Main.curMusic = 0;
                Main.newMusic = 0;
                for(int i = 0; i < Main.musicFade.Length; i++)
                {
                    Main.musicFade[i] = 0;
                }
            }

            bool rCtrl = Keyboard.GetState().IsKeyDown(Keys.RightControl);
            if(!rCtrlLast && rCtrl)
            {
                //BeeGame.Active = !BeeGame.Active;

            }
            rCtrlLast = rCtrl;
            
            
            if (!Main.playerInventory)
            {
                if (ArmorForgingStationUI.Visible)
                {
                    CalamityEntropy.Instance.armorForgingStationUI.close();
                }
                ArmorForgingStationUI.Visible = false;

            }
            escLast = Keyboard.GetState().IsKeyDown(Keys.Escape);
            if (!Main.dedServ)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.N))
                {
                    if (!prd)
                    {
                        prd = true;
                        mi = !mi;
                    }
                }
                else
                {
                    prd = false;
                }
            }
            if (mi)
            {
                Main.instance.IsMouseVisible = true;
            }

            LoopSoundManager.update();
        }
        public void drawChargeBar(Vector2 center, float prog, Color color)
        {
            Texture2D bar = ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/ui_chargebar").Value;
            Main.spriteBatch.Draw(bar, center, new Rectangle(0, 0, 54, 12), Color.White, 0, new Vector2(27, 6), 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bar, center, new Rectangle(0, 14, 4 + (int)Math.Round(46 * prog), 12), color, 0, new Vector2(27, 6), 1, SpriteEffects.None, 0);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            foreach (GameInterfaceLayer layer in layers)
            {
                if (layer.Name == "Vanilla: Cursor" && mi)
                {
                    layer.Active = false;
                }
            }
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CalamityEntropy: Void Charge Bar", () =>
                {
                    DrawVoidChargeBar(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CalamityEntropy: Other Charge Bars", () =>
                {
                    int baroffsety = 84;
                    if (Main.LocalPlayer.Entropy().mawOfVoidCharge > 0)
                    {
                        drawChargeBar(Main.ScreenSize.ToVector2() / 2 + new Vector2(0, baroffsety), Main.LocalPlayer.Entropy().mawOfVoidCharge, Color.Red);
                        baroffsety += 20;
                    }
                    if (Main.LocalPlayer.Entropy().revelationCharge > 0)
                    {
                        drawChargeBar(Main.ScreenSize.ToVector2() / 2 + new Vector2(0, baroffsety), Main.LocalPlayer.Entropy().revelationCharge, new Color(255, 255, 190));
                        baroffsety += 20;
                    }
                    return true;
                }, InterfaceScaleType.None));
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("CalamityEntropy: Poop UI", () =>
                {
                    if (Main.LocalPlayer.Entropy().brokenAnkh)
                    {
                        Main.spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
                        PoopsUI.Draw();
                        Main.spriteBatch.UseSampleState_UI(SamplerState.AnisotropicClamp);
                    }
                    return true;
                }, InterfaceScaleType.None));
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
           //这里是绘制层的名字
           "CalamityEntropy: Armor Reforging Station",
           //这里是匿名方法
           delegate
           {
               //当Visible开启时（当UI开启时）
               if (ArmorForgingStationUI.Visible)
                   //绘制UI（运行exampleUI的Draw方法）
                    CalamityEntropy.Instance.armorForgingStationUI.Draw(Main.spriteBatch);
               return true;
           },
           //这里是绘制层的类型
           InterfaceScaleType.UI)
       );
            }
            }

        public void DrawVoidChargeBar(SpriteBatch spriteBatch)
        {
            if (!Main.LocalPlayer.Entropy().VFSet)
            {
                return;
            }
            Texture2D bar = ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/VoidChargeBar").Value;
            Texture2D prog = ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/VoidChargeProgress").Value;
            Config config = ModContent.GetInstance<Config>();
            Vector2 offset = new Vector2(config.VoidChargeBarX, config.VoidChargeBarY);
            float p = Main.LocalPlayer.Entropy().VoidCharge;
            if (Main.LocalPlayer.Entropy().VoidInspire > 0)
            {
                p = (float)Main.LocalPlayer.Entropy().VoidInspire / 600f;
                offset += new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));
            }
            spriteBatch.Draw(bar, offset, null, Color.White, 0, bar.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(prog, offset, new Rectangle(0, 0, (int)(prog.Width * p), prog.Height), Color.White, 0, prog.Size() / 2, 1, SpriteEffects.None, 0);
            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 1, 1);
            if (new Rectangle((int)(offset - bar.Size() / 2).X, (int)((offset - bar.Size() / 2).Y), (int)bar.Size().X, (int)bar.Size().Y).Intersects(mouse))
            {
                string textToDisplay = Language.GetOrRegister("Mods.CalamityEntropy.VoidChargeBar").Value + " : " + ((int)(p * 100)).ToString() + "%";
                Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
            }
        }

        public override void PostUpdateNPCs()
        {
            bool eow = false;
            int maxlifeEows = 0;
            bool sg = false;
            int maxlifeSg = 0;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail)
                {
                    eow = true;
                    maxlifeEows += n.lifeMax;
                }
                if(ModContent.NPCType<CrimulanPaladin>() == n.type || ModContent.NPCType<SplitCrimulanPaladin>() == n.type || ModContent.NPCType<EbonianPaladin>() == n.type || ModContent.NPCType<SplitEbonianPaladin>() == n.type)
                {
                    sg = true;
                    maxlifeSg += n.lifeMax;
                }
            }
            if (eow && !eowLast)
            {
                eowMaxLife = maxlifeEows;
            }
            eowLast = eow;

            if(sg && !slimeGodLast)
            {
                slimeGodMaxLife = maxlifeSg;
            }

            slimeGodLast = sg;
            
        }
        public override void PostAddRecipes()
        {
            foreach(var recipe in Main.recipe)
            {
                if (recipe.createItem.type == ModContent.ItemType<AuricToilet>())
                {
                    recipe.DisableRecipe();
                }
            }

        }
        public Vector2 LastPlayerPos;
        public Vector2 LastPlayerVel;
        internal static int timer;

        public override void PreUpdateProjectiles()
        {
            timer++;
            if (!Main.dedServ)
            {
                LastPlayerPos = Main.LocalPlayer.Center;
            }
        }

        public override void PostUpdateProjectiles()
        {
            if (!Main.dedServ)
            {
                //Main.LocalPlayer.Center = LastPlayerPos;
            }
        }
    }
}
