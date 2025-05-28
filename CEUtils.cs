using CalamityEntropy.Content;
using CalamityEntropy.Content.Items.Books;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public static class CEUtils
    {
        public static DamageClass RogueDC => ModContent.GetInstance<RogueDamageClass>();

        public static bool HasEBookEffect<T>(this Projectile p) where T : EBookProjectileEffect {
            if (p.ModProjectile is EBookBaseProjectile ep) {
                foreach (var ef in ep.ProjectileEffects) {
                    if (ef.GetType() == typeof(T)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static NMSGItem TFAW(this Item item) => item.GetGlobalItem<NMSGItem>();
        public static NMSPLayer TFAW(this Player player) => player.GetModPlayer<NMSPLayer>();

        /// <summary>
        /// 用于将一个武器设置为手持刀剑类，这个函数若要正确设置物品的近战属性，需要让其在初始化函数中最后调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public static void SetKnifeHeld<T>(this Item item) where T : ModProjectile {
            item.noMelee = true;
            item.noUseGraphic = true;
            item.TFAW().IsShootCountCorlUse = true;
            item.shoot = ModContent.ProjectileType<T>();
        }
        public static Vector2 randVr(int min, int max) {
            return Main.rand.NextVector2Unit() * Main.rand.Next(min, max);
        }
        public static float GetCorrectRadian(float minusRadian) {
            return minusRadian < 0 ? (MathHelper.TwoPi + minusRadian) / MathHelper.TwoPi : minusRadian / MathHelper.TwoPi;
        }

        /// <summary>
        /// 获取纹理实例，类型为 Texture2D
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Texture2D GetT2DValue(string texture, bool immediateLoad = false) {
            return ModContent.Request<Texture2D>(texture, immediateLoad ? AssetRequestMode.AsyncLoad : AssetRequestMode.ImmediateLoad).Value;
        }
        /// <summary>
        /// 获取纹理实例，类型为 AssetTexture2D
        /// </summary>
        /// <param name="texture">纹理路径</param>
        /// <returns></returns>
        public static Asset<Texture2D> GetT2DAsset(string texture, bool immediateLoad = false) {
            return ModContent.Request<Texture2D>(texture, immediateLoad ? AssetRequestMode.AsyncLoad : AssetRequestMode.ImmediateLoad);
        }
    }
}
