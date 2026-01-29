using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class SusiesBracelet : ModItem, IGetFromStarterBag
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += AddDef;
            player.GetDamage(DamageClass.Generic) += AddDamage;
            player.statLifeMax2 += AddHP;
            player.statManaMax2 += AddMana;
        }

        public int Level = 0;
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Level);
        }
        public override void NetReceive(BinaryReader reader)
        {
            Level = reader.ReadInt32();
        }
        public override void SaveData(TagCompound tag)
        {
            if (Level > 0)
                tag["Level"] = Level;
        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<int>("Level", out int lv))
            {
                Level = lv;
            }
        }
        public int GetLevel()
        {
            CheckUpdate();
            return Level;
        }

        public bool OwnAble(Player player, ref int count)
        {
            return ShadowCrystalDeltarune.Ch4Crystal;
        }

        public void CheckUpdate()
        {
            void Check(bool f, int lv)
            {
                if (lv > Level && f)
                {
                    Level = lv;
                }
            }
            Check(NPC.downedBoss1 || NPC.downedSlimeKing, 1);
            Check(NPC.downedBoss2 || DownedBossSystem.downedPerforator || DownedBossSystem.downedHiveMind, 2);
            Check(NPC.downedBoss3 || NPC.downedQueenBee, 3);
            Check(Main.hardMode, 4);
            Check(NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, 5);
            Check(NPC.downedPlantBoss || DownedBossSystem.downedCalamitasClone, 6);
            Check(NPC.downedMoonlord, 7);
            Check(DownedBossSystem.downedProvidence, 8);
            Check(DownedBossSystem.downedPolterghast || DownedBossSystem.downedStormWeaver || DownedBossSystem.downedCeaselessVoid || DownedBossSystem.downedSignus, 9);
            Check(DownedBossSystem.downedDoG, 10);
            Check(DownedBossSystem.downedYharon, 11);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = 0;
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Name.Contains("Tooltip"))
                {
                    index = i;
                }
            }
            index++;
            if (GetLevel() < 11)
            {
                tooltips.Add(new TooltipLine(Mod, $"Tooltip{index}", GetLt($"Trial", "Trials").Value + $"{GetLevel() + 1} - " + GetLt($"t{GetLevel()}", "Trials").Value) { OverrideColor = Color.Yellow });
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, $"Tooltip{index}", GetLt("t11", "Trials").Value) { OverrideColor = Color.Yellow });
            }

            tooltips.Add(new TooltipLine(Mod, $"Tooltip{index}", GetLt($"l{GetLevel()}").Value) { OverrideColor = Color.Pink });

            tooltips.Replace("[DMG]", AddDamage.ToPercent().ToString());
            tooltips.Replace("[LIFE]", AddHP.ToString());
            tooltips.Replace("[MANA]", AddMana.ToString());
            tooltips.Replace("[DEF]", AddDef.ToString());
        }
        public float AddDamage => GetLevel() switch
        {
            0 => 0.02f,
            1 => 0.04f,
            2 => 0.06f,
            3 => 0.08f,
            4 => 0.1f,
            5 => 0.12f,
            6 => 0.14f,
            7 => 0.16f,
            8 => 0.18f,
            9 => 0.2f,
            10 => 0.22f,
            11 => 0.24f,
            _ => 0.24f
        };
        public int AddHP => GetLevel() switch
        {
            0 => 10,
            1 => 15,
            2 => 20,
            3 => 25,
            4 => 30,
            5 => 40,
            6 => 50,
            7 => 60,
            8 => 70,
            9 => 80,
            10 => 90,
            11 => 100,
            _ => 100
        };
        public int AddMana => GetLevel() switch
        {
            0 => 30,
            1 => 40,
            2 => 60,
            3 => 80,
            4 => 90,
            5 => 100,
            6 => 110,
            7 => 120,
            8 => 130,
            9 => 140,
            10 => 150,
            11 => 160,
            _ => 160
        };
        public int AddDef => GetLevel() switch
        {
            0 => 2,
            1 => 4,
            2 => 6,
            3 => 8,
            4 => 10,
            5 => 12,
            6 => 14,
            7 => 16,
            8 => 18,
            9 => 20,
            10 => 22,
            11 => 25,
            _ => 25
        };
        public static LocalizedText GetLt(string n, string h = "Lores")
        {
            return Language.GetText($"Mods.CalamityEntropy.LegendaryAbility.SusiesBracelet.{h}.{n}");
        }
    }
}
