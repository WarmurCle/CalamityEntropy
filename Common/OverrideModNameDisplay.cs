using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityEntropy.Common
{
    public class OverrideModNameDisplay : ModSystem
    {
        public override void Load()
        {
            if (Main.dedServ)
            {
                return;
            }
            Main.QueueMainThreadAction(delegate ()
            {
                string text = base.Mod.DisplayName;
                Point size = Utils.ToPoint(FontAssets.MouseText.Value.MeasureString(text) + new Vector2(4, 4));
                _renderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, size.X, size.Y);
                Main.spriteBatch.Begin();
                Main.graphics.GraphicsDevice.SetRenderTarget(_renderTarget);
                Main.graphics.GraphicsDevice.Clear(Color.Transparent);
                for(float i = 0; i < 360; i += 60)
                {
                    Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(2, 2) + MathHelper.ToRadians(i).ToRotationVector2() * 1, new Color(100, 80, 200), 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                }
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(2, 2), new Color(20, 16, 50), 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
                Main.spriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            });
            _uiModItemType = Enumerable.First<Type>(typeof(Main).Assembly.GetTypes(), (Type t) => t.Name == "UIModItem");
            _drawMethod = _uiModItemType.GetMethod("Draw", (BindingFlags)20);
            if (_drawMethod != null)
            {
                MonoModHooks.Add(_drawMethod, new Action<DrawDelegate, object, SpriteBatch>(this.DrawHook));
            }
        }

        private void DrawHook(DrawDelegate orig, object uiModItem, SpriteBatch sb)
        {
            orig(uiModItem, sb);
            if (_renderTarget == null)
            {
                return;
            }
            FieldInfo field = _uiModItemType.GetField("_modName", (BindingFlags)36);
            UIText modName = ((field != null) ? field.GetValue(uiModItem) : null) as UIText;
            if (modName == null)
            {
                return;
            }
            if (!modName.Text.Contains(base.Mod.DisplayName))
            {
                return;
            }
            var texture = Util.getExtraTex("NameMask");
            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/NameEffect", AssetRequestMode.ImmediateLoad).Value;
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            Vector2 position = modName.GetDimensions().Position() - new Vector2(0f, 2f) - Vector2.One * 2;
            Main.instance.GraphicsDevice.Textures[1] = texture;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, shader, Main.UIScaleMatrix);
            shader.CurrentTechnique.Passes["EnchantedPass"].Apply();
            sb.Draw(_renderTarget, position, Color.White);
            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
        }
        private static Type _uiModItemType;

        private static MethodInfo _drawMethod;

        private static RenderTarget2D _renderTarget;

        public delegate void DrawDelegate(object uiModItem, SpriteBatch sb);
    }
}
