using System;
using System.IO;
using Terraria;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// 原版 netOffset 每帧只消化 2~4px,冲刺会叠成锯齿,所以本类清掉它再自己纠偏
    /// 近距离误差逐帧消化,超过 SnapDistance 就硬对齐,速度用包里的
    /// Timer 和 Counter 同包过线,容差内不硬对齐
    /// </summary>
    public sealed class CEBossNetMotion
    {
        /// <summary>SnapDistance 的单位是 px,超过就硬对齐</summary>
        public const float SnapDistance = 160f;
        /// <summary>Rate 是每帧消化纠偏的比例</summary>
        public const float Rate = 0.25f;
        /// <summary>MaxFrameDelta 是帧差上限,超过就不投影</summary>
        public const int MaxFrameDelta = 2;
        /// <summary>TimerTolerance 是计时收养容差,单位是帧</summary>
        public const int TimerTolerance = 2;
        /// <summary>HeartbeatFrames 是空闲心跳间隔,单位是帧</summary>
        public const int HeartbeatFrames = 45;

        private Vector2 predictedNext;
        private bool hasPrediction;
        /// <summary>还没消化的纠偏,服务端减本地</summary>
        private Vector2 pending;

        private bool timingPending;
        private int packetStateId = -1;
        private int packetTimer;
        private int packetCounter;

        public void BeginFrame(NPC npc) {
            npc.netOffset = Vector2.Zero;
            if (pending == Vector2.Zero) {
                return;
            }
            Vector2 step = pending.LengthSquared() < 1f ? pending : pending * Rate;
            npc.position += step;
            pending -= step;
        }

        /// <summary>EndFrame 记下 position 加 velocity,原版随后才把速度加进 position</summary>
        public void EndFrame(NPC npc) {
            predictedNext = npc.position + npc.velocity;
            hasPrediction = true;
        }

        /// <summary>ForgetPrediction 在直写位置后丢掉预测,下一包不当失步</summary>
        public void ForgetPrediction() {
            hasPrediction = false;
            pending = Vector2.Zero;
        }

        /// <summary>OnSnapshot 在 ReceiveExtraAI 里调,那时 position 已被服务端覆盖</summary>
        /// <param name="frameDelta">本地帧减包内帧,未知填 0</param>
        public void OnSnapshot(NPC npc, int frameDelta) {
            npc.netOffset = Vector2.Zero;
            if (!hasPrediction) {
                return;
            }

            //包里的位置停在发包帧,这里沿速度推到本地时钟
            Vector2 serverNow = npc.position + npc.velocity * frameDelta;
            Vector2 error = serverNow - predictedNext;

            if (error.LengthSquared() > SnapDistance * SnapDistance) {
                npc.position = serverNow;
                pending = Vector2.Zero;
                return;
            }

            npc.position = predictedNext;
            pending = error;
        }

        /// <summary>WriteTiming 在 SendExtraAI 里调,格式只写在这里</summary>
        public static void WriteTiming(BinaryWriter writer, int stateId, int timer, int counter) {
            writer.Write(stateId);
            writer.Write(timer);
            writer.Write(counter);
        }

        /// <summary>ReceiveTiming 在 ReceiveExtraAI 里调,字段顺序和 WriteTiming 一致</summary>
        public void ReceiveTiming(BinaryReader reader, NPC npc, int localStateId, int localTimer) {
            int stateId = reader.ReadInt32();
            int timer = reader.ReadInt32();
            int counter = reader.ReadInt32();
            ReceiveTiming(stateId, timer, counter, npc, localStateId, localTimer);
        }

        /// <summary>计时走同步槽时从这里进入 ReceiveTiming</summary>
        public void ReceiveTiming(int stateId, int timer, int counter, NPC npc, int localStateId, int localTimer) {
            packetStateId = stateId;
            packetTimer = timer;
            packetCounter = counter;
            timingPending = true;

            //换态包比不了相位,frameDelta 按 0 帧算
            int frameDelta = 0;
            if (stateId == localStateId) {
                int d = localTimer - timer;
                if (Math.Abs(d) <= MaxFrameDelta) {
                    frameDelta = d;
                }
            }
            OnSnapshot(npc, frameDelta);
        }

        /// <summary>TryTakeTiming 在同态时由 AI 开头取,换态时由 OnStateChanged 取,读后即清</summary>
        public bool TryTakeTiming(int localStateId, out int timer, out int counter) {
            timer = 0;
            counter = 0;
            if (!timingPending || packetStateId != localStateId) {
                return false;
            }
            timingPending = false;
            timer = packetTimer;
            counter = packetCounter;
            return true;
        }

        /// <summary>AdoptTimer 在容差内留本地值,硬对齐会跳过 Timer == N</summary>
        public static int AdoptTimer(int local, int synced, int tolerance = TimerTolerance) {
            return Math.Abs(synced - local) > tolerance ? synced : local;
        }

        /// <summary>确定性重算的部件,逐帧清 netOffset</summary>
        public static void ClearSmoothing(NPC npc) {
            npc.netOffset = Vector2.Zero;
        }

        /// <summary>
        /// 抖动写 netOffset,BeginFrame 会清掉,只影响本帧贴图
        /// 不要写 position,!dedServ 里只有客户端在飘
        /// </summary>
        public static void DrawShake(NPC npc, Vector2 offset) {
            npc.netOffset = offset;
        }
    }
}
