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
            if (NPC.downedBoss2)
            {
                player.Entropy().addEquip("VastLV2");
            }
            if (DownedBossSystem.downedSlimeGod)
            {
                player.Entropy().addEquip("VastLV3");
            }
            if (DownedBossSystem.downedCryogen || DownedBossSystem.downedBrimstoneElemental)
            {
                player.Entropy().addEquip("VastLV4");
            }
            if (EDownedBosses.downedProphet)
            {
                player.Entropy().addEquip("VastLV5");

            }
            player.manaCost -= ManaCostDecrease;
        }
    }
    public class VastMPlayer : ModPlayer
    {
        public int ManaCostCount = 0;
        public int ExtraManaLv = 0;
        public int ExtraManaTime = 0;
        public bool BossClearFlag = false;
        public int ManaVeinLV = 0;
        public float GetEnhancedMana => 6 * ExtraManaLv;
        public override void PostUpdate()
        {
            var player = Player;
            if (!Player.HasBuff<ManaVein>())
            {
                ManaVeinLV = 0;
            }
            if (!Player.Entropy().hasAcc("VastLV3"))
            {
                ExtraManaLv = 0;
                ExtraManaTime = 0;
                return;
            }
            if (!BossClearFlag && Main.CurrentFrameFlags.AnyActiveBossNPC)
            {
                ExtraManaTime = 0;
                Player.ClearBuff(ModContent.BuffType<ManaVein>());
            }
            BossClearFlag = Main.CurrentFrameFlags.AnyActiveBossNPC;
            if (ManaCostCount > Player.Entropy().manaNorm / 2)
            {
                ExtraManaLv += (ExtraManaLv < 5 ? 1 : 0);
                ExtraManaTime = 12 * 60;
                if (ExtraManaLv == 5)
                {
                    ExtraManaTime = 60 * 60 * 5;
                }
                ManaCostCount -= Player.Entropy().manaNorm / 2;
            }
            if (ExtraManaTime-- <= 0)
            {
                ExtraManaLv = 0;
            }
            if (Player.Entropy().hasAcc("VastLV4"))
            {
                Player.endurance += Player.statManaMax2 - Player.Entropy().manaNorm * 0.005f;
            }
            if (Player.Entropy().hasAcc("VastLV5"))
            {
                if (player.HeldItem.mana > 0 && player.statMana < player.GetManaCost(player.HeldItem))
                {
                    player.AddBuff(ModContent.BuffType<ManaAwaken>(), 6 * 60);
                    player.AddBuff(BuffID.ManaSickness, 10 * 60);
                    if (NPC.downedMoonlord)
                    {
                        if (ExtraManaLv == 5)
                        {
                            ManaVeinLV += (ManaVeinLV < 5 ? 1 : 0);
                            Player.AddBuff(ModContent.BuffType<ManaVein>(), ManaVeinLV == 5 ? 10 * 60 * 60 : 8 * 60);
                            if (ManaVeinLV == 5)
                            {
                                ExtraManaTime = 10 * 60 * 60;
                            }
                        }
                    }
                }
            }
            Player.GetCritChance(DamageClass.Magic) += ManaVeinLV * 2;
            var v = Player.GetCritDamage(DamageClass.Magic);
            v += 0.03f * ManaVeinLV;
        }
        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            ManaCostCount++;
            if (Player.HasBuff<ManaAwaken>())
            {
                Player.HealMana(manaConsumed * 2);
            }
        }
    }
}