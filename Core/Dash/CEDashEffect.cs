using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Dash
{
    /// <summary>OnHit 填写,Damage 为 0 不结算</summary>
    public struct CEDashHit
    {
        /// <summary>引擎套职业加成与暴击</summary>
        public int Damage;
        public float Knockback;
        public int PlayerImmuneFrames;
        /// <summary>默认 Generic</summary>
        public DamageClass DamageClass;
    }

    /// <summary>效果是无状态单例,随冲刺变的数据在这里</summary>
    public sealed class CEDashState
    {
        public CEDashEffect Effect;
        public CEDashEnhancer Enhancer;
        /// <summary>单位向量</summary>
        public Vector2 Direction;
        /// <summary>首帧 0</summary>
        public int Timer;
        /// <summary>起手速度,收尾还竖直</summary>
        public Vector2 EntryVelocity;
        public int Duration;
        /// <summary>每帧位移,px</summary>
        public float[] Speeds = Array.Empty<float>();
        /// <summary>远端只复现视觉,不写速度不判撞击</summary>
        public bool Remote;
        public int HitCount;
        public readonly HashSet<int> HitNPCs = new();
        /// <summary>效果私有槽</summary>
        public object EffectData;
        /// <summary>强化器私有槽</summary>
        public object EnhancerData;

        public bool Horizontal => Direction.Y == 0f;
        public float Progress => Duration <= 0 ? 1f : MathHelper.Clamp(Timer / (float)Duration, 0f, 1f);
        public float CurrentSpeed => Timer >= 0 && Timer < Speeds.Length ? Speeds[Timer] : 0f;
        /// <summary>含弹幕</summary>
        public bool Invincible => Effect.Invincible || (Enhancer != null && Enhancer.Invincible);
        public bool Enhanced => Enhancer != null;
        public int HorizontalSign(Player player) => Direction.X != 0f ? Math.Sign(Direction.X) : player.direction;
    }

    /// <summary>
    /// 饰品在 UpdateAccessory 里 Offer
    /// 子类无参构造,不得持有随冲刺变的字段
    /// </summary>
    public abstract class CEDashEffect
    {
        /// <summary>写入 LastUsedDashID,联机用</summary>
        public abstract string ID { get; }

        /// <summary>高者接管,相同则后登记</summary>
        public virtual int Priority => 0;

        /// <summary>假则只能靠 Hotkey</summary>
        public virtual bool UsesDoubleTap => true;

        /// <summary>允许双击上下</summary>
        public virtual bool Omnidirectional => false;

        /// <summary>非空时 ProcessTriggers 监听</summary>
        public virtual ModKeybind Hotkey => null;

        /// <summary>空则本次不起手</summary>
        public virtual Vector2? HotkeyDirection(Player player) => null;

        public virtual bool CanInterrupt => false;

        /// <summary>原版驱动,引擎不写速度不判墙</summary>
        public virtual bool ExternalMotion => false;

        /// <summary>帧</summary>
        public abstract int Duration { get; }

        /// <summary>px,自由空间总位移</summary>
        public abstract float Distance { get; }

        /// <summary>结束后的锁定帧,期间不能起手</summary>
        public virtual int Cooldown => 30;

        /// <summary>缓出指数,1 为线性</summary>
        public virtual float Curve => 2f;

        public virtual float GravityMult => 0.3f;

        /// <summary>水平且不按跳时衰减竖直</summary>
        public virtual bool DampVertical => true;

        /// <summary>全程竖直保留比,按帧乘 0.85 的话 20 帧只剩 4%</summary>
        public virtual float VerticalRetain => 0.35f;

        /// <summary>收尾还起手竖直的比例,0 不还</summary>
        public virtual float ExitVerticalCarry => 0.6f;

        /// <summary>全程无敌,穿弹幕</summary>
        public virtual bool Invincible => false;

        /// <summary>穿过并结算撞击</summary>
        public virtual bool HitsEnemies => false;

        public virtual bool IgnorePlatforms => false;

        public virtual bool CanBeEnhanced => true;

        public virtual float EndSpeed(Player player, Vector2 direction) {
            if (direction.Y != 0f)
                return 4f;
            return MathHelper.Clamp(Math.Max(player.accRunSpeed, player.maxRunSpeed), 3f, 9f);
        }

        /// <summary>引擎已排除坐骑、控制、死亡、锁定帧</summary>
        public virtual bool CanStart(Player player) => true;

        /// <summary>仅本地</summary>
        public virtual void OnStart(Player player, CEDashState state) { }

        /// <summary>位移后,本地与远端都调</summary>
        public virtual void OnVisuals(Player player, CEDashState state) { }

        /// <summary>每怪每次冲刺一次,填 hit</summary>
        public virtual void OnHit(Player player, NPC npc, CEDashState state, ref CEDashHit hit) { }

        /// <summary>本地与远端</summary>
        public virtual void OnEnd(Player player, CEDashState state) { }
    }

    /// <summary>无状态单例,TryConsume 过了本次被强化</summary>
    public abstract class CEDashEnhancer
    {
        public abstract string ID { get; }

        /// <summary>只在本地</summary>
        public abstract bool TryConsume(Player player);

        /// <summary>位移同倍</summary>
        public virtual float SpeedMult => 1.2f;

        public virtual bool Invincible => true;

        public virtual void OnStart(Player player, CEDashState state) { }
        public virtual void OnVisuals(Player player, CEDashState state) { }
        public virtual void OnEnd(Player player, CEDashState state) { }
    }

    /// <summary>运动仍走原版 DashMovement,引擎只跟踪无敌和强化视觉</summary>
    public sealed class CEVanillaDashEffect : CEDashEffect
    {
        public override string ID => "VanillaDash";
        public override bool UsesDoubleTap => false;
        public override bool ExternalMotion => true;
        /// <summary>远端视觉用,本地看 dashDelay 回到非负</summary>
        public override int Duration => 30;
        public override float Distance => 0f;
        public override int Cooldown => 0;
        public override bool CanBeEnhanced => true;
    }
}
