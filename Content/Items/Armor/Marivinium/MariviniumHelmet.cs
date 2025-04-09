using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Marivinium
{
    [AutoloadEquip(EquipType.Head)]
    public class MariviniumHelmet : ModItem
    {
        public static int ShieldCd = 12 * 60;
        public static int MaxShield = 2;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.defense = 58;
            Item.rare = ModContent.RarityType<AbyssalBlue>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MariviniumBodyArmor>() && legs.type == ModContent.ItemType<MariviniumLeggings>();
        }


        public override void UpdateArmorSet(Player player)
        {
            player.Entropy().meleeDamageReduce += 0.3f;
            player.maxMinions += 10;
            player.Entropy().damageReduce += 0.15f;
            player.GetDamage(DamageClass.Summon) += 1;
            player.whipRangeMultiplier += 0.4f;
            player.Entropy().summonCrit += 5;
            player.GetArmorPenetration(DamageClass.Generic) += 100;
            player.Entropy().MariviniumSet = true;
            if (player.HeldItem.DamageType.CountsAsClass(Util.CUtil.rogueDC))
            {
                player.Calamity().wearingRogueArmor = true;
                player.Calamity().rogueStealthMax += 1.4f;
            }
            if (player.velocity.Length() < 1)
            {
                player.lifeRegen += 45;
                player.Entropy().lifeRegenPerSec += 4;
            }
            ApplyBuffImmune(player);
            if (player.HeldItem.DamageType.CountsAsClass(ModContent.GetInstance<TrueMeleeDamageClass>()))
            {
                player.Entropy().damageReduce += 0.15f;
                player.statDefense += 25;
            }
        }
        public static void ApplyBuffImmune(Player player)
        {
            player.buffImmune[ModContent.BuffType<VulnerabilityHex>()] = true;
            player.buffImmune[ModContent.BuffType<MiracleBlight>()] = true;
            player.buffImmune[ModContent.BuffType<Dragonfire>()] = true;
            player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;
            player.buffImmune[ModContent.BuffType<VoidTouch>()] = true;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
            player.buffImmune[ModContent.BuffType<VoidVirus>()] = true;
            player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
            player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[ModContent.BuffType<Nightwither>()] = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[ModContent.BuffType<ArmorCrunch>()] = true;
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[148] = true;
            player.buffImmune[BuffID.BrokenArmor] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            if (Main.zenithWorld)
            {
                player.buffImmune[ModContent.BuffType<NOU>()] = true;
            }
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.GetCritChance(DamageClass.Generic) += 20;
            player.GetAttackSpeed(DamageClass.Melee) += 0.25f;
            player.statLifeMax2 += 300;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.Entropy().MariviniumSet)
            {
                tooltips.Add(new TooltipLine(Mod, "Armor Bonus", Mod.GetLocalization("MariviniumSet").Value));
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OmegaBlueHelmet>()
                .AddIngredient<WyrmTooth>(4)
                .AddIngredient<AscendantSpiritEssence>(2).AddTile<AbyssalAltarTile>()
                .Register();
        }
    }
}
