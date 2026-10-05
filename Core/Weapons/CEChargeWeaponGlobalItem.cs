using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Weapons
{
    public class CEChargeWeaponGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
            => lateInstantiation && entity.ModItem is ICEChargeWeapon;

        //蓄势走 HoldItem,不走 UpdateInventory,同款武器按格子各涨一条
        public override void HoldItem(Item item, Player player) {
            var profile = ((ICEChargeWeapon)item.ModItem).ChargeProfile;
            if (profile.Trigger == CEChargeTrigger.ChargeBar || profile.Trigger == CEChargeTrigger.Periodic)
                CEChargeWeapon.Gain(player, item, profile, 1f);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            //ModifyShootStats 的时序对齐原 RogueWeapon.ModifyShootStats
            if (!CEChargeWeapon.IsReady(item))
                return;
            var profile = ((ICEChargeWeapon)item.ModItem).ChargeProfile;
            damage = (int)(damage * profile.DamageMult);
            velocity *= profile.VelocityMult;
            knockback *= profile.KnockbackMult;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            var meter = CEChargeWeapon.GetMeter(item);
            if (meter == null)
                return;

            var profile = ((ICEChargeWeapon)item.ModItem).ChargeProfile;
            Color barColor = CEChargeBarHUD.TriggerColor(profile.Trigger);
            if (meter.Ready)
                barColor = Color.Lerp(barColor, Color.White, 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 8f));

            int fullWidth = (int)(frame.Width * scale);
            int fillWidth = (int)(fullWidth * meter.Ratio);
            Vector2 barPos = position - origin * scale + new Vector2(0, frame.Height * scale - 2f);
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, barPos, new Rectangle(0, 0, 1, 1), Color.Black * 0.45f, 0f, Vector2.Zero, new Vector2(fullWidth, 2f), SpriteEffects.None, 0f);
            if (fillWidth > 0)
                spriteBatch.Draw(pixel, barPos, new Rectangle(0, 0, 1, 1), barColor * 0.9f, 0f, Vector2.Zero, new Vector2(fillWidth, 2f), SpriteEffects.None, 0f);
        }
    }
}
