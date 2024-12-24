using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.UI.Poops;
using CalamityEntropy.Util;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class PoopPickup : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 1;
        }
        
        public override bool OnPickup(Player player)
        {
            if (player.Entropy().brokenAnkh)
            {
                if (player.Entropy().poops.Count < player.Entropy().MaxPoops)
                {
                    Poop padd = Poop.getRandomPoopForPlayer(player);
                    player.Entropy().poops.Add(padd);
                    if (Main.dedServ)
                    {
                        ModPacket mp = Mod.GetPacket();
                        mp.Write((byte)CalamityEntropy.NetPackages.PickUpPoop);
                        mp.Write(player.whoAmI);
                        mp.Write(padd.FullName);
                        mp.Send();
                    }
                }
            }
            return false;
        }
    }
}
