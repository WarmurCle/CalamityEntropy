using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Placeables.FurnitureVoid;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.NihTwins
{
    [AutoloadEquip(EquipType.Head)]
    public class ChaoticHelmet : ModItem
    {
        public static int MaxCells = 8;
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 26;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ChaoticBodyArmor>() && legs.type == ModContent.ItemType<ChaoticLeggings>();
        }


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Mod.GetLocalization("ChaoticSetBonus").Value;
            player.setBonus = player.setBonus.Replace("[KEY]", CalamityKeybinds.ArmorSetBonusHotKey.TooltipHotkeyString());
            player.setBonus = player.setBonus.Replace("[KN]", CalamityKeybinds.ArmorSetBonusHotKey.DisplayName.Value);
            player.setBonus = player.setBonus.Replace("[LIMIT]", MaxCells.ToString());
            string cnctStr = Mod.GetLocalization("NihArmorConnet").Value;
            cnctStr = cnctStr.Replace("[ANOTHERSET]", Mod.GetLocalization("VoidEaterSet").Value);
            cnctStr = cnctStr.Replace("[CONNECT]", CEKeybinds.NihilityAndChaoticArmorConnectKey.TooltipHotkeyString());
            player.setBonus += "\n" + cnctStr;
            if (!ModContent.GetInstance<Config>().MariviumArmorSetOnlyProvideStealthBarWhenHoldingRogueWeapons || player.HeldItem.DamageType.CountsAsClass(CEUtils.RogueDC))
            {
                player.Calamity().wearingRogueArmor = true;
                player.Calamity().rogueStealthMax += 1.2f;
            }
            player.endurance += 0.12f;
            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.maxMinions += 2;
            player.Entropy().ChaoticSet = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ChaoticPiece>(5)
                .AddIngredient<SmoothVoidstone>(6)
                .AddIngredient(ItemID.LunarBar, 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    public class ChaoticCellMinion : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(ModContent.GetInstance<AverageDamageClass>(), false, -1);
            Projectile.width = Projectile.height = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.timeLeft = 25 * 60;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity *= -1f;
            Projectile.netUpdate = true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.numHits);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.numHits = reader.ReadInt32();
        }
        public override void AI()
        {
            if(Projectile.numHits < 6 && Projectile.timeLeft > 6 * 60)
            {
                NPC target = Projectile.FindMinionTarget(6000);
                if (target != null)
                {
                    Projectile.velocity *= 0.97f;
                    Projectile.velocity += (target.Center - Projectile.Center).normalize() * 1;
                }
                else
                {
                    if(Projectile.Distance(Projectile.GetOwner().Center) > 120)
                    {
                        Projectile.velocity *= 0.96f;
                        Projectile.velocity += (Projectile.GetOwner().Center - Projectile.Center).normalize() * 1.2f;
                    }
                }
            }
            else
            {
                Projectile.velocity *= 0.96f;
                Projectile.velocity += (Projectile.GetOwner().Center - Projectile.Center).normalize() * 1.2f;
                if(Projectile.Distance(Projectile.GetOwner().Center) < Projectile.velocity.Length() + 32)
                {
                    Projectile.Kill();
                    int heal = (int)(1 + Projectile.GetOwner().statLifeMax2 * 0.022f);
                    Projectile.GetOwner().Heal(heal);

                    CEUtils.PlaySound("cellheal", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center, 8, 0.9f);
                    Projectile.Kill();
                }
            }
            Projectile.rotation += Projectile.velocity.X * 0.01f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }
}
