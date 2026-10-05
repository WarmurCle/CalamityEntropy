using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Core.Cooldowns
{
    /// <summary>CECooldownInstance 的形状对齐原 CooldownInstance,标识用字符串 ID,不引入 netID</summary>
    public class CECooldownInstance
    {
        private const string DurationSaveKey = "duration";
        private const string TimeLeftSaveKey = "timeLeft";

        public readonly string ID;

        public Player player;

        /// <summary>duration 的单位是帧</summary>
        public int duration;

        /// <summary>timeLeft 的单位是帧</summary>
        public int timeLeft;

        /// <summary>handler 在查不到 ID 时为 null,调用方把它丢掉</summary>
        public CECooldownHandler handler;

        /// <summary>Completion 刚开始是 1,结束是 0</summary>
        public float Completion => duration != 0 ? timeLeft / (float)duration : 0;

        public CECooldownInstance(Player p, string id, int dur) {
            ID = id;
            player = p;
            duration = dur;
            timeLeft = dur;
            handler = CECooldownRegistry.CreateHandler(id);
            if (handler != null)
                handler.instance = this;
        }

        internal CECooldownInstance(Player p, string id, TagCompound tag)
            : this(p, id, tag.GetAsInt(DurationSaveKey)) {
            timeLeft = tag.GetAsInt(TimeLeftSaveKey);
        }

        internal TagCompound Save() {
            return new TagCompound
            {
                { DurationSaveKey, duration },
                { TimeLeftSaveKey, timeLeft }
            };
        }

        internal void Write(BinaryWriter writer) {
            writer.Write(ID);
            writer.Write(duration);
            writer.Write(timeLeft);
        }

        internal static CECooldownInstance Read(BinaryReader reader, Player player) {
            string id = reader.ReadString();
            int duration = reader.ReadInt32();
            int timeLeft = reader.ReadInt32();
            return new CECooldownInstance(player, id, duration) { timeLeft = timeLeft };
        }
    }
}
