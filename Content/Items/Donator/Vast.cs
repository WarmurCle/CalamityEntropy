using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public class Vast : ModItem, IDonatorItem
    {
        public string DonatorName => "Azuta";
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
            float ManaCostDecrease = 0f;
            player.manaFlower = true;
            if (player.HasBuff(BuffID.ManaRegeneration))
            {
                player.GetCritChance(DamageClass.Magic) += 4;
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
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Diamond)
                .AddIngredient(ItemID.ManaFlower)
                .AddIngredient(5339)
                .AddCondition(Mod.GetLocalization("NearShimmer", () => "Near shimmer"), () => (Main.LocalPlayer.ZoneShimmer))
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = tooltips.Count - 1; i >= 0; i--)
            {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Text.StartsWith("#"))
                {
                    bool hide = true;
                    if (int.TryParse(tooltips[i].Text[1].ToString(), out int n))
                    {
                        if (Level() >= n)
                        { hide = false; }
                    }
                    tooltips[i].Text = tooltips[i].Text.Substring(2);
                    if (hide)
                    {
                        tooltips.RemoveAt(i);
                    }
                }
            }
        }
        public static int Level()
        {
            int l = 0;
            if (NPC.downedSlimeKing || NPC.downedBoss1 || NPC.downedBoss2 || DownedBossSystem.downedDesertScourge)
            {
                l = 1;
            }
            if (NPC.downedBoss2)
            {
                l = 2;
            }
            if (DownedBossSystem.downedSlimeGod)
            {
                l = 3;
            }
            if (DownedBossSystem.downedCryogen || DownedBossSystem.downedBrimstoneElemental)
            {
                l = 4;
            }
            if (EDownedBosses.downedProphet)
            {
                l = 5;
            }
            if (NPC.downedMoonlord)
            {
                l = 6;
            }
            if(DownedBossSystem.downedPolterghast)
            {
                l = 7;
            }
            return l;
        }
    }
    public class VastMPlayer : ModPlayer
    {
        public int ManaCostCount = 0;
        public int ExtraManaLv = 0;
        public int ExtraManaTime = 0;
        public bool BossClearFlag = false;
        public int ManaVeinLV = 0;
        public int LastMana = 0;
        public float GetEnhancedMana => 0.06f * ExtraManaLv;
        public override void PostUpdate()
        {
            if(LastMana < Player.statMana)
            {
                LastMana = Player.statMana;
            }
            if(Player.statMana < LastMana)
            {
                ManaCostCount += LastMana - Player.statMana;
                LastMana = Player.statMana;
            }
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
            if (!BossClearFlag && Main.CurrentFrameFlags.AnyActiveBossNPC && !BossRushEvent.BossRushActive)
            {
                ExtraManaTime = 0;
                Player.ClearBuff(ModContent.BuffType<ManaVein>());
            }
            BossClearFlag = Main.CurrentFrameFlags.AnyActiveBossNPC;
            if (ManaCostCount > 150 + Player.statManaMax / 10)
            {
                ExtraManaLv += (ExtraManaLv < 5 ? 1 : 0);
                ExtraManaTime = 30 * 60;
                if (ExtraManaLv == 5)
                {
                    ExtraManaTime = 60 * 60 * 5;
                }
                ManaCostCount -= 150 + Player.statManaMax / 10;
            }
            if (ManaCostCount < 0)
                ManaCostCount = 0;
            if (ExtraManaTime-- <= 0)
            {
                ExtraManaLv = 0;
            }
            if (Player.Entropy().hasAcc("VastLV3"))
            {
                Player.endurance += (Player.statManaMax2 - Player.Entropy().manaNorm) * 0.0005f;
            }
            if (Player.Entropy().hasAcc("VastLV5"))
            {
                if (player.HeldItem.mana > 0 && player.statMana < player.GetManaCost(player.HeldItem))
                {
                    /*player.AddBuff(ModContent.BuffType<ManaAwaken>(), 6 * 60);
                    player.AddBuff(BuffID.ManaSickness, 10 * 60);*/
                    
                }
                if (NPC.downedMoonlord)
                {
                    Player.GetCritChance(DamageClass.Magic) += ExtraManaLv * 2;
                    var v = Player.GetCritDamage(DamageClass.Magic);
                    v += 0.03f * ExtraManaLv;
                }
            }
            for(int i = 0; i < ExtraManaLv; i++)
            {
                if(Main.rand.NextBool())
                {
                    GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Player.Center + new Vector2(Main.rand.NextFloat(-3, 3), Player.height / 2) + CEUtils.randomVec(1), CEUtils.randomVec(1), new Color(100, 100, 255), 40, 0.16f, 1, 0.1f, true, 0, true));

                }
            }
            if(ExtraManaLv >= 5)
            {
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(CEUtils.randomPoint(Player.getRect()), Player.velocity * 0.2f + CEUtils.randomVec(1), new Color(100, 100, 255), 40, 0.2f, 1, 0.1f, true, 0, true));
            }

        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            r = float.Lerp(r, 0.5f, ExtraManaLv / 5f);
            g = float.Lerp(g, 0.5f, ExtraManaLv / 5f);
            b = float.Lerp(b, 1, ExtraManaLv / 5f);
        }
        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            if (Player.HasBuff<ManaAwaken>())
            {
                Player.HealMana(manaConsumed * 2);
            }
        }
    }
}