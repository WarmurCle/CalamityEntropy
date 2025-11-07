using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles
{
    public partial class BaseParticleManager : ModSystem
    {
        public static readonly List<BaseParticle> ActiveParticlesAlpha = [];
        public static readonly List<BaseParticle> ActiveParticlesNonPremultiplied = [];
        public static readonly List<BaseParticle> ActiveParticlesAdditive = [];
        public static readonly List<BaseParticle> PriorityActiveParticlesAlpha = [];
        public static readonly List<BaseParticle> PriorityActiveParticlesNonPremultiplied = [];
        public static readonly List<BaseParticle> PriorityActiveParticlesAdditive = [];
        #region 加载卸载
        public override void Load()
        {
            On_Main.DrawDust += DrawParticles;
        }
        public override void Unload()
        {
            On_Main.DrawDust -= DrawParticles;
        }
        #endregion

        /// <summary>
        /// 清除世界状态时调用（例如退出世界时）。
        /// </summary>
        public override void ClearWorld()
        {
            ActiveParticlesAlpha.Clear();
            ActiveParticlesNonPremultiplied.Clear();
            ActiveParticlesAdditive.Clear();
            PriorityActiveParticlesAlpha.Clear();
            PriorityActiveParticlesNonPremultiplied.Clear();
            PriorityActiveParticlesAdditive.Clear();
        }

        // 粒子更新
        public override void PostUpdateDusts()
        {
            UpdatePriorityParticles();
            UpdateParticles();

        }
        //绘制粒子
        public static void DrawParticles(On_Main.orig_DrawDust orig, Main self)
        {
            //调用
            orig(self);
            #region 渲染粒子
            #region 渲染优先粒子
            if (PriorityActiveParticlesAlpha.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < PriorityActiveParticlesAlpha.Count; i++)
                {
                    PriorityActiveParticlesAlpha[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (PriorityActiveParticlesAdditive.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < PriorityActiveParticlesAdditive.Count; i++)
                {
                    PriorityActiveParticlesAdditive[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (PriorityActiveParticlesNonPremultiplied.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < PriorityActiveParticlesNonPremultiplied.Count; i++)
                {
                    PriorityActiveParticlesNonPremultiplied[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            #endregion
            #region 渲染常规粒子
            if (ActiveParticlesAlpha.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ActiveParticlesAlpha.Count; i++)
                {
                    ActiveParticlesAlpha[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ActiveParticlesAdditive.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ActiveParticlesAdditive.Count; i++)
                {
                    ActiveParticlesAdditive[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ActiveParticlesNonPremultiplied.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ActiveParticlesNonPremultiplied.Count; i++)
                {
                    ActiveParticlesNonPremultiplied[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            #endregion
            #endregion
        }
    }
}
