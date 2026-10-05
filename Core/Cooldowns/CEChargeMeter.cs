using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Core.Cooldowns
{
    /// <summary>玩家用 GetChargeMeter(key, max),物品用 item.GetChargeMeter(max)</summary>
    public class CEChargeMeter
    {
        public float Charge;

        public float Max = 1f;

        public bool Ready => Charge >= Max;

        /// <summary>Ratio 的范围是 0 到 1</summary>
        public float Ratio => Max > 0 ? Math.Clamp(Charge / Max, 0f, 1f) : 0f;

        public CEChargeMeter() { }

        public CEChargeMeter(float max) {
            Max = max;
        }

        /// <summary>返回这次是否刚好充满</summary>
        public bool Gain(float amount) {
            bool wasReady = Ready;
            Charge = Math.Min(Charge + amount, Max);
            return !wasReady && Ready;
        }

        public bool Consume() {
            if (!Ready)
                return false;
            Charge = 0f;
            return true;
        }

        public bool Consume(float amount) {
            if (Charge < amount)
                return false;
            Charge -= amount;
            return true;
        }

        public void Reset() {
            Charge = 0f;
        }

        /// <summary>PlayReadyCue 在 Gain 刚返回 true 时调</summary>
        public static void PlayReadyCue(Player player) {
            if (Main.dedServ)
                return;
            SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/WulfrumPingReady") { Volume = 0.6f }, player.Center);
        }

        internal TagCompound Save() {
            return new TagCompound
            {
                { "charge", Charge },
                { "max", Max }
            };
        }

        internal static CEChargeMeter Load(TagCompound tag) {
            return new CEChargeMeter {
                Charge = tag.GetFloat("charge"),
                Max = tag.GetFloat("max")
            };
        }
    }

    public class CEChargeGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        /// <summary>meter 在没用过时是 null</summary>
        public CEChargeMeter meter;

        public override GlobalItem Clone(Item from, Item to) {
            CEChargeGlobalItem clone = (CEChargeGlobalItem)base.Clone(from, to);
            if (meter != null)
                clone.meter = new CEChargeMeter(meter.Max) { Charge = meter.Charge };
            return clone;
        }

        public override void SaveData(Item item, TagCompound tag) {
            if (meter != null && meter.Charge > 0)
                tag["ceCharge"] = meter.Save();
        }

        public override void LoadData(Item item, TagCompound tag) {
            if (tag.TryGet("ceCharge", out TagCompound meterTag))
                meter = CEChargeMeter.Load(meterTag);
        }

        public override void NetSend(Item item, System.IO.BinaryWriter writer) {
            writer.Write(meter != null);
            if (meter != null) {
                writer.Write(meter.Charge);
                writer.Write(meter.Max);
            }
        }

        public override void NetReceive(Item item, System.IO.BinaryReader reader) {
            if (reader.ReadBoolean()) {
                meter ??= new CEChargeMeter();
                meter.Charge = reader.ReadSingle();
                meter.Max = reader.ReadSingle();
            }
        }
    }
}
