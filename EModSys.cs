using CalamityMod.Items.Placeables.FurnitureAuric;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;
using CalamityEntropy;
using CalamityEntropy.Buffs;
using Microsoft.Xna.Framework.Input;
using CalamityEntropy.Util;
using CalamityMod.UI.DraedonsArsenal;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Terraria.Localization;
using CalamityEntropy.UI;
using CalamityEntropy.BeesGame;

namespace CalamityEntropy
{
    public class EModSys : ModSystem
    {
        public float counter = 0;
        public override void Load()
        {
        }
        public override void UpdateUI(GameTime gameTime)
        {
            counter += 1f;
            if (ArmorForgingStationUI.Visible)
            {
                CalamityEntropy.Instance.userInterface?.Update(gameTime);

            }
            base.UpdateUI(gameTime);
        }
        
        public override void PostUpdateDusts()
        {
            VoidParticles.Update();
            FloatParticles.Update();
        }
        public bool prd = true;
        public bool mi = false;
        public bool escLast = true;
        public bool rCtrlLast = false;
        
        public override void PostUpdatePlayers()
        {
            
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
            Texture2D bar = ModContent.Request<Texture2D>("CalamityEntropy/UI/VoidChargeBar").Value;
            Texture2D prog = ModContent.Request<Texture2D>("CalamityEntropy/UI/VoidChargeProgress").Value;
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
                Main.LocalPlayer.Center = LastPlayerPos;
            }
        }
    }
}
