using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CalamityEntropy.Core.AI
{
    /// <summary>CEBossNetDiag 是 DEBUG 诊断,Conditional 连实参一起删,调用处不用再套 #if</summary>
    public static class CEBossNetDiag
    {
#if DEBUG
        /// <summary>键是 标签|W 或 标签|R</summary>
        private static readonly Dictionary<string, long> blockMarks = new();
        /// <summary>没给期望字节数时,sizeReported 让每个标签只报一次</summary>
        private static readonly HashSet<string> sizeReported = new();
        /// <summary>收包在 socket 线程,字典并发改会死循环</summary>
        private static readonly object gate = new();

        private static void Log(string line, bool warn) {
            CalamityEntropy inst = CalamityEntropy.Instance;
            if (inst == null) {
                return;
            }
            if (warn) {
                inst.Logger.Warn(line);
            }
            else {
                inst.Logger.Debug(line);
            }
        }

        private static void Mark(string key, Stream stream) {
            if (stream == null || !stream.CanSeek) {
                return;
            }
            lock (gate) {
                blockMarks[key] = stream.Position;
            }
        }

        private static void Measure(string key, string tag, string side, Stream stream, int expectedBytes) {
            if (stream == null || !stream.CanSeek) {
                return;
            }
            long start;
            bool first;
            lock (gate) {
                if (!blockMarks.TryGetValue(key, out start)) {
                    return;
                }
                blockMarks.Remove(key);
                first = expectedBytes < 0 && sizeReported.Add(key);
            }
            int len = (int)(stream.Position - start);
            if (expectedBytes < 0) {
                if (first) {
                    Log($"[CEBossNet] {tag} {side} 定长块实测 {len} 字节,把它填进两端共用的常量里", false);
                }
                return;
            }
            if (len != expectedBytes) {
                Log($"[CEBossNet] {tag} {side} 定长块字节数不符:实测 {len},契约 {expectedBytes}。"
                    + "这一侧的读写顺序已经和契约脱节,整条共享流会从这里开始错位", true);
            }
        }
#endif

        /// <summary>
        /// TimingAdopted 在收养越过容差时打点,挂在 AdoptNetTiming 上
        /// 不要写在 ReceiveExtraAI,那里计时还在待收养槽,比较恒假
        /// </summary>
        /// <param name="localTimer">localTimer 取收养前的本地计时</param>
        [Conditional("DEBUG")]
        public static void TimingAdopted(string state, int stateId, int localTimer, int syncedTimer) {
#if DEBUG
            int delta = syncedTimer - localTimer;
            if (Math.Abs(delta) > CEBossNetMotion.TimerTolerance) {
                Log($"[CEBossNet] {state}(#{stateId}) 计时收养越过容差:本地 {localTimer} ← 权威 {syncedTimer}(Δ{delta})", false);
            }
#endif
        }

        /// <summary>
        /// BeginWrite 和 EndWrite 夹住 SendExtraAI 块体
        /// NPCLoader 只在整条流末尾查长度,块内错位会甩给后面的 GlobalNPC
        /// </summary>
        [Conditional("DEBUG")]
        public static void BeginWrite(string tag, BinaryWriter writer) {
#if DEBUG
            Mark(tag + "|W", writer?.BaseStream);
#endif
        }

        /// <summary>EndWrite 比的是两端共用的常量,不比对端,也不把长度写进流</summary>
        /// <param name="expectedBytes">expectedBytes 为负数时只观测一次</param>
        [Conditional("DEBUG")]
        public static void EndWrite(string tag, BinaryWriter writer, int expectedBytes = -1) {
#if DEBUG
            Measure(tag + "|W", tag, "写侧", writer?.BaseStream, expectedBytes);
#endif
        }

        /// <summary>BeginRead 和 EndRead 夹住 ReceiveExtraAI 块体</summary>
        [Conditional("DEBUG")]
        public static void BeginRead(string tag, BinaryReader reader) {
#if DEBUG
            Mark(tag + "|R", reader?.BaseStream);
#endif
        }

        /// <summary>EndRead 和 EndWrite 共用同一个字节数常量</summary>
        [Conditional("DEBUG")]
        public static void EndRead(string tag, BinaryReader reader, int expectedBytes = -1) {
#if DEBUG
            Measure(tag + "|R", tag, "读侧", reader?.BaseStream, expectedBytes);
#endif
        }
    }
}
