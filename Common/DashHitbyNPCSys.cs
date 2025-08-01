using CalamityMod;
using CalamityMod.NPCs;
using InnoVault;
using InnoVault.GameSystem;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    internal class DashHitbyNPCSys : PlayerOverride
    {
        private bool OnDashNPC(NPC n)
        {
            if (Main.gameMenu)
            {
                return false;
            }
            if (n.dontTakeDamage || n.friendly)
            {
                return false;
            }
            return true;
        }

        internal static void SendDashNPCDoUpdate(NPC target, int playerIndex)
        {
            ModPacket modPacket = CalamityEntropy.Instance.GetPacket();
            modPacket.Write((byte)CEMessageType.SendDashNPCDoUpdate);
            modPacket.Write(target.whoAmI);
            modPacket.Write(playerIndex);
            modPacket.Send();
        }

        internal static void HandlerDashNPCDoUpdateNet(BinaryReader reader, int whoAmI)
        {
            int npcIndex = reader.ReadInt32();
            int playerIndex = reader.ReadInt32();
            if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
            {
                return;
            }

            DashNPCDoUpdateInner(Main.npc[npcIndex], playerIndex, false);

            if (!VaultUtils.isServer)
            {
                return;
            }

            ModPacket modPacket = CalamityEntropy.Instance.GetPacket();
            modPacket.Write((byte)CEMessageType.SendDashNPCDoUpdate);
            modPacket.Write(npcIndex);
            modPacket.Write(playerIndex);
            modPacket.Send(-1, whoAmI);
        }

        internal static void DashNPCDoUpdateInner(NPC target, int playerIndex, bool updateNet = true)
        {
            if (!target.Alives())
            {
                return;
            }

            int dashImmunityTime = target.Calamity().dashImmunityTime[playerIndex];
            if (dashImmunityTime == 0)
            {
                return;
            }

            int sourceID = VaultUtils.GetSourceNPC(target.type);
            if (sourceID == -1)
            {
                return;
            }

            int[] hovers = VaultUtils.SourceNPCByHoverNPC(sourceID);
            if (hovers.Length == 0)
            {
                return;
            }

            foreach (var npc in Main.ActiveNPCs)
            {
                bool result = false;
                foreach (var id in hovers)
                {
                    if (npc.type == id)
                    {
                        result = true;
                    }
                }

                if (!result || npc.whoAmI == target.whoAmI)
                {
                    continue;
                }

                if (!npc.TryGetGlobalNPC<CalamityGlobalNPC>(out var hoverCalNPC))
                {
                    continue;
                }

                hoverCalNPC.dashImmunityTime[playerIndex] = dashImmunityTime;
            }

            if (VaultUtils.isSinglePlayer || !updateNet)
            {
                return;
            }

            SendDashNPCDoUpdate(target, playerIndex);
        }

        public override bool On_GiveImmuneTimeForCollisionAttack(int time)
        {
            NPC target = Player.Center.FindClosestNPC(Player.width * 2, true, true, null, OnDashNPC);

            if (!target.Alives())
            {
                return true;
            }

            DashNPCDoUpdateInner(target, Player.whoAmI);

            return true;
        }
    }
}
