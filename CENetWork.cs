using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Content.UI.Poops;
using CalamityMod.Events;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
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
        SyncEntropyMode,
        RuneDashSync,
        SendDashNPCDoUpdate,
        SyncPlayer = 255
    }

    internal class CENetWork
    {
        public static void Handle(BinaryReader reader, int whoAmI)
        {
            CEMessageType messageType = (CEMessageType)reader.ReadByte();
            if (messageType == CEMessageType.LotteryMachineRightClicked)
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
                        packet.Send();//如果接受端是服务器，说明是来自客户端的广播，所以可以忽略来源的客户端
                        //by polaris:这个客户端发送的时候并没有执行对应的代码，得给客户端发一遍
                    }
                }
            }
            else if (messageType == CEMessageType.TurnFriendly)
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
                    packet.Send();
                }
            }
            else if (messageType == CEMessageType.Text)
            {
                Main.NewText(reader.ReadString());
            }
            else if (messageType == CEMessageType.BossKilled)
            {
                bool flag = reader.ReadBoolean();
                if (ModContent.GetInstance<Config>().BindingOfIsaac_Rep_BossMusic && !Main.dedServ && noMusTime <= 0 && !BossRushEvent.BossRushActive && (ModContent.GetInstance<Config>().RepBossMusicReplaceCalamityMusic || flag))
                {
                    noMusTime = 300;
                    SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/Music/RepTrackJingle"));
                }
            }
            else if (messageType == CEMessageType.PlayerSetRB)
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
                    packet.Send();
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
            else if (messageType == CEMessageType.PlayerSetPos)
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
            else if (messageType == CEMessageType.VoidTouchDamageShow)
            {
                if (!Main.dedServ)
                {
                    NPC npc = reader.ReadInt32().ToNPC();
                    int damageDone = reader.ReadInt32();
                    CombatText.NewText(npc.getRect(), new Color(148, 148, 255), damageDone);
                }
            }
            else if (messageType == CEMessageType.PoopSync)
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
            else if (messageType == CEMessageType.SpawnItem)
            {
                int plr = reader.ReadInt32();
                int itemtype = reader.ReadInt32();
                int stack = reader.ReadInt32();
                Player player = plr.ToPlayer();

                if (!Main.dedServ && itemtype == ModContent.ItemType<PoopPickup>())
                {
                    CEUtils.PlaySound("fart", 1, player.Center);
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
            else if (messageType == CEMessageType.PickUpPoop)
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
            else if (messageType == CEMessageType.SyncEntropyMode)
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
            else if (messageType == CEMessageType.RuneDashSync)
            {
                int wai = reader.ReadInt32();
                Player player = wai.ToPlayer();
                float dir = reader.ReadSingle();
                bool e = reader.ReadBoolean();
                if (e)
                {
                    player.Entropy().RuneDash = 0;
                }
                else
                {
                    player.Entropy().RuneDash = RuneWing.MAXDASHTIME;
                    player.Entropy().RuneDashDir = dir;
                }
                if (Main.dedServ)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)(CEMessageType.RuneDashSync));
                    packet.Write(wai);
                    packet.Write(dir);
                    packet.Write(e);
                    packet.Send(-1, wai);
                }
            }
            else if (messageType == CEMessageType.SendDashNPCDoUpdate)
            {
                DashHitbyNPCSys.HandlerDashNPCDoUpdateNet(reader, whoAmI);
            }
            else if (messageType == CEMessageType.SyncPlayer)
            {
                int loreCount = reader.ReadInt32();
                Player plr = whoAmI.ToPlayer();
                plr.Entropy().enabledLoreItems.Clear();
                for (int i = 0; i < loreCount; i++)
                {
                    plr.Entropy().enabledLoreItems.Add(reader.ReadInt32());
                }
                int bookmarkCount = reader.ReadInt32();
                plr.Entropy().EBookStackItems = new();
                for (int i = 0; i < bookmarkCount; i++)
                {
                    Item itm = new Item(reader.ReadInt32());
                    ItemIO.Receive(itm, reader);
                    plr.Entropy().EBookStackItems.Add(itm);
                }
                if (Main.dedServ)
                {
                    ModPacket packet = Instance.GetPacket();
                    packet.Write((byte)CEMessageType.SyncPlayer);
                    packet.Write(loreCount);
                    foreach (var i in plr.Entropy().enabledLoreItems)
                    {
                        packet.Write(i);
                    }
                    foreach (var item in plr.Entropy().EBookStackItems)
                    {
                        packet.Write(item.type);
                        ItemIO.Send(item, packet);
                    }
                    packet.Send(-1, whoAmI);
                }
            }
        }
    }
}
