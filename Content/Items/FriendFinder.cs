using CalamityEntropy.Content.NPCs.FriendFinderNPC;
using CalamityMod;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class FriendFinder : ModItem
    {
        public static List<int> summonList;
        public static int CooldownSec = CalamityUtils.SecondsToFrames(20);
        public override void SetStaticDefaults()
        {
            summonList = new List<int>() { ModContent.NPCType<AeroSlimeFriendly>(), ModContent.NPCType<DespairStoneFriendly>(), ModContent.NPCType<IceClasperFriendly>(), ModContent.NPCType<ScryllarFriendly>(), ModContent.NPCType<SkyfinFriendly>(), ModContent.NPCType<SoulSlurperFriendly>() };
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 70;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.scale = 0.6f;
        }
        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return !player.HasCooldown("FriendfinderCd");
            float slots = 0;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.ModNPC is FriendFindNPC)
                {
                    slots += 1;
                }
            }
            if (slots >= player.maxMinions)
            {
                return false;
            }
            return !(player.Entropy().ffinderCd > 0);
        }

        public override bool? UseItem(Player player)
        {
            if(!Main.dedServ)
                player.AddCooldown("FriendfinderCd", CooldownSec);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return true;
            }
            int n = NPC.NewNPC(player.GetSource_FromAI(), (int)player.position.X, (int)player.position.Y, summonList[Main.rand.Next(0, summonList.Count)]);
            n.ToNPC().localAI[3] = player.whoAmI + 1;
            n.ToNPC().Center = player.Center - new Vector2(0, 60);
            if (Main.dedServ)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
            }
            player.Entropy().ffinderCd = (int)(CooldownSec * player.Entropy().CooldownTimeMult);
            
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.LifeCrystal, 5).AddIngredient(ItemID.GoldBar, 6).AddIngredient(ItemID.Ruby, 4).AddTile(TileID.Anvils).Register();
        }
    }
}
