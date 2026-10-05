using CalamityEntropy.Core.Cooldowns;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityEntropy.Core.Weapons
{
    public class CEChargeBarHUD : ModSystem
    {
        private const int BarWidth = 36; //GenericBar 宽

        //加载期就位,不每帧查 getExtraTex
        [VaultLoaden("CalamityEntropy/Assets/Extra/Ports/GenericBarBack")]
        private static Asset<Texture2D> BarBackTex;
        [VaultLoaden("CalamityEntropy/Assets/Extra/Ports/GenericBarFront")]
        private static Asset<Texture2D> BarFrontTex;

        public static Color TriggerColor(CEChargeTrigger trigger) => trigger switch {
            CEChargeTrigger.ChargeBar => new Color(255, 170, 60),
            CEChargeTrigger.HitCount => new Color(235, 90, 80),
            _ => new Color(90, 200, 235),
        };

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Entity Health Bars");
            if (index != -1) {
                layers.Insert(index, new LegacyGameInterfaceLayer("CalamityEntropy: Charge Bar", () => {
                    Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.Game));
            }
        }

        private static void Draw(SpriteBatch spriteBatch) {
            if (Main.gameMenu)
                return;

            Player player = Main.LocalPlayer;
            if (player == null || !player.active || player.dead || player.ghost)
                return;
            if (player.HeldItem?.ModItem is not ICEChargeWeapon chargeWeapon)
                return;

            CEChargeMeter meter = CEChargeWeapon.GetMeter(player.HeldItem);
            if (meter == null)
                return;

            Texture2D back = BarBackTex.Value;
            Texture2D front = BarFrontTex.Value;
            Vector2 drawPos = player.Top - Main.screenPosition + new Vector2(-back.Width / 2f, -22f + player.gfxOffY);
            drawPos = new Vector2((int)drawPos.X, (int)drawPos.Y);

            Color color = TriggerColor(chargeWeapon.ChargeProfile.Trigger);
            float opacity = 0.85f;
            if (meter.Ready) {
                float pulse = 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 8f);
                color = Color.Lerp(color, Color.White, 0.35f + 0.4f * pulse);
                opacity = 0.95f;
            }

            spriteBatch.Draw(back, drawPos, null, Color.White * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            int fill = (int)(BarWidth * meter.Ratio);
            if (fill > 0) {
                Rectangle fillRect = new Rectangle(0, 0, fill, front.Height);
                spriteBatch.Draw(front, drawPos, fillRect, color * opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
