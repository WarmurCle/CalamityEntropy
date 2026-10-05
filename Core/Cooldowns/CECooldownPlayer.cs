using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Core.Cooldowns
{
    /// <summary>各端本地推进,不逐帧同步,加入时整表走 SyncCooldowns</summary>
    public class CECooldownPlayer : ModPlayer
    {
        private const string CooldownsSaveKey = "ceCooldowns";
        private const string ChargesSaveKey = "ceCharges";

        /// <summary>键是字符串 ID</summary>
        public Dictionary<string, CECooldownInstance> cooldowns;

        public Dictionary<string, CEChargeMeter> charges;

        public override void Initialize() {
            cooldowns = new Dictionary<string, CECooldownInstance>(16);
            charges = new Dictionary<string, CEChargeMeter>();
        }

        #region 增删查
        /// <summary>倍率原在 EModILEdit 钩灾厄 AddCooldown</summary>
        public CECooldownInstance Add(string id, int duration, bool overwrite = true) {
            duration = (int)(duration * Player.Entropy().CooldownTimeMult);
            var instance = new CECooldownInstance(Player, id, duration);
            if (instance.handler == null) {
                CalamityEntropy.Instance?.Logger?.Warn($"冷却 \"{id}\" 未注册,AddCooldown 被忽略。");
                return null;
            }

            if (overwrite || !cooldowns.ContainsKey(id))
                cooldowns[id] = instance;

            return instance;
        }

        public bool Has(string id) => cooldowns.ContainsKey(id);

        public bool TryGet(string id, out CECooldownInstance instance) => cooldowns.TryGetValue(id, out instance);

        public bool Remove(string id) => cooldowns.Remove(id);

        public void Clear() => cooldowns.Clear();

        public IList<CECooldownInstance> GetDisplayed() {
            List<CECooldownInstance> result = new List<CECooldownInstance>(cooldowns.Count);
            foreach (CECooldownInstance instance in cooldowns.Values) {
                if (instance.handler.ShouldDisplay)
                    result.Add(instance);
            }
            return result;
        }

        public CEChargeMeter GetCharge(string key, float max) {
            if (!charges.TryGetValue(key, out CEChargeMeter meter)) {
                meter = new CEChargeMeter(max);
                charges[key] = meter;
            }
            else {
                meter.Max = max;
            }
            return meter;
        }
        #endregion

        #region 逐帧推进
        public override void PostUpdateMiscEffects() {
            TickCooldowns();
        }

        private void TickCooldowns() {
            if (cooldowns.Count == 0)
                return;

            List<string> expired = null;
            foreach (var kv in cooldowns) {
                CECooldownInstance instance = kv.Value;
                CECooldownHandler handler = instance.handler;

                if (handler.CanTickDown)
                    --instance.timeLeft;

                //Tick 跟减不减无关
                handler.Tick();

                if (instance.timeLeft < 0) {
                    handler.OnCompleted();
                    if (!Main.dedServ && handler.EndSound != null && handler.ShouldPlayEndSound)
                        SoundEngine.PlaySound(handler.EndSound.GetValueOrDefault(), Player.Center);
                    (expired ??= new List<string>()).Add(kv.Key);
                }
            }

            if (expired != null) {
                foreach (string id in expired)
                    cooldowns.Remove(id);
            }
        }

        public override void UpdateDead() {
            if (cooldowns.Count == 0)
                return;

            List<string> removed = null;
            foreach (var kv in cooldowns) {
                if (!kv.Value.handler.PersistsThroughDeath)
                    (removed ??= new List<string>()).Add(kv.Key);
            }
            if (removed != null) {
                foreach (string id in removed)
                    cooldowns.Remove(id);
            }
        }
        #endregion

        #region 存档
        public override void SaveData(TagCompound tag) {
            TagCompound cdTag = new TagCompound();
            foreach (var kv in cooldowns) {
                if (kv.Value.handler.SavedWithPlayer)
                    cdTag[kv.Key] = kv.Value.Save();
            }
            tag[CooldownsSaveKey] = cdTag;

            TagCompound chargeTag = new TagCompound();
            foreach (var kv in charges) {
                if (kv.Value.Charge > 0)
                    chargeTag[kv.Key] = kv.Value.Save();
            }
            tag[ChargesSaveKey] = chargeTag;
        }

        public override void LoadData(TagCompound tag) {
            cooldowns.Clear();
            if (tag.TryGet(CooldownsSaveKey, out TagCompound cdTag)) {
                foreach (var kv in cdTag) {
                    var instance = new CECooldownInstance(Player, kv.Key, cdTag.GetCompound(kv.Key));
                    if (instance.handler != null)
                        cooldowns[kv.Key] = instance;
                    else
                        CalamityEntropy.Instance?.Logger?.Warn($"存档中的冷却 \"{kv.Key}\" 未注册,已丢弃。");
                }
            }

            charges.Clear();
            if (tag.TryGet(ChargesSaveKey, out TagCompound chargeTag)) {
                foreach (var kv in chargeTag)
                    charges[kv.Key] = CEChargeMeter.Load(chargeTag.GetCompound(kv.Key));
            }
        }
        #endregion

        #region 加入同步序列化(发包见 CENetWork 的 SyncCooldowns 分支)
        /// <summary>SyncPlayer 路径调用后发包</summary>
        public void WriteAllCooldowns(BinaryWriter writer) {
            writer.Write((byte)Player.whoAmI);
            writer.Write((ushort)cooldowns.Count);
            foreach (var kv in cooldowns)
                kv.Value.Write(writer);
        }

        /// <summary>SyncCooldowns 分支调用,整表替换</summary>
        public static void ReceiveAllCooldowns(BinaryReader reader) {
            int whoAmI = reader.ReadByte();
            int count = reader.ReadUInt16();
            Player target = Main.player[whoAmI];
            var modPlayer = target.GetModPlayer<CECooldownPlayer>();
            modPlayer.cooldowns.Clear();
            for (int i = 0; i < count; i++) {
                CECooldownInstance instance = CECooldownInstance.Read(reader, target);
                if (instance.handler != null)
                    modPlayer.cooldowns[instance.ID] = instance;
            }
        }
        #endregion
    }
}
