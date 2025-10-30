using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class OdinsRefuge : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 86;
            Item.height = 86;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
            Item.defense = 30;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().holyMantle = true;
            player.Calamity().DashID = AsgardianAegisDash.ID;
            player.dashType = 0;
            player.noKnockback = true;
            player.fireWalk = true;
            player.buffImmune[33] = true;
            player.buffImmune[36] = true;
            player.buffImmune[30] = true;
            player.buffImmune[20] = true;
            player.buffImmune[32] = true;
            player.buffImmune[31] = true;
            player.buffImmune[35] = true;
            player.buffImmune[23] = true;
            player.buffImmune[22] = true;
            player.buffImmune[194] = true;
            player.buffImmune[156] = true;
            player.buffImmune[ModContent.BuffType<ArmorCrunch>()] = true;
            player.buffImmune[ModContent.BuffType<BrainRot>()] = true;
            player.buffImmune[ModContent.BuffType<BurningBlood>()] = true;
            player.buffImmune[70] = true;
            player.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = true;
            player.buffImmune[149] = true;
            player.buffImmune[80] = true;
            player.buffImmune[24] = true;
            player.buffImmune[323] = true;
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            player.buffImmune[46] = true;
            player.buffImmune[47] = true;
            player.buffImmune[44] = true;
            player.buffImmune[324] = true;
            player.buffImmune[39] = true;
            player.buffImmune[153] = true;
            player.buffImmune[189] = true;
            player.buffImmune[ModContent.BuffType<Nightwither>()] = true;
            player.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;

            CalamityPlayer calamityPlayer = player.Calamity();
            player.longInvince = true;
            calamityPlayer.dAmulet = true;
            calamityPlayer.rampartOfDeities = true;
            if ((double)player.statLife <= (double)player.statLifeMax2 * 0.5)
            {
                player.AddBuff(62, 5);
            }

            player.noKnockback = true;
            if (!((float)player.statLife > (float)player.statLifeMax2 * 0.25f))
            {
                return;
            }

            player.hasPaladinShield = true;
            if (player.whoAmI == Main.myPlayer || player.miscCounter % 10 != 0)
            {
                return;
            }

            int myPlayer = Main.myPlayer;
            if (Main.player[myPlayer].team == player.team && player.team != 0)
            {
                float num = player.position.X - Main.player[myPlayer].position.X;
                float num2 = player.position.Y - Main.player[myPlayer].position.Y;
                if ((float)Math.Sqrt(num * num + num2 * num2) < 800f)
                {
                    Main.player[myPlayer].AddBuff(43, 20);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<AsgardianAegis>(), 1).
                AddIngredient(ModContent.ItemType<RampartofDeities>(), 1).
                AddIngredient(ModContent.ItemType<HolyMantle>(), 1).
                AddIngredient(ModContent.ItemType<VoidBar>(), 10).
                AddTile(ModContent.TileType<VoidWellTile>()).
                Register();
        }
    }
}
