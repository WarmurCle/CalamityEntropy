using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles
{
    public abstract class BaseParticle
    {
        #region 基础属性
        public bool Important = false;
        /// <summary>
        /// 使用材质
        /// </summary>
        public virtual string Texture => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');

        /// <summary>
        /// 该粒子存在了多少帧，一般不需要手动修改这个值
        /// </summary>
        public int Time;

        /// <summary>
        /// 粒子的存在时间上限
        /// </summary>
        public int Lifetime = 0;

        /// <summary>
        /// 位置与向量
        /// </summary>
        public Vector2 Position;
        public Vector2 Velocity;

        public Vector2 Origin;
        public Color DrawColor;
        public float Rotation;
        public float Scale = 1f;

        /// <summary>
        /// 粒子的透明度
        /// </summary>
        public float Opacity = 1f;

        /// <summary>
        /// 生命周期的进度，介于0到1之间。
        /// 0表示粒子刚生成，1表示粒子消失。
        /// </summary>
        public float LifetimeRatio => Time / (float)Lifetime;

        /// <summary>
        /// 渲染的混合模式，默认为<see cref="BlendState.AlphaBlend"/>.
        /// </summary>
        public virtual BlendState BlendState => BlendState.AlphaBlend;
        #endregion

        /// <summary>
        /// 在世界内生成粒子
        /// </summary>
        /// <returns></returns>
        public BaseParticle Spawn()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            // 初始化时间
            Time = 0;
            //这个大改优化的粒子系统不会像原灾那样卡炸
            //大概吧
            if (BlendState == BlendState.AlphaBlend)
            {
                if (!Important && BaseParticleManager.ActiveParticlesAlpha.Count > 50000)
                    BaseParticleManager.ActiveParticlesAlpha.RemoveAt(0);
                BaseParticleManager.ActiveParticlesAlpha.Add(this);
            }
            else if(BlendState == BlendState.Additive)
            {
                if (!Important && BaseParticleManager.ActiveParticlesAdditive.Count > 50000)
                    BaseParticleManager.ActiveParticlesAdditive.RemoveAt(0);
                BaseParticleManager.ActiveParticlesAdditive.Add(this);
            }
            else
            {
                if (!Important && BaseParticleManager.ActiveParticlesNonPremultiplied.Count > 50000)
                    BaseParticleManager.ActiveParticlesNonPremultiplied.RemoveAt(0);
                BaseParticleManager.ActiveParticlesNonPremultiplied.Add(this);
            }
            OnSpawn();
            return this;
        }       
        /// <summary>     
        /// 在世界内生成粒子   
        /// </summary>
        /// <returns></returns>
        public BaseParticle SpawnToPriority()
        {
            if (Main.netMode == NetmodeID.Server)
                return this;
            // 初始化时间
            Time = 0;
            if (BlendState == BlendState.AlphaBlend)
            {
                if (!Important && BaseParticleManager.PriorityActiveParticlesAlpha.Count > 50000)
                    BaseParticleManager.PriorityActiveParticlesAlpha.RemoveAt(0);
                BaseParticleManager.PriorityActiveParticlesAlpha.Add(this);
            }
            else if (BlendState == BlendState.Additive)
            {
                if (!Important && BaseParticleManager.PriorityActiveParticlesAdditive.Count > 50000)
                    BaseParticleManager.PriorityActiveParticlesAdditive.RemoveAt(0);
                BaseParticleManager.PriorityActiveParticlesAdditive.Add(this);
            }
            else
            {
                if (!Important && BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Count > 50000)
                    BaseParticleManager.PriorityActiveParticlesNonPremultiplied.RemoveAt(0);
                BaseParticleManager.PriorityActiveParticlesNonPremultiplied.Add(this);
            }
            OnSpawn();
            return this;
        }
        public virtual void OnSpawn() { }

        /// <summary>
        /// 粒子的更新，默认不做任何操作
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// 立刻清除粒子
        /// </summary>
        public void Kill()
        {
            Time = Lifetime;
        }

        public virtual void OnKill() { }

        /// <summary>
        /// 覆写这个就可以自定义绘制
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
