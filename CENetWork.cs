using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Content.UI.Poops;
using CalamityEntropy.Util;
using CalamityMod.Events;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using static CalamityEntropy.CalamityEntropy;

namespace CalamityEntropy
{
    public enum CEMessageType : byte
    {
        LotteryMachineRightClicked,
        TurnFriendly,
        Text,
        BossKilled,
        PlayerSetRB,
        PlayerSetPos,
        VoidTouchDamageShow,
        PoopSync,
        SpawnItem,
        PickUpPoop,
        SyncEntropyMode
    }

    internal class CENetWork
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            CEMessageType type = (CEMessageType)reader.ReadByte();
            if (type == CEMessageType.LotteryMachineRightClicked)
            {
                int plr = reader.ReadInt32();
                int npc = reader.ReadInt32();
                int wai = reader.ReadInt32();
                if (npc.ToNPC().ModNPC is LotteryMachine lm)
                {
                    lm.RightClicked(Main.player[plr]);
                    if (Main.dedServ)
                    {
                        ModPacket packet = Instance.GetPacket();
                        packet.Write((byte)CEMessageType.LotteryMachineRightClicked);
                        packet.Write(plr);
                        packet.Write(npc);
                        packet.Write(wai);
                        packet.Send(-1, whoAmI);//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                    }
                }
            }
            if (type == CEMessageType.TurnFriendly)
            {
                int id = reader.ReadInt32();
                int owner = reader.ReadInt32();
                id.ToNPC().Entropy().ToFriendly = true;
                id.ToNPC().Entropy().f_owner = owner;
                if (Main.dedServ)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)CEMessageType.TurnFriendly);
                    packet.Write(id);
                    packet.Write(owner);
                    packet.Send(-1, whoAmI);//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                }
            }
            if (type == CEMessageType.Text)
            {
                Main.NewText(reader.ReadString());
            }
            if (type == CEMessageType.BossKilled)
            {
                bool flag = reader.ReadBoolean();
                if (ModContent.GetInstance<Config>().BindingOfIsaac_Rep_BossMusic && !Main.dedServ && noMusTime <= 0 && !BossRushEvent.BossRushActive && (ModContent.GetInstance<Config>().RepBossMusicReplaceCalamityMusic || flag))
                {
                    noMusTime = 300;
                    SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/Music/RepTrackJingle"));
                }
            }
            if (type == CEMessageType.PlayerSetRB)
            {
                int playerIndex = reader.ReadInt32();
                bool active = reader.ReadBoolean();
                playerIndex.ToPlayer().Entropy().rBadgeActive = active;

                if (Main.dedServ)
                {
                    if (!active)
                    {
                        playerIndex.ToPlayer().velocity *= 0.2f;
                    }
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)CEMessageType.PlayerSetRB);
                    packet.Write(playerIndex);
                    packet.Write(active);
                    packet.Send(-1, whoAmI);//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                }
                else
                {
                    if (playerIndex != Main.myPlayer)
                    {
                        if (!active)
                        {
                            playerIndex.ToPlayer().velocity *= 0.2f;
                        }
                        if (active)
                        {
                            SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/AscendantActivate"), playerIndex.ToPlayer().Center);
                        }
                        else
                        {
                            SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/AscendantOff"), playerIndex.ToPlayer().Center);
                        }
                    }
                }
            }
            if (type == CEMessageType.PlayerSetPos)
            {
                int id = reader.ReadInt32();
                Vector2 pos = reader.ReadVector2();
                if (id != Main.myPlayer)
                {
                    id.ToPlayer().Center = pos;
                }
                if (Main.dedServ)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)CEMessageType.PlayerSetPos);
                    packet.Write(id);
                    packet.WriteVector2(pos);
                    packet.Send(-1, whoAmI);//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                }
            }
            if (type == CEMessageType.VoidTouchDamageShow)
            {
                if (!Main.dedServ)
                {
                    NPC npc = reader.ReadInt32().ToNPC();
                    int damageDone = reader.ReadInt32();
                    CombatText.NewText(npc.getRect(), new Color(148, 148, 255), damageDone);
                }
            }
            if (type == CEMessageType.PoopSync)
            {
                Player player = reader.ReadInt32().ToPlayer();
                bool holding = reader.ReadBoolean();
                player.Entropy().holdingPoop = holding;
                if (Main.dedServ)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)CEMessageType.PoopSync);
                    packet.Write(player.whoAmI);
                    packet.Write(holding);
                    packet.Send(-1, whoAmI);//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                }
            }
            if (type == CEMessageType.SpawnItem)
            {
                int plr = reader.ReadInt32();
                int itemtype = reader.ReadInt32();
                int stack = reader.ReadInt32();
                Player player = plr.ToPlayer();

                if (!Main.dedServ && itemtype == ModContent.ItemType<PoopPickup>())
                {
                    Util.Util.PlaySound("fart", 1, player.Center);
                }
                if (Main.dedServ)
                {
                    int i = Item.NewItem(player.GetSource_FromThis(), player.getRect(), new Item(itemtype, stack), false, true);
                    if (i < Main.item.Length)
                    {
                        Main.item[i].noGrabDelay = 100;
                    }
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)CEMessageType.SpawnItem);
                    packet.Write(player.whoAmI);
                    packet.Write(itemtype);
                    packet.Write(stack);
                    packet.Send(-1, whoAmI);//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                }
            }
            if (type == CEMessageType.PickUpPoop)
            {
                int plr = reader.ReadInt32();
                string name = reader.ReadString();
                Player player = plr.ToPlayer();
                Poop poop = new PoopNormal();
                foreach (Poop p in Poop.instances)
                {
                    if (p.FullName == name)
                    {
                        poop = p;
                    }
                }
                player.Entropy().poops.Add(poop);
            }
            if (type == CEMessageType.SyncEntropyMode)
            {
                bool enabled = reader.ReadBoolean();
                EntropyMode = enabled;
                if (Main.dedServ)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)CEMessageType.SyncEntropyMode);
                    packet.Write(enabled);
                    packet.Send(-1, whoAmI);//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                }
                else
                {
                    if (EntropyMode)
                    {
                        Main.NewText(Instance.GetLocalization("EntropyModeActive").Value, new Color(170, 18, 225));
                    }
                    else
                    {
                        Main.NewText(Instance.GetLocalization("EntropyModeDeactive").Value, new Color(170, 18, 225));
                    }
                }
            }
        }
    }
}
