using CalamityEntropy.Content.Items;
using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.NPCs.PrimordialWyrm;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityEntropy.Content.Tiles
{
    public class AbyssalAltarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<AbyssalAltar>());
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
            TileObjectData.addTile(Type);
            Main.tileFrameImportant[(int)base.Type] = true;
            AddMapEntry(new Color(134, 180, 240), CalamityUtils.GetItemName<AbyssalAltar>());

            DustType = (int)CalamityDusts.PurpleCosmilite;
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<WyrmTooth>())
            {
                Player player = Main.LocalPlayer;
                int type = ModContent.NPCType<PrimordialWyrmHead>();
                if (NPC.AnyNPCs(type))
                {
                    return false;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);

                return true;
            }
            return false;
        }
    }
}
