using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Integrations.BossLog
{
    /// <summary>兜底只在反射可选成员缺失时用,正常读 EntryInfo</summary>
    internal sealed class CEBossLogEntry
    {
        /// <summary>BossChecklist 存档键,不能随 NPC 类名改</summary>
        public string InternalName { get; }
        public CEBossPortraitActor Actor { get; }
        public Func<bool> Downed { get; }
        public IReadOnlyList<int> NpcTypes { get; }

        public CEBossLogEntry(string internalName, CEBossPortraitActor actor, Func<bool> downed, IReadOnlyList<int> npcTypes) {
            InternalName = internalName;
            Actor = actor;
            Downed = downed;
            NpcTypes = npcTypes;
        }

        /// <summary>单 NPC 条目的默认名</summary>
        public string FallbackName {
            get {
                if (NpcTypes.Count > 0 && ModContent.GetModNPC(NpcTypes[0]) is ModNPC npc) {
                    return npc.DisplayName.Value;
                }
                return InternalName;
            }
        }

        /// <summary>与 BossChecklist 默认头图标同源</summary>
        public List<Asset<Texture2D>> FallbackHeads() {
            List<Asset<Texture2D>> heads = [];
            foreach (int type in NpcTypes) {
                if (type < 0 || type >= NPCID.Sets.BossHeadTextures.Length) {
                    continue;
                }
                int slot = NPCID.Sets.BossHeadTextures[type];
                if (slot >= 0 && slot < TextureAssets.NpcHeadBoss.Length && TextureAssets.NpcHeadBoss[slot] != null) {
                    heads.Add(TextureAssets.NpcHeadBoss[slot]);
                }
            }
            return heads;
        }
    }

    /// <summary>键是 "模组名 内部名";LogBoss 在 CEBossChecklistIntegration,这里只挂演员</summary>
    internal static class CEBossLogRegistry
    {
        private static readonly Dictionary<string, CEBossLogEntry> entries = [];

        public static IReadOnlyCollection<CEBossLogEntry> Entries => entries.Values;

        public static bool TryGet(string key, out CEBossLogEntry entry) {
            entry = null;
            return key != null && entries.TryGetValue(key, out entry);
        }

        public static string Add(Mod host, string internalName, CEBossPortraitActor actor, Func<bool> downed, object npcTypes) {
            string key = $"{host.Name} {internalName}";
            entries[key] = new CEBossLogEntry(internalName, actor, downed, AsTypeList(npcTypes));
            return key;
        }

        public static void Clear() => entries.Clear();

        private static IReadOnlyList<int> AsTypeList(object npcTypes) => npcTypes switch {
            List<int> list => list,
            int single => [single],
            _ => Array.Empty<int>(),
        };
    }
}
