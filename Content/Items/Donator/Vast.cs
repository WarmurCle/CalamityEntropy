using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public class Vast : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip("Vast", !hideVisual);
            float ManaCostDecrease = 0.08f;
            player.manaFlower = true;
            if (NPC.downedSlimeKing || NPC.downedBoss1 || NPC.downedBoss2 || DownedBossSystem.downedDesertScourge)
            {
                player.Entropy().ManaExtraHeal += 0.1f;
                if (player.HasBuff(BuffID.ManaRegeneration))
                {
                    player.GetCritChance(DamageClass.Magic) += 4;
                }
            }

            player.manaCost -= ManaCostDecrease;
        }
    }
}