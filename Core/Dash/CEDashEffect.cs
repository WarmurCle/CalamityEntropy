using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Dash
{
    /// <summary>OnHit 填写 CEDashHit,Damage 为 0 时不结算</summary>
    public struct CEDashHit
    {
        /// <summary>引擎给 Damage 套职业加成和暴击</summary>
        public int Damage;
        public float Knockback;
        public int PlayerImmuneFrames;
        /// <summary>DamageClass 默认是 Generic</summary>
        public DamageClass DamageClass;
    }

    /// <summary>效果是无状态单例,随冲刺变的数据在这里</summary>
    public sealed class CEDashState
    {
        public CEDashEffect Effect;
        public CEDashEnhancer Enhancer;
        /// <summary>Direction 是单位向量</summary>
        public Vector2 Direction;
        /// <summary>Timer 在首帧是 0</summary>
        public int Timer;
        /// <summary>EntryVelocity 记下起手速度,收尾时还一部分竖直分量</summary>
        public Vector2 EntryVelocity;
        public int Duration;
        /// <summary>Speeds 是每帧位移,单位 px</summary>
        public float[] Speeds = Array.Empty<float>();
        /// <summary>远端只复现视觉,不写速度不判撞击</summary>
        public bool Remote;
        public int HitCount;
        public readonly HashSet<int> HitNPCs = new();
        /// <summary>EffectData 是效果的私有槽</summary>
        public object EffectData;
        /// <summary>EnhancerData 是强化器的私有槽</summary>
        public object EnhancerData;

        public bool Horizontal => Direction.Y == 0f;
        public float Progress => Duration <= 0 ? 1f : MathHelper.Clamp(Timer / (float)Duration, 0f, 1f);
        public float CurrentSpeed => Timer >= 0 && Timer < Speeds.Length ? Speeds[Timer] : 0f;
        /// <summary>Invincible 含弹幕无敌,效果或强化器任一为真即可</summary>
        public bool Invincible => Effect.Invincible || (Enhancer != null && Enhancer.Invincible);
        public bool Enhanced => Enhancer != null;
        public int HorizontalSign(Player player) => Direction.X != 0f ? Math.Sign(Direction.X) : player.direction;
    }

    /// <summary>
    /// 饰品在 UpdateAccessory 里调 Offer
    /// 效果子类用无参构造,不得持有随冲刺变的字段
    /// </summary>
    public abstract class CEDashEffect
    {
        /// <summary>ID 写入 LastUsedDashID,联机用它反查</summary>
        public abstract string ID { get; }

        /// <summary>Priority 高的接管,相同就以后登记的为准</summary>
        public virtual int Priority => 0;

        /// <summary>UsesDoubleTap 为假时只能靠 Hotkey</summary>
        public virtual bool UsesDoubleTap => true;

        /// <summary>Omnidirectional 允许双击上下</summary>
        public virtual bool Omnidirectional => false;

        /// <summary>Hotkey 非空时 ProcessTriggers 监听它</summary>
        public virtual ModKeybind Hotkey => null;

        /// <summary>HotkeyDirection 为空时本次不起手</summary>
        public virtual Vector2? HotkeyDirection(Player player) => null;

        public virtual bool CanInterrupt => false;

        /// <summary>原版驱动,引擎不写速度不判墙</summary>
        public virtual bool ExternalMotion => false;

        /// <summary>Duration 按帧计</summary>
        public abstract int Duration { get; }

        /// <summary>Distance 是自由空间总位移,单位 px</summary>
        public abstract float Distance { get; }

        /// <summary>结束后的锁定帧,期间不能起手</summary>
        public virtual int Cooldown => 30;

        /// <summary>Curve 是缓出指数,1 为线性</summary>
        public virtual float Curve => 2f;

        public virtual float GravityMult => 0.3f;

        /// <summary>DampVertical 在水平冲刺且不按跳时衰减竖直速度</summary>
        public virtual bool DampVertical => true;

        /// <summary>VerticalRetain 是全程竖直保留比,若每帧乘 0.85,20 帧只剩 4%</summary>
        public virtual float VerticalRetain => 0.35f;

        /// <summary>ExitVerticalCarry 是收尾时还起手竖直速度的比例,0 表示不还</summary>
        public virtual float ExitVerticalCarry => 0.6f;

        /// <summary>Invincible 为真时全程无敌,弹幕也穿得过</summary>
        public virtual bool Invincible => false;

        /// <summary>HitsEnemies 为真时穿过敌人并结算撞击</summary>
        public virtual bool HitsEnemies => false;

        public virtual bool IgnorePlatforms => false;

        public virtual bool CanBeEnhanced => true;

        public virtual float EndSpeed(Player player, Vector2 direction) {
            if (direction.Y != 0f)
                return 4f;
            return MathHelper.Clamp(Math.Max(player.accRunSpeed, player.maxRunSpeed), 3f, 9f);
        }

        /// <summary>CanStart 调用前,引擎已排除坐骑、控制、死亡和锁定帧</summary>
        public virtual bool CanStart(Player player) => true;

        /// <summary>OnStart 只在本地调</summary>
        public virtual void OnStart(Player player, CEDashState state) { }

        /// <summary>OnVisuals 在位移之后调,本地和远端都走</summary>
        public virtual void OnVisuals(Player player, CEDashState state) { }

        /// <summary>OnHit 每只怪每次冲刺只进一次,伤害填进 hit</summary>
        public virtual void OnHit(Player player, NPC npc, CEDashState state, ref CEDashHit hit) { }

        /// <summary>OnEnd 在本地和远端都调</summary>
        public virtual void OnEnd(Player player, CEDashState state) { }
    }

    /// <summary>强化器是无状态单例,TryConsume 通过后这一次冲刺被强化</summary>
    public abstract class CEDashEnhancer
    {
        public abstract string ID { get; }

        /// <summary>TryConsume 只在本地判断</summary>
        public abstract bool TryConsume(Player player);

        /// <summary>SpeedMult 同时放大位移和收尾速度</summary>
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
        /// <summary>Duration 给远端视觉用,本地看 dashDelay 回到非负</summary>
        public override int Duration => 30;
        public override float Distance => 0f;
        public override int Cooldown => 0;
        public override bool CanBeEnhanced => true;
    }
}
