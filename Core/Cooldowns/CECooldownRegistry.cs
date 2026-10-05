using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Cooldowns
{
    /// <summary>按静态 ID 建表,没有就用类型全名,存档和同步用这个字符串</summary>
    public sealed class CECooldownRegistry : ModSystem
    {
        private static Dictionary<string, Type> handlerTypes;

        public override void PostSetupContent() {
            handlerTypes = new Dictionary<string, Type>(64);
            foreach (var handler in ModContent.GetContent<CECooldownHandler>()) {
                Type type = handler.GetType();
                string id = (string)type.GetProperty("ID", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                id ??= handler.FullName;
                handlerTypes[id] = type;
            }
        }

        public override void Unload() {
            handlerTypes = null;
        }

        public static bool TryGetHandlerType(string id, out Type handlerType) {
            handlerType = null;
            return handlerTypes != null && handlerTypes.TryGetValue(id, out handlerType);
        }

        public static CECooldownHandler CreateHandler(string id) {
            if (!TryGetHandlerType(id, out Type type))
                return null;
            return Activator.CreateInstance(type) as CECooldownHandler;
        }
    }
}
