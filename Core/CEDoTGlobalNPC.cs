using CalamityEntropy.Content.Buffs.PortsDoT;
using CalamityEntropy.Core.CalamityRef;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Core
{
    /// <summary>
    /// 移植减益（PortsDoT）的每类结算参数。
    /// 语义对齐灾厄 DebuffData：LostRegen 为 lifeRegen 扣减，2 点 = 每秒 1 点伤害。
    /// </summary>
    public class CEDoTEntry
    {
        /// <summary>lifeRegen 扣减量（2 = 每秒 1 点伤害）</summary>
        public int LostRegen;

        /// <summary>跳字下限</summary>
        public int MinTick = 1;

        /// <summary>跳字 = LostRegen × TickMult 与 MinTick 取大</summary>
        public float TickMult = 0.25f;

        /// <summary>电系：目标横向移动中 DoT ×4</summary>
        public bool ElectricMoving;

        /// <summary>风寒：目标浸湿（wet/honeyWet/dripping 或身负水系移植减益）时 ×1.5</summary>
        public bool WetBoost;

        /// <summary>放逐之焰：lifeMax ≥ 100 万时改用 lifeMax / 500</summary>
        public bool ScaleWithMaxLife;
    }

    /// <summary>
    /// 乘区按类型 FullName 字母序:EDamageOverTimeNPC,再 EGlobalNPC,再本类
    /// EGlobalNPC 的全局放大跑在前面,盖不到这里的扣减,本类自乘 DebuffDamageMult
    /// </summary>
    public class CEDoTGlobalNPC : GlobalNPC
    {
        /// <summary>buffType → 结算参数；由 PortsDoT 各 ModBuff 在 SetStaticDefaults 注册</summary>
        public static readonly Dictionary<int, CEDoTEntry> Registry = new Dictionary<int, CEDoTEntry>();

        public static void Register(int buffType, CEDoTEntry entry) {
            Registry[buffType] = entry;
        }

        /// <summary>
        /// 原版带电对敌怪本无效果(NPC 没有对应字段);未装灾厄时按灾厄 DebuffData.Electrified 补上:静止 15/s,横向移动中 ×4。
        /// 装了灾厄由它自己结算伤害与电火花,这里不重复注册,否则伤害与粒子都翻倍
        /// </summary>
        public override void SetStaticDefaults() {
            if (!CERef.Has) {
                Register(BuffID.Electrified, new CEDoTEntry { LostRegen = 30, ElectricMoving = true });
            }
        }

        public override void Unload() {
            Registry.Clear();
        }

        public override void DrawEffects(NPC npc, ref Color drawColor) {
            if (!CERef.Has && npc.HasBuff(BuffID.Electrified) && Main.rand.NextBool(2)) {
                Dust d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Electric, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 0.35f);
                d.noGravity = true;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            float dotMult = 0f;
            for (int i = 0; i < NPC.maxBuffs; i++) {
                if (npc.buffTime[i] <= 0 || !Registry.TryGetValue(npc.buffType[i], out var entry))
                    continue;

                // 惰性求值：只在确有移植减益时取一次倍率
                if (dotMult == 0f)
                    dotMult = npc.Entropy().DebuffDamageMult();

                int regen = entry.LostRegen;
                if (entry.ScaleWithMaxLife && npc.lifeMax >= 1000000)
                    regen = npc.lifeMax / 500;
                if (entry.ElectricMoving && npc.velocity.X != 0f)
                    regen *= 4;
                if (entry.WetBoost && IsWetTarget(npc))
                    regen = (int)(regen * 1.5f);
                if (dotMult != 1f)
                    regen = (int)(regen * dotMult);

                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= regen;

                int tick = Math.Max((int)(regen * entry.TickMult), entry.MinTick);
                if (damage < tick)
                    damage = tick;
            }
        }

        private static bool IsWetTarget(NPC npc) {
            return npc.wet || npc.honeyWet || npc.dripping
                || npc.HasBuff<CrushDepth>() || npc.HasBuff<HadopelagicPressure>();
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
            if (npc.HasBuff<ArmorCrunch>())
                modifiers.Defense.Flat -= ArmorCrunch.DefenseReduction;
            if (npc.HasBuff<Crumbling>())
                modifiers.Defense.Flat -= Crumbling.DefenseReduction;
            if (npc.HasBuff<MarkedforDeath>())
                modifiers.SourceDamage *= MarkedforDeath.DamageTakenMult;
        }

        public override void PostAI(NPC npc) {
            float slow = 1f;
            if (npc.HasBuff<TemporalSadness>())
                slow += 0.2f;
            if (npc.HasBuff<GalvanicCorrosion>())
                slow += 0.05f;
            if (slow > 1f)
                npc.velocity /= slow;

            if (npc.HasBuff<VulnerabilityHex>() || npc.HasBuff<TrueVulnerabilityHex>())
                npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-5f, -5f), new Vector2(5f, 10f));
        }
    }
}
