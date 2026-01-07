using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.LoreItems;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LEWof : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreWallofFlesh>();
        public static int Cooldown = 20;
        public static float DmgReduce = 0.05f;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DmgReduce.ToPercent().ToString());
            tooltip.Text = tooltip.Text.Replace("{2}", Cooldown.ToString());
        }
    }
    public class LEQueenSlime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreQueenSlime>();
        public static float FallingSpeedAdd = 0.05f;
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", FallingSpeedAdd.ToPercent().ToString());
        }
        public override void UpdateEffects(Player player)
        {
            player.Entropy().FallSpeed += FallingSpeedAdd;
        }
    }
    public class LECryo : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreArchmage>();
        public override void UpdateEffects(Player player)
        {
            for (int b = 0; b < Player.MaxBuffs; b++)
            {
                if (player.buffType[b] == BuffID.Chilled || player.buffType[b] == BuffID.Frozen || player.buffType[b] == ModContent.BuffType<GlacialState>())
                {
                    if (player.buffTime[b] > 2 && Main.GameUpdateCount % 2 == 0)
                    {
                        player.buffTime[b] -= 1;
                    }
                }
            }
        }
    }
    public class LEMech : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreMechs>();
        public static int DEF = 1;
        public override void UpdateEffects(Player player)
        {
            player.statDefense += DEF;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DEF.ToString());
        }
    }
    public class LETwin : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreTwins>();
        public static int DEF = 1;
        public override void UpdateEffects(Player player)
        {
            player.statDefense += DEF;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DEF.ToString());
        }
    }
    public class LEDestroyer : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreDestroyer>();
        public static int DEF = 1;
        public override void UpdateEffects(Player player)
        {
            player.statDefense += DEF;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DEF.ToString());
        }
    }
    public class LESkePrime : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreSkeletronPrime>();
        public static float DR = 0.005f;
        public override void UpdateEffects(Player player)
        {
            player.endurance += DR;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DR.ToPercent().ToString());
        }
    }
    public class LESulphurSea : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreSulphurSea>();
        public override void UpdateEffects(Player player)
        {
            player.buffImmune[BuffID.Venom] = true;
        }
    }
    public class LEAzafure : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreAzafure>();
        public static int Crit = 1;
        public override void UpdateEffects(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += Crit;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", Crit.ToString());
        }
    }
    public class LECalClone : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreCalamitasClone>();
        public static float Ats = 0.02f;
        public override void UpdateEffects(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) += Ats;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", Ats.ToPercent().ToString());
        }
    }
    public class LEPlantera : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LorePlantera>();
        public static int Regen = 1;
        public override void UpdateEffects(Player player)
        {
            player.lifeRegen += Regen;
        }
    }
    public class LEAbyss : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreAbyss>();
        public override void UpdateEffects(Player player)
        {
            player.breathMax += 60;
        }
    }
    public class LELeviathan : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreLeviathanAnahita>();
        public static int Regen = 2;
        public override void UpdateEffects(Player player)
        {
            if (player.wet)
                player.lifeRegen += Regen;
        }
    }
    public class LEAstrumAureus : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreAstrumAureus>();
        public override void UpdateEffects(Player player)
        {
            for (int b = 0; b < Player.MaxBuffs; b++)
            {
                if (player.buffType[b] == ModContent.BuffType<AstralInjectionBuff>())
                {
                    if (player.buffTime[b] > 2 && Main.GameUpdateCount % 2 == 0)
                    {
                        player.buffTime[b] -= 1;
                    }
                }
            }
        }
    }
    public class LEGolem : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreGolem>();
        public static float DR = 0.005f;
        public override void UpdateEffects(Player player)
        {
            player.endurance += DR;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DR.ToPercent().ToString());
        }
    }
    public class LEPlagueBee : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LorePlaguebringerGoliath>();
        public override void UpdateEffects(Player player)
        {
            for (int b = 0; b < Player.MaxBuffs; b++)
            {
                if (player.buffType[b] == ModContent.BuffType<Plague>())
                {
                    if (player.buffTime[b] > 2 && Main.GameUpdateCount % 2 == 0)
                    {
                        player.buffTime[b] -= 2;
                    }
                }
            }
        }
    }
    public class LELightEmpress : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreEmpressofLight>();
        public static int DEF = 2;
        public override void UpdateEffects(Player player)
        {
            if (Main.dayTime)
                player.statDefense += DEF;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", DEF.ToString());
        }
    }
    public class LEFishron : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreDukeFishron>();
        public override void UpdateEffects(Player player)
        {
            player.ignoreWater = true;
        }
    }
    public class LERavager : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreRavager>();
        public override void UpdateEffects(Player player)
        {
            player.Entropy().moveSpeed += 0.02f;
        }
    }
    public class LEPrelude : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LorePrelude>();
        public override void UpdateEffects(Player player)
        {
            player.Entropy().light += 0.05f;
        }
    }
    public class LEAstral : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreAstralInfection>();
        public static float WingTime = 0.05f;
        public override void UpdateEffects(Player player)
        {
            player.Entropy().WingTimeMult += WingTime;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", WingTime.ToPercent().ToString());
        }
    }
    public class LEDeus : LoreEffect
    {
        public override int ItemType => ModContent.ItemType<LoreAstrumDeus>();
        public static float WingSpeed = 0.03f;
        public override void UpdateEffects(Player player)
        {
            player.Entropy().WingSpeed += WingSpeed;
        }
        public override void ModifyTooltip(TooltipLine tooltip)
        {
            tooltip.Text = tooltip.Text.Replace("{1}", WingSpeed.ToPercent().ToString());
        }
    }
}