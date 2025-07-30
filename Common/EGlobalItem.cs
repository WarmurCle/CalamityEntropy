using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Accessories.Cards;
using CalamityEntropy.Content.Items.Accessories.SoulCards;
using CalamityEntropy.Content.Items.Armor.VoidFaquir;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.PrefixItem;
using CalamityEntropy.Content.Items.Vanity;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Items.Weapons.CrystalBalls;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityMod;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Common
{
    public class S3Particle
    {
        public Vector2 velocity = Vector2.Zero;
        public Vector2 position;
        public void update()
        {
            this.position += this.velocity;
        }
        
        public void draw(float alpha, Vector2 offset, Color color)
        {
            SpriteBatch sb = Main.spriteBatch;
            Color b = color * alpha;
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/style3").Value;
            sb.Draw(tx, this.position + offset, null, b, this.velocity.ToRotation(), new Vector2(tx.Width, tx.Height) / 2, 0.3f, SpriteEffects.None, 0);

        }
    }


    public class EGlobalItem : GlobalItem
    {
        public bool Legend = false;
        public int tooltipStyle = 0;
        public bool stroke = false;
        public Color strokeColor = Color.White;
        public Color NameColor = Color.White;
        public bool HasCustomNameColor = false;
        public bool HasCustomStrokeColor = false;
        public List<S3Particle> particles1 = new List<S3Particle>();
        public float[] wispColor = null;
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ModContent.ItemType<StarblightSoot>())
            {
                entity.ammo = 3728;
            }
            if (entity.type == 1325)
            {
                entity.damage = 32;
                entity.shootSpeed *= 1.25f;
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.wingSlot != -1)
            {
                player.Entropy().wing = item;
            }
        }


        public override bool CanRightClick(Item item)
        {
            return (CEUtils.IsArmor(item) && Main.mouseItem.IsArmorReforgeItem(out var _)) || (BookMarkLoader.IsABookMark(item) && EBookUI.active && BookMarkLoader.HasEmptyBookMarkSlot(EBookUI.bookItem, Main.LocalPlayer));
        }
        public override void RightClick(Item item, Player player)
        {
            if (BookMarkLoader.IsABookMark(item) && EBookUI.active)
            {
                bool flag = true;
                for (int h = 0; h < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, EBookUI.bookItem), Main.LocalPlayer.Entropy().EBookStackItems.Count); h++)
                {
                    if (BookMarkLoader.IsABookMark(Main.LocalPlayer.Entropy().EBookStackItems[h]))
                    {
                        var bm = Main.LocalPlayer.Entropy().EBookStackItems[h];
                        var mi = (BookMark)bm.ModItem;
                        if (!BookMarkLoader.CanBeEquipWith(item, bm))
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                if (flag)
                {
                    for (int i = 0; i < player.Entropy().EBookStackItems.Count; i++)
                    {
                        if (player.Entropy().EBookStackItems[i].IsAir)
                        {
                            player.Entropy().EBookStackItems[i] = item.Clone();
                            item.TurnToAir();
                            PlayerLoader.SyncPlayer(Main.LocalPlayer, -1, Main.myPlayer, false);
                        }
                    }
                }
            }
            Item held = Main.mouseItem;
            if (CEUtils.IsArmor(item))
            {
                if (held.IsArmorReforgeItem(out var p))
                {
                    if (p == null)
                    {
                        for (int i = 0; i < ItemLoader.ItemCount; i++)
                        {
                            var ins = ItemLoader.GetItem(i);
                            if (ins is BasePrefixItem pi && pi.PrefixName == armorPrefixName)
                            {
                                player.QuickSpawnItem(player.GetSource_FromThis(), new Item(ins.Type), 1);
                                break;
                            }
                        }
                    }
                    item.Entropy().SetArmorPrefix(p);
                    SoundStyle s = new SoundStyle("CalamityEntropy/Assets/Sounds/Reforge");
                    SoundEngine.PlaySound(s);
                }
            }
        }

        public override void GetHealMana(Item item, Player player, bool quickHeal, ref int healValue)
        {
            healValue += (int)(healValue * player.Entropy().ManaExtraHeal);
            if(player.Entropy().hasAcc("VastLV2"))
            {
                healValue = (int)((CalCI ? 0.25f : 0.75f) * healValue);
            }
        }

        public static bool CalCI = false;
        public override bool ConsumeItem(Item item, Player player)
        {
            
            if (player.Entropy().hasAcc("VastLV2") && item.healMana > 0)
            {
                CalCI = true;
                int h = item.healMana;
                ItemLoader.GetHealMana(item, player, true, ref h);
                CalCI = false;
                player.Entropy().ManaRegenPer30Tick = h / 10;
                player.Entropy().ManaRegenTime = 60 * 5 + 5;
            }
            if (BookMarkLoader.IsABookMark(item) && EBookUI.active)
            {
                return false;
            }
            Item held = Main.mouseItem;
            if (CEUtils.IsArmor(item))
            {
                if (held.IsArmorReforgeItem(out var _))
                {
                    if (ItemLoader.ConsumeItem(held, player))
                    {
                        held.Shrink();
                    }
                    return false;
                }
            }
            return true;
        }
        public void SetArmorPrefix(ArmorPrefix armorPrefixS)
        {
            if (armorPrefixS == null)
            {
                this.armorPrefix = null;
                this.armorPrefixName = string.Empty;
                return;
            }
            this.armorPrefix = armorPrefixS;
            this.armorPrefixName = armorPrefixS.RegisterName();
        }
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            speed *= player.Entropy().WingSpeed;
            acceleration *= player.Entropy().WingSpeed;
            speed *= 1 + player.Entropy().VoidCharge * 0.25f;
            acceleration *= 1 + player.Entropy().VoidCharge * 0.25f;

        }
        public override void UpdateEquip(Item item, Player player)
        {
            if (item.type == ItemID.SantaHat)
            {
                player.Entropy().cHat = true;
            }
            if (armorPrefix != null)
            {
                armorPrefix.UpdateEquip(player, item);
                player.statDefense += (int)(Math.Round(item.defense * armorPrefix.AddDefense()));
            }
        }
        public override void UpdateVanity(Item item, Player player)
        {
            if (item.wingSlot != -1)
            {
                player.Entropy().vanityWing = item;
            }
            if (item.type == ItemID.SantaHat)
            {
                player.Entropy().cHat = true;
            }
        }

        public override bool? UseItem(Item item, Player player)
        {
            /*if (item.type == ItemID.RodOfHarmony)
            {
                if (NPC.AnyNPCs(ModContent.NPCType<AbyssalWraith>()))
                {
                    SubworldSystem.Enter<VOIDSubworld>();
                }
            }*/
            if (player.channel || player.whoAmI != Main.myPlayer || item.pick > 0 || item.damage <= 0 || item.ammo != AmmoID.None || item.axe > 0 || !player.Entropy().TarnishCard)
            {
                return null;
            }
            var mp = player.Entropy();
            if (mp.BlackFlameCd <= 0 && player.whoAmI == Main.myPlayer)
            {
                mp.BlackFlameCd = item.useTime - 2;
                Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One) * 14, ModContent.ProjectileType<BlackFire>(), player.GetWeaponDamage(item) / 8 + 1, 2, player.whoAmI);
            }
            return null;
        }
        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling *= 1 + player.Entropy().VoidCharge * 0.5f;
            ascentWhenRising *= 1 + player.Entropy().VoidCharge * 0.5f;
            maxAscentMultiplier *= 1 + player.Entropy().VoidCharge * 0.5f;
            maxCanAscendMultiplier *= 1 + player.Entropy().VoidCharge * 0.5f;
            constantAscend *= 1 + player.Entropy().VoidCharge * 0.5f;
            ascentWhenFalling *= player.Entropy().WingSpeed;
            ascentWhenRising *= player.Entropy().WingSpeed;
            maxAscentMultiplier *= player.Entropy().WingSpeed;
            maxCanAscendMultiplier *= player.Entropy().WingSpeed;
            constantAscend *= player.Entropy().WingSpeed;

        }

        public override bool InstancePerEntity => true;
        public string armorPrefixName = string.Empty;
        public ArmorPrefix armorPrefix = null;
        public override void SaveData(Item item, TagCompound tag)
        {
            tag.Add("ArmorPrefix", armorPrefixName);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("ArmorPrefix"))
            {
                armorPrefixName = tag.Get<string>("ArmorPrefix");
                ArmorPrefix result = ArmorPrefix.findByName(armorPrefixName);
                armorPrefix = result;
            }
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(armorPrefixName);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            armorPrefixName = reader.ReadString();
            armorPrefix = ArmorPrefix.findByName(armorPrefixName);
        }

        public string getAmmoName(int type)
        {
            if (type == AmmoID.Solution)
            {
                return Mod.GetLocalization("AmmoSolution").Value;
            }
            if (type == AmmoID.Arrow)
            {
                return Mod.GetLocalization("AmmoArrow").Value;
            }
            if (type == AmmoID.Bullet)
            {
                return Mod.GetLocalization("AmmoBullet").Value;
            }
            if (type == AmmoID.CandyCorn)
            {
                return Mod.GetLocalization("AmmoCandyCorn").Value;
            }
            if (type == AmmoID.Coin)
            {
                return Mod.GetLocalization("AmmoCoin").Value;
            }
            if (type == AmmoID.Dart)
            {
                return Mod.GetLocalization("AmmoDart").Value;
            }
            if (type == AmmoID.FallenStar)
            {
                return Mod.GetLocalization("AmmoFallenStar").Value;
            }
            if (type == AmmoID.Flare)
            {
                return Mod.GetLocalization("AmmoFlare").Value;
            }
            if (type == AmmoID.Gel)
            {
                return Mod.GetLocalization("AmmoGel").Value;
            }
            if (type == AmmoID.JackOLantern)
            {
                return Mod.GetLocalization("AmmoJackOLantern").Value;
            }
            if (type == AmmoID.NailFriendly)
            {
                return Mod.GetLocalization("AmmoNail").Value;
            }
            if (type == AmmoID.Rocket)
            {
                return Mod.GetLocalization("AmmoRocket").Value;
            }
            if (type == AmmoID.Sand)
            {
                return Mod.GetLocalization("AmmoSand").Value;
            }
            if (type == AmmoID.Snowball)
            {
                return Mod.GetLocalization("AmmoSnowball").Value;
            }
            if (type == AmmoID.Stake)
            {
                return Mod.GetLocalization("AmmoStake").Value;
            }
            if (type == AmmoID.StyngerBolt)
            {
                return Mod.GetLocalization("AmmoStyngerBolt").Value;
            }
            if (ModLoader.HasMod("MoreBoulders") && type == 540)
            {
                return Mod.GetLocalization("AmmoBoulders").Value;
            }
            if (type == 3728)
            {
                return Mod.GetLocalization("AmmoStarblightSoot").Value;
            }
            if (type == 6259 || type == 8584)
            {
                return CalamityUtils.GetItemName<WulfrumMetalScrap>().Value;
            }
            return type.ToString();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ModContent.GetInstance<Config>().ItemAdditionalInfo)
            {
                if (item.ammo != AmmoID.None)
                {
                    tooltips.Add(new TooltipLine(Mod, "Ammo Type", Mod.GetLocalization("AmmoType").Value + ": " + getAmmoName(item.ammo)));
                    if (item.shoot > ProjectileID.None)
                    {
                        tooltips.Add(new TooltipLine(Mod, "Ammo Life Time", Mod.GetLocalization("AmmoLifeTime").Value + ": " + Math.Round((CalamityEntropy.GetAProjectileInstance(item.shoot).timeLeft / (float)CalamityEntropy.GetAProjectileInstance(item.shoot).MaxUpdates) / 60f, 2).ToString() + "s"));
                        tooltips.Add(new TooltipLine(Mod, "Ammo Shoot Speed", Mod.GetLocalization("AmmoShootSpeed").Value + ": " + ((item.shootSpeed * (float)CalamityEntropy.GetAProjectileInstance(item.shoot).MaxUpdates)).ToString()));
                        tooltips.Add(new TooltipLine(Mod, "Ammo Penetrate", Mod.GetLocalization("AmmoPenetrate").Value + ": " + ((CalamityEntropy.GetAProjectileInstance(item.shoot).penetrate) >= 0 ? (CalamityEntropy.GetAProjectileInstance(item.shoot).penetrate - 1).ToString() : Mod.GetLocalization("AmmoPenetrateInfinite").Value)));
                        if (CalamityEntropy.GetAProjectileInstance(item.shoot).ArmorPenetration > 0)
                        {
                            tooltips.Add(new TooltipLine(Mod, "Ammo Armor Penetration", Mod.GetLocalization("ArmorPenetrationItemTooltip").Value + ": " + (CalamityEntropy.GetAProjectileInstance(item.shoot).ArmorPenetration).ToString()));
                        }
                    }
                }
                if (item.useAmmo != AmmoID.None)
                {
                    tooltips.Add(new TooltipLine(Mod, "Use Ammo", Mod.GetLocalization("UseAmmo").Value + ": " + getAmmoName(item.useAmmo)));
                }
                for (int i = 0; i < tooltips.Count; i++)
                {
                    if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Knockback")
                    {
                        if (item.damage > 0 && item.ArmorPenetration > 0)
                        {
                            tooltips.Insert(i + 1, new TooltipLine(Mod, "Armor Penetration", item.ArmorPenetration.ToString() + " " + Mod.GetLocalization("ArmorPenetrationItemTooltip").Value));
                        }
                    }
                }
            }

            if (item.Entropy().armorPrefix != null)
            {
                foreach (var tooltip in tooltips)
                {
                    if (tooltip.Mod == "Terraria")
                    {
                        if (tooltip.Name == "ItemName")
                        {
                            tooltip.Text = item.Entropy().armorPrefix.getName() + " " + tooltip.Text;
                        }
                        if (tooltip.Name == "Defense" && armorPrefix.AddDefense() != 0)
                        {
                            tooltip.Text += (armorPrefix.AddDefense() > 0 ? "(+" : "(") + ((int)Math.Round(armorPrefix.AddDefense() * item.defense)).ToString() + ")";
                        }
                    }
                }
            }
            if (item.type == ModContent.ItemType<VoidFaquirBodyArmor>() || item.type == ModContent.ItemType<VoidFaquirCuises>() || item.type == ModContent.ItemType<VoidFaquirCosmosHood>() || item.type == ModContent.ItemType<VoidFaquirDevourerHelm>() || item.type == ModContent.ItemType<VoidFaquirEvokerHelm>() || item.type == ModContent.ItemType<VoidFaquirLurkerMask>() || item.type == ModContent.ItemType<VoidFaquirShadowHelm>())
            {
                if (Main.LocalPlayer.Entropy().VFSet)
                {
                    TooltipLine t = new TooltipLine(CalamityEntropy.Instance, "Armor Bonus", Language.GetOrRegister("Mods.CalamityEntropy.vfb").Value);
                    tooltips.Add(t);
                }
                if (Main.LocalPlayer.Entropy().VFHelmMagic)
                {
                    TooltipLine t = new TooltipLine(CalamityEntropy.Instance, "Armor Bonus", Language.GetOrRegister("Mods.CalamityEntropy.helmvfc").Value);
                    tooltips.Add(t);
                }
                if (Main.LocalPlayer.Entropy().VFHelmMelee)
                {
                    TooltipLine t = new TooltipLine(CalamityEntropy.Instance, "Armor Bonus", Language.GetOrRegister("Mods.CalamityEntropy.helmvfd").Value);
                    tooltips.Add(t);
                }
                if (Main.LocalPlayer.Entropy().VFHelmRanged)
                {
                    TooltipLine t = new TooltipLine(CalamityEntropy.Instance, "Armor Bonus", Language.GetOrRegister("Mods.CalamityEntropy.helmvfs").Value);
                    tooltips.Add(t);
                }
                if (Main.LocalPlayer.Entropy().VFHelmRogue)
                {
                    TooltipLine t = new TooltipLine(CalamityEntropy.Instance, "Armor Bonus", Language.GetOrRegister("Mods.CalamityEntropy.helmvfl").Value);
                    tooltips.Add(t);
                }
                if (Main.LocalPlayer.Entropy().VFHelmSummoner)
                {
                    TooltipLine t = new TooltipLine(CalamityEntropy.Instance, "Armor Bonus", Language.GetOrRegister("Mods.CalamityEntropy.helmvfe").Value);
                    tooltips.Add(t);
                }
            }
            if (armorPrefix != null)
            {
                tooltips.Add(armorPrefix.getDescTooltipLine());
            }
            if (item.Entropy().Legend)
            {
                TooltipLine tl = new TooltipLine(CalamityEntropy.Instance, "LegendItem", Language.GetTextValue("Mods.CalamityEntropy.LegendTooltip"));
                tl.OverrideColor = new Microsoft.Xna.Framework.Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                tooltips.Add(tl);
            }
        }

        public override GlobalItem Clone(Item from, Item to)
        {
            EGlobalItem obj = (EGlobalItem)base.Clone(from, to);
            obj.Legend = Legend;
            obj.tooltipStyle = tooltipStyle;
            obj.stroke = stroke;
            obj.strokeColor = strokeColor;
            obj.NameColor = NameColor;
            obj.HasCustomNameColor = HasCustomNameColor;
            obj.HasCustomStrokeColor = HasCustomStrokeColor;
            obj.armorPrefix = armorPrefix;
            obj.armorPrefixName = armorPrefixName;
            return obj;
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<CalamityMod.Items.Placeables.FurnitureAuric.AuricToilet>())
            {
                Item ai = new Item(ModContent.ItemType<AuricToilet>(), item.stack, 0);
                item.stack = 0;
                for (int i = 0; i < player.inventory.Count(); i++)
                {
                    if (player.inventory[i] == item)
                    {
                        player.inventory[i] = ai;
                        break;
                    }
                }

            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if ((player.HasBuff<VoidVirus>() || (CalamityEntropy.EntropyMode && player.Entropy().HitTCounter > 0)) && item.healLife > 0)
            {
                return false;
            }
            if (player.HasBuff(ModContent.BuffType<StealthState>()) || player.Entropy().DarkArtsTarget.Count > 0 || player.Entropy().noItemTime > 0)
            {
                return false;
            }
            return base.CanUseItem(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Entropy().shadowPact && item.DamageType.CountsAsClass<ThrowingDamageClass>())
            {
                if (player.Entropy().shadowStealth >= 1)
                {
                    CEUtils.PlaySound("shadowKnife");
                    player.Entropy().shadowStealth = 0; Projectile.NewProjectile(source, position, velocity.normalize() * 12, ModContent.ProjectileType<ShadowShoot>(), (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(ShadowPact.BaseDamage), 2, player.whoAmI);
                }
            }
            if (player.Entropy().worshipRelic && item.DamageType.CountsAsClass<ThrowingDamageClass>() && player.Calamity().StealthStrikeAvailable())
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SolarArrowSpawner>(), (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(WorshipRelic.ArrowDamage), 2, player.whoAmI);
                player.Entropy().ResetStealth = true;
            }
            if(player.Entropy().GaleWristbladeCharge >= 5)
            {
                player.Entropy().GaleWristbladeCharge = 0;
                Projectile.NewProjectile(source, position, velocity.normalize() * 8, ModContent.ProjectileType<WristTornado>(), (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(GaleWristblades.BaseDamage), 2, player.whoAmI);
            }
            if(type == ModContent.ProjectileType<RockBulletShot>())
            {
                if (Main.rand.NextBool(6))
                {
                    CEUtils.PlaySound("gunshot_small" + Main.rand.Next(1, 4).ToString(), 1, position);
                    return false;
                }
            }
            if (!Main.dedServ)
            {
                if (item.DamageType != DamageClass.Summon)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        if (player.Entropy().WeaponBoost > 0)
                        {
                            if (item.type == ModContent.ItemType<LunarKunai>())
                            {
                                float shootSpeed = item.shootSpeed;
                                Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
                                float num = (float)Main.mouseX + Main.screenPosition.X - vector.X;
                                float num2 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
                                if (player.gravDir == -1f)
                                {
                                    num2 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
                                }

                                float num3 = (float)Math.Sqrt(num * num + num2 * num2);
                                if ((float.IsNaN(num) && float.IsNaN(num2)) || (num == 0f && num2 == 0f))
                                {
                                    num = player.direction;
                                    num2 = 0f;
                                    num3 = shootSpeed;
                                }
                                else
                                {
                                    num3 = shootSpeed / num3;
                                }

                                num *= num3;
                                num2 *= num3;
                                int num4 = (player.Calamity().StealthStrikeAvailable() ? 9 + 3 * player.Entropy().WeaponBoost : 3 + player.Entropy().WeaponBoost);
                                for (int i = 0; i < num4; i++)
                                {
                                    float num5 = num;
                                    float num6 = num2;
                                    float num7 = 0.05f * (float)i;
                                    num5 += (float)Main.rand.Next(-35, 36) * num7;
                                    num6 += (float)Main.rand.Next(-35, 36) * num7;
                                    num3 = (float)Math.Sqrt(num5 * num5 + num6 * num6);
                                    num3 = shootSpeed / num3;
                                    num5 *= num3;
                                    num6 *= num3;
                                    float x = vector.X;
                                    float y = vector.Y;
                                    int num8 = Projectile.NewProjectile(source, x, y, num5, num6, ModContent.ProjectileType<LunarKunaiProj>(), damage, knockback, player.whoAmI);
                                    if (num8.WithinBounds(Main.maxProjectiles) && player.Calamity().StealthStrikeAvailable())
                                    {
                                        Main.projectile[num8].Calamity().stealthStrike = true;
                                    }
                                }
                                return false;
                            }
                            if (item.type == ModContent.ItemType<NuclearFury>())
                            {
                                for (int i = 0; i < 8 + 4 * player.Entropy().WeaponBoost; i++)
                                {
                                    Vector2 velocity2 = (MathF.PI * 2f * (float)i / (float)(8 + 4 * player.Entropy().WeaponBoost) + velocity.ToRotation()).ToRotationVector2() * velocity.Length() * 0.5f;
                                    Projectile.NewProjectile(source, position, velocity2, type, damage, knockback, Main.myPlayer);
                                }
                                return false;
                            }
                            if (item.type == ModContent.ItemType<AsteroidStaff>())
                            {
                                float shootSpeed = item.shootSpeed;
                                Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
                                float num = (float)Main.mouseX + Main.screenPosition.X - vector.X;
                                float num2 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
                                if (player.gravDir == -1f)
                                {
                                    num2 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
                                }

                                float num3 = (float)Math.Sqrt(num * num + num2 * num2);
                                if ((float.IsNaN(num) && float.IsNaN(num2)) || (num == 0f && num2 == 0f))
                                {
                                    num = player.direction;
                                    num2 = 0f;
                                    num3 = shootSpeed;
                                }
                                else
                                {
                                    num3 = shootSpeed / num3;
                                }

                                int num4 = player.Entropy().WeaponBoost;
                                for (int i = 0; i < num4; i++)
                                {
                                    vector = new Vector2(player.Center.X + (float)Main.rand.Next(201) * (0f - (float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                                    vector.X = (vector.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                                    vector.Y -= 100 * i;
                                    num = (float)Main.mouseX + Main.screenPosition.X - vector.X + (float)Main.rand.Next(-40, 41) * 0.03f;
                                    num2 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
                                    if (num2 < 0f)
                                    {
                                        num2 *= -1f;
                                    }

                                    if (num2 < 20f)
                                    {
                                        num2 = 20f;
                                    }

                                    num3 = (float)Math.Sqrt(num * num + num2 * num2);
                                    num3 = shootSpeed / num3;
                                    num *= num3;
                                    num2 *= num3;
                                    float num5 = num;
                                    float num6 = num2 + (float)Main.rand.Next(-40, 41) * 0.02f;
                                    Projectile.NewProjectile(source, vector.X, vector.Y, num5 * 0.75f, num6 * 0.75f, type, damage, knockback, player.whoAmI, 0f, 0.5f + (float)Main.rand.NextDouble() * 0.3f);
                                }
                            }
                            if (type == 502)
                            {
                                for (int i = 0; i < Main.rand.Next(0, 1 + player.Entropy().WeaponBoost); i++)
                                {
                                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.2f), type, damage, knockback, player.whoAmI);
                                }
                            }
                            if (item.type == ModContent.ItemType<Disseminator>())
                            {
                                int num = 2 * player.Entropy().WeaponBoost;
                                for (int i = 0; i < num; i++)
                                {
                                    velocity.X += (float)Main.rand.Next(-15, 16) * 0.05f;
                                    velocity.Y += (float)Main.rand.Next(-15, 16) * 0.05f;
                                    int num2 = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                                    Main.projectile[num2].extraUpdates += 2;
                                }

                                int num3 = 3 * player.Entropy().WeaponBoost;
                                int[] array = new int[num3];
                                int num4 = 0;
                                Rectangle value = new Rectangle((int)player.Center.X - 960, (int)player.Center.Y - 540, 1920, 1080);
                                ActiveEntityIterator<NPC>.Enumerator enumerator = Main.ActiveNPCs.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    NPC current = enumerator.Current;
                                    if (current.chaseable && current.lifeMax > 5 && !current.dontTakeDamage && !current.friendly && !current.immortal && current.Hitbox.Intersects(value))
                                    {
                                        if (num4 >= num3)
                                        {
                                            break;
                                        }

                                        array[num4] = current.whoAmI;
                                        num4++;
                                    }
                                }

                                if (num4 == 0)
                                {
                                    return false;
                                }

                                int damage2 = (int)((double)damage * 0.7);
                                for (int j = 0; j < num4; j++)
                                {
                                    Vector2 vector = new Vector2(player.position.X + (float)player.width * 0.5f + (float)Main.rand.Next(201) * (0f - (float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                                    vector.X = (vector.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                                    vector.Y -= 100 * j;
                                    Vector2 velocity2 = Vector2.Normalize(Main.npc[array[j]].Center - vector) * item.shootSpeed;
                                    int num5 = Projectile.NewProjectile(source, vector, velocity2, type, damage2, knockback, player.whoAmI);
                                    Main.projectile[num5].extraUpdates += 14;
                                    Main.projectile[num5].tileCollide = false;
                                    Main.projectile[num5].timeLeft /= 2;
                                    vector = new Vector2(player.position.X + (float)player.width * 0.5f + (float)Main.rand.Next(201) * (0f - (float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y + 600f);
                                    vector.X = (vector.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                                    vector.Y += 100 * j;
                                    velocity2 = Vector2.Normalize(Main.npc[array[j]].Center - vector) * item.shootSpeed;
                                    num5 = Projectile.NewProjectile(source, vector, velocity2, type, damage2, knockback, player.whoAmI);
                                    Main.projectile[num5].extraUpdates += 14;
                                    Main.projectile[num5].tileCollide = false;
                                    Main.projectile[num5].timeLeft /= 2;
                                }

                                if (num4 == num3)
                                {
                                    return false;
                                }

                                for (int k = 0; k < num3 - num4; k++)
                                {
                                    int num6 = Main.rand.Next(num4);
                                    Vector2 vector = new Vector2(player.position.X + (float)player.width * 0.5f + (float)Main.rand.Next(201) * (0f - (float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                                    vector.X = (vector.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                                    vector.Y -= 100 * num6;
                                    Vector2 velocity3 = Vector2.Normalize(Main.npc[array[num6]].Center - vector) * item.shootSpeed;
                                    int num7 = Projectile.NewProjectile(source, vector, velocity3, type, damage2, knockback, player.whoAmI);
                                    Main.projectile[num7].extraUpdates += 14;
                                    Main.projectile[num7].tileCollide = false;
                                    Main.projectile[num7].timeLeft /= 2;
                                    vector = new Vector2(player.position.X + (float)player.width * 0.5f + (float)Main.rand.Next(201) * (0f - (float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y + 600f);
                                    vector.X = (vector.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                                    vector.Y += 100 * num6;
                                    velocity3 = Vector2.Normalize(Main.npc[array[num6]].Center - vector) * item.shootSpeed;
                                    num7 = Projectile.NewProjectile(source, vector, velocity3, type, damage2, knockback, player.whoAmI);
                                    Main.projectile[num7].extraUpdates += 14;
                                    Main.projectile[num7].tileCollide = false;
                                    Main.projectile[num7].timeLeft /= 2;
                                }
                            }
                        }
                        if (player.ownedProjectileCounts[ModContent.ProjectileType<TwistedTwinMinion>()] > 0)
                        {
                            if ((item.useAmmo == AmmoID.Arrow && type == ProjectileID.WoodenArrowFriendly) || (item.useAmmo == AmmoID.Bullet && type == ProjectileID.Bullet))
                            {
                                type = item.shoot;

                            }
                            else if (item.useAmmo == AmmoID.Arrow || item.useAmmo == AmmoID.Bullet)
                            {
                                Item t = player.ChooseAmmo(item);
                                if (t != null)
                                {
                                    type = t.shoot;
                                }
                            }
                            foreach (Projectile p in Main.projectile)
                            {
                                if (p.type == ModContent.ProjectileType<TwistedTwinMinion>() && p.active && p.owner == Main.myPlayer)
                                {
                                    player.Entropy().twinSpawnIndex = p.identity;
                                    p.ai[0] = 30;
                                    if (item.ModItem == null)
                                    {
                                        int pj = Projectile.NewProjectile(p.GetSource_FromAI(), position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwinMinion.damageMul), knockback, Main.myPlayer);

                                        pj.ToProj().scale *= 0.8f;
                                        pj.ToProj().Entropy().IndexOfTwistedTwinShootedThisProj = p.identity;
                                        pj.ToProj().netUpdate = true;

                                        Projectile projts = pj.ToProj();
                                        if (!projts.usesLocalNPCImmunity)
                                        {
                                            pj.ToProj().usesLocalNPCImmunity = true;
                                            pj.ToProj().localNPCHitCooldown = 12;
                                        }
                                    }
                                    else
                                    {
                                        if (item.ModItem.Shoot(player, source, position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwinMinion.damageMul), knockback))
                                        {
                                            int pj = Projectile.NewProjectile(p.GetSource_FromAI(), position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwinMinion.damageMul), knockback, Main.myPlayer);
                                            pj.ToProj().scale *= 0.8f;
                                            pj.ToProj().Entropy().IndexOfTwistedTwinShootedThisProj = p.identity;
                                            pj.ToProj().netUpdate = true;
                                            Projectile projts = pj.ToProj();
                                            if (!projts.usesLocalNPCImmunity)
                                            {
                                                pj.ToProj().usesLocalNPCImmunity = true;
                                                pj.ToProj().localNPCHitCooldown = 12;
                                            }
                                        }
                                    }
                                    player.Entropy().twinSpawnIndex = -1;
                                    /*int pj = Projectile.NewProjectile(p.GetSource_FromAI(), position + p.Center - player.Center, velocity, type, (int)(damage * 0.26f), knockback, Main.myPlayer);
                                    *                                     {
                                        
                                    }*                                     
                                    pj.ToProj().scale *= 0.8f;
                                    
                                    pj.ToProj().Entropy().ttindex = p.whoAmI;*/
                                }

                            }
                        }
                    }
                }
            }
            return true;
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.Entropy().plagueEngine && item.DamageType.CountsAsClass<TrueMeleeDamageClass>())
            {
                PlagueInternalCombustionEngine.ApplyTrueMeleeEffect(player);
            }
            if (item.type == ModContent.ItemType<StellarStriker>())
            {
                IEntitySource source_ItemUse = player.GetSource_ItemUse(item);
                SoundEngine.PlaySound(in SoundID.Item88, player.Center);
                int myPlayer = Main.myPlayer;
                float shootSpeed = item.shootSpeed;
                Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
                for (int i = 0; i < player.Entropy().WeaponBoost; i++)
                {
                    vector = new Vector2(player.Center.X + (float)Main.rand.Next(201) * (0f - (float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                    vector.X = (vector.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                    vector.Y -= 100 * i;
                    Vector2 velocity = CalamityUtils.CalculatePredictiveAimToTargetMaxUpdates(vector, target, shootSpeed, 6);
                    int num = Projectile.NewProjectile(source_ItemUse, vector, velocity, 645, damageDone, player.GetWeaponKnockback(item), myPlayer, 0f, Main.rand.Next(3));
                    if (num.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[num].DamageType = DamageClass.Melee;
                    }
                }
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            float counter = ModContent.GetInstance<EModSys>().counter;
            Color namecolor = line.Color;
            if (!HasCustomNameColor)
            {
                namecolor = (Color)item.Entropy().NameColor;
            }
            if (line.Mod == "Terraria")
            {
                if (item.type == ModContent.ItemType<TheFilthyContractWithMammon>() && line.Text.Contains("*"))
                {
                    return false;
                }
                if (line.Text.Contains("$"))
                {
                    if(item.type == ModContent.ItemType<TheFilthyContractWithMammon>())
                    {
                        float p = 1;
                        Main.spriteBatch.Draw(CEUtils.getExtraTex("T1"), new Vector2(line.X, line.Y - 4) + new Vector2(p, p), Color.Red); Main.spriteBatch.Draw(CEUtils.getExtraTex("T1"), new Vector2(line.X, line.Y - 4), Color.Red);
                        Main.spriteBatch.Draw(CEUtils.getExtraTex("T1"), new Vector2(line.X, line.Y - 4) + new Vector2(-p, p), Color.Red);
                        Main.spriteBatch.Draw(CEUtils.getExtraTex("T1"), new Vector2(line.X, line.Y - 4) + new Vector2(p, -p), Color.Red);
                        Main.spriteBatch.Draw(CEUtils.getExtraTex("T1"), new Vector2(line.X, line.Y - 4) + new Vector2(-p, -p), Color.Red);

                        Main.spriteBatch.Draw(CEUtils.getExtraTex("T1"), new Vector2(line.X, line.Y - 4), Color.Black);

                        
                        return false;
                    }
                    if(item.type == ModContent.ItemType<CelestialChronometer>())
                    {
                        string textall = line.Text.Replace("$", "");
                        float xa = 0; var font = FontAssets.MouseText.Value;
                        float h = 0;
                        for (int i = 0; i < textall.Length; i++)
                        {
                            var text = textall[i].ToString();
                            Vector2 size = font.MeasureString(text);
                            float yofs;
                            if (size.Y > h)
                            {
                                h = size.Y;
                            }
                            Color color = Color.White;
                            yofs = 0;
                            Color strokeColord = Main.DiscoColor;

                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);


                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs), color);

                            xa += size.X + 2;

                        }
                        SpriteBatch sb = Main.spriteBatch;
                        sb.End();
                        sb.Begin(0, BlendState.Additive, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
                        Texture2D glow = CEUtils.getExtraTex("Glow");
                        sb.Draw(glow, new Vector2(line.X + xa / 2, line.Y + h / 4), null, new Color(255, 255, 255) * 0.6f, 0, glow.Size() / 2, new Vector2((32 + xa * 2.4f) / glow.Width, 0.34f), SpriteEffects.None, 0);
                        sb.End();
                        sb.Begin(0, BlendState.AlphaBlend, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                        return false;
                    }
                    if(item.type == ModContent.ItemType<ScorchingShoot>())
                    {
                        string textall = line.Text.Replace("$", "");
                        float xa = 0; var font = FontAssets.MouseText.Value;
                        float h = 0;
                        for (int i = 0; i < textall.Length; i++)
                        {
                            var text = textall[i].ToString();
                            Vector2 size = font.MeasureString(text);
                            float yofs;
                            if (size.Y > h)
                            {
                                h = size.Y;
                            }
                            Color color = Color.White;
                            yofs = 0;
                            Color strokeColord = Color.Orange;

                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);


                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs), color);

                            xa += size.X + 2;

                        }
                        SpriteBatch sb = Main.spriteBatch;
                        sb.End();
                        sb.Begin(0, BlendState.Additive, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
                        Texture2D glow = CEUtils.getExtraTex("Glow");
                        sb.Draw(glow, new Vector2(line.X + xa / 2, line.Y + h / 4), null, Color.Orange * 0.8f, 0, glow.Size() / 2, new Vector2((32 + xa * 2.6f) / glow.Width, 0.34f), SpriteEffects.None, 0);
                        sb.End();
                        sb.Begin(0, BlendState.AlphaBlend, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                        return false;
                    }
                }
            }
            if (line.Name == "ItemName")
            {
                if (item.Entropy().tooltipStyle == 1 || item.Entropy().tooltipStyle == 4)
                {
                    float xa = 0;
                    for (int i = 0; i < line.Text.Length; i++)
                    {
                        string text = line.Text[i].ToString();
                        var font = FontAssets.MouseText.Value;
                        Vector2 size = font.MeasureString(text);
                        float yofs;
                        int cj = (int)(Math.Cos(counter / 14 - i * 1) * 50);
                        Color color = new Color(namecolor.R + cj, namecolor.G + cj, namecolor.B + cj, namecolor.A);
                        if (color.R > 255)
                        {
                            color.R = 255;
                        }
                        if (color.G > 255)
                        {
                            color.G = 255;
                        }
                        if (color.B > 255)
                        {
                            color.B = 255;
                        }
                        if (color.R < 0)
                        {
                            color.R = 0;
                        }
                        if (color.G < 0)
                        {
                            color.G = 0;
                        }
                        if (color.B < 0)
                        {
                            color.B = 0;
                        }

                        yofs = 0;
                        if (item.Entropy().tooltipStyle == 1)
                        {
                            yofs = (float)(Math.Cos(counter / 14 - i * 1) * 1.3f) + 1f;
                        }
                        if (item.Entropy().stroke)
                        {
                            Color strokeColord = Color.White;
                            if (!HasCustomStrokeColor)
                            {
                                strokeColord = color;
                                strokeColord.R = (byte)(strokeColord.R * 0.2f);
                                strokeColord.G = (byte)(strokeColord.G * 0.2f);
                                strokeColord.B = (byte)(strokeColord.B * 0.2f);

                            }
                            else
                            {
                                strokeColord = (Color)strokeColor;
                            }
                            strokeColord.A = 255;
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                            Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

                        }
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs), color);



                        xa += size.X;
                        if (item.Entropy().stroke)
                        {
                            xa += 2;
                        }
                    }
                    return false;
                }
                if (item.rare == ModContent.RarityType<VoidPurple>())
                {
                    float xa = 0;
                    for (int i = 0; i < line.Text.Length; i++)
                    {
                        string text = line.Text[i].ToString();
                        var font = FontAssets.MouseText.Value;
                        Vector2 size = font.MeasureString(text);
                        float yofs;
                        Color color = Color.Black;
                        yofs = 0;
                        Color strokeColord = new Color(106, 40, 190);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);


                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs), color);



                        xa += size.X;

                    }
                    return false;
                }
                if (item.rare == ModContent.RarityType<AbyssalBlue>())
                {
                    float xa = 0; var font = FontAssets.MouseText.Value;
                    float h = 0;
                    for (int i = 0; i < line.Text.Length; i++)
                    {
                        string text = line.Text[i].ToString();

                        Vector2 size = font.MeasureString(text);
                        float yofs;
                        if (size.Y > h)
                        {
                            h = size.Y;
                        }
                        Color color = new Color(248, 132, 56);
                        yofs = 0;
                        Color strokeColord = new Color(104, 120, 255);

                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);


                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs), color);

                        xa += size.X + 2;

                    }
                    SpriteBatch sb = Main.spriteBatch;
                    sb.End();
                    sb.Begin(0, BlendState.Additive, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
                    Texture2D glow = CEUtils.getExtraTex("Glow");
                    sb.Draw(glow, new Vector2(line.X + xa / 2, line.Y + h / 4), null, new Color(140, 150, 255) * 0.8f, 0, glow.Size() / 2, new Vector2((32 + xa * 2.4f) / glow.Width, 0.34f), SpriteEffects.None, 0);
                    sb.End();
                    sb.Begin(0, BlendState.AlphaBlend, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                    return false;
                }
                if (item.rare == ModContent.RarityType<GlowGreen>() || item.rare == ModContent.RarityType<GlowPurple>() || item.rare == ModContent.RarityType<SkyBlue>())
                {
                    float xa = 0;
                    for (int i = 0; i < line.Text.Length; i++)
                    {
                        string text = line.Text[i].ToString();
                        var font = FontAssets.MouseText.Value;
                        Vector2 size = font.MeasureString(text);
                        float yofs;
                        Color color = new Color(60, 255, 60);
                        if (item.rare == ModContent.RarityType<GlowPurple>())
                        {
                            color = new Color(120, 0, 180);
                        }
                        if (item.rare == ModContent.RarityType<SkyBlue>())
                        {
                            color = new Color(24, 24, 255);
                        }
                        yofs = 0;
                        Color strokeColord = new Color(210, 255, 210);
                        if (item.rare == ModContent.RarityType<GlowPurple>())
                        {
                            strokeColord = new Color(146, 86, 240);
                        }
                        if (item.rare == ModContent.RarityType<SkyBlue>())
                        {
                            strokeColord = new Color(180, 180, 255);
                        }
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);


                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs), color);



                        xa += size.X + 1;

                    }
                    return false;
                }
            }
            if (line.Name == "LegendItem" || (line.Name == "ItemName" && item.Entropy().tooltipStyle == 2))
            {
                float xa = 0;
                for (int i = 0; i < line.Text.Length; i++)
                {
                    string text = line.Text[i].ToString();
                    var font = FontAssets.MouseText.Value;
                    Vector2 size = font.MeasureString(text);
                    float yofs;
                    int cj = (int)(Math.Cos(counter / 10 - i * 1) * 70);
                    Color color = new Color(Main.DiscoR + cj, Main.DiscoG + cj, Main.DiscoB + cj, namecolor.A);
                    if (color.R > 255)
                    {
                        color.R = 255;
                    }
                    if (color.G > 255)
                    {
                        color.G = 255;
                    }
                    if (color.B > 255)
                    {
                        color.B = 255;
                    }
                    if (color.R < 0)
                    {
                        color.R = 0;
                    }
                    if (color.G < 0)
                    {
                        color.G = 0;
                    }
                    if (color.B < 0)
                    {
                        color.B = 0;
                    }
                    yofs = (float)(Math.Cos(counter / 14) * 1.3f) + 1f;
                    if (item.Entropy().stroke)
                    {
                        Color strokeColord = Color.White;
                        if (!HasCustomStrokeColor)
                        {
                            strokeColord = color;
                            strokeColord.R = (byte)(strokeColord.R * 0.5f);
                            strokeColord.G = (byte)(strokeColord.G * 0.5f);
                            strokeColord.B = (byte)(strokeColord.B * 0.5f);
                        }
                        else
                        {
                            strokeColord = (Color)strokeColor;
                        }
                        strokeColord.A = 255;
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

                    }
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs), color);
                    xa += size.X;
                    if (item.Entropy().stroke)
                    {
                        xa += 2;
                    }
                }
                return false;
            }
            if (line.Name == "ItemName" && item.Entropy().tooltipStyle == 3)
            {
                string text = line.Text.ToString();
                var font = FontAssets.MouseText.Value;
                Vector2 size = font.MeasureString(text);
                int cj = (int)(Math.Cos(counter / 16) * 50) - 40;
                Color color = new Color(namecolor.R + cj, namecolor.G + cj, namecolor.B + cj, namecolor.A);
                if (color.R > 255)
                {
                    color.R = 255;
                }
                if (color.G > 255)
                {
                    color.G = 255;
                }
                if (color.B > 255)
                {
                    color.B = 255;
                }
                if (color.R < 0)
                {
                    color.R = 0;
                }
                if (color.G < 0)
                {
                    color.G = 0;
                }
                if (color.B < 0)
                {
                    color.B = 0;
                }
                if (item.Entropy().stroke)
                {
                    Color strokeColord = Color.White;
                    if (!HasCustomStrokeColor)
                    {
                        strokeColord = color;
                        strokeColord.R = (byte)(strokeColord.R * 0.5f);
                        strokeColord.G = (byte)(strokeColord.G * 0.5f);
                        strokeColord.B = (byte)(strokeColord.B * 0.5f);
                    }
                    else
                    {
                        strokeColord = (Color)strokeColor;
                    }
                    strokeColord.A = 255;
                    int xa = 0;
                    int yofs = 0;
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(-1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(0, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, -1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 0), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    Main.spriteBatch.DrawString(font, text, new Vector2(line.X + xa, line.Y + yofs) + new Vector2(1, 1), strokeColord, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

                }
                Main.spriteBatch.DrawString(font, text, new Vector2(line.X, line.Y), color);

                if (counter % 15 == 0)
                {
                    S3Particle pt = new S3Particle();
                    var r = Main.rand;
                    pt.velocity = new Vector2((float)r.Next(-2, 3) / 10, -(float)r.Next(4, 6) / 10);
                    pt.position = new Vector2(r.Next(0, (int)size.X), size.Y);

                    particles1.Add(pt);
                }

                foreach (S3Particle p in particles1)
                {
                    p.update();
                    float alpha = 1;
                    if (p.position.Y > size.Y - 10)
                    {
                        alpha = (10f - (float)(p.position.Y - (size.Y - 10))) / 10;
                        if (alpha > 1)
                        {
                            alpha = 1;
                        }

                    }
                    if (p.position.Y < 8)
                    {
                        alpha = ((float)p.position.Y) / 8;
                    }
                    if (alpha > 1)
                    {
                        alpha = 1;
                    }
                    p.draw(alpha * 0.5f, new Vector2(line.X, line.Y), namecolor);

                }
                foreach (S3Particle p in particles1)
                {
                    if (p.position.Y < -30)
                    {
                        particles1.Remove(p);
                        break;
                    }
                }
                return false;
            }
            return true;
        }

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.FishronBossBag)
            {
                itemLoot.Add(ItemDropRule.ByCondition(new IsDeathMode(), ModContent.ItemType<IlmeranAsylum>()));
            }
            if (item.type == ItemID.FloatingIslandFishingCrate)
            {
                itemLoot.Add(ModContent.ItemType<IndigoCard>(), 5);
            }
            if (item.type == ItemID.GolemBossBag)
            {
                itemLoot.Add(ModContent.ItemType<MourningCard>(), 1);
            }
            if (item.type == 3203 || item.type == 3204 || item.type == 3983 || item.type == 3982)
            {
                itemLoot.Add(ModContent.ItemType<ObscureCard>(), 5);
            }
            if (item.Is<CrabulonBag>())
            {
                itemLoot.Add(ModContent.ItemType<WisperCard>(), 2);
            }
            if (item.Is<PlaguebringerGoliathBag>())
            {
                itemLoot.Add(ModContent.ItemType<PlagueInternalCombustionEngine>(), 4);
            }
            if (item.type == ModContent.ItemType<HiveMindBag>())
            {
                itemLoot.Add(ModContent.ItemType<MindCorruptor>(), 3);
            }
            if (item.type == ModContent.ItemType<PerforatorBag>())
            {
                itemLoot.Add(ModContent.ItemType<SinewLash>(), 3);
            }
            if (item.type == ModContent.ItemType<HiveMindBag>() || item.type == ModContent.ItemType<PerforatorBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkAerialite>(), new Fraction(1, 2));
            }
            if (item.Is<LeviathanBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkAquarius>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.QueenSlimeBossBag)
            {
                itemLoot.Add(ModContent.ItemType<Crystedge>(), new Fraction(1, 3));
            }
            if (item.type == ItemID.SkeletronBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkAries>(), new Fraction(1, 1));
            }
            if (item.Is<AstrumDeusBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkAstral>(), new Fraction(1, 2));
            }
            if (item.Is<YharonBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkAuric>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.QueenBeeBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkBee>(), new Fraction(1, 1));
            }
            if (item.Is<BrimstoneWaifuBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkBrimstone>(), new Fraction(1, 2));
            }
            if (item.Is<CrabulonBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkCancer>(), new Fraction(1, 2));
            }
            if (item.Is<AquaticScourgeBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkCapricorn>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.EaterOfWorldsBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkCorrupt>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.BrainOfCthulhuBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkCrimson>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.WallOfFleshBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkFlesh>(), new Fraction(1, 1));
            }
            if (item.Is<NihilityTwinBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkGemini>(), new Fraction(1, 1));
            }
            if (ModLoader.TryGetMod("CalamityHunt", out Mod ch) && (item.type == ch.Find<ModItem>("TreasureTrunk").Type || item.type == ch.Find<ModItem>("TreasureBucket").Type))
            {
                itemLoot.Add(ModContent.ItemType<BookMarkGoozma>(), new Fraction(1, 1));
            }
            if (ModLoader.TryGetMod("CatalystMod", out Mod cl) && item.type == cl.Find<ModItem>("AstrageldonBag").Type)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkIntergelactic>(), new Fraction(1, 2));
            }
            if (item.Is<CryogenBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkIce>(), new Fraction(1, 2));
            }
            if (item.Is<DesertScourgeBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkLeo>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.FairyQueenBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkLibra>(), new Fraction(1, 1));
            }
            if (item.type == ItemID.MoonLordBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkLunar>(), new Fraction(1, 1));
            }
            if (item.type == ItemID.SkeletronPrimeBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkMechanical>(), new Fraction(1, 1));
            }
            if (item.type == ItemID.QueenSlimeBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkOfLight>(), new Fraction(1, 1));
            }
            if (item.Is<CalamitasCloneBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkOfNight>(), new Fraction(1, 1));
            }
            if (item.Is<CalamitasCoffer>())
            {
                itemLoot.Add(ModContent.ItemType<BookmarkPactOfDecay>(), new Fraction(1, 1));
            }
            if (item.Is<DraedonBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookmarkPactOfWar>(), new Fraction(1, 1));
            }
            if (item.type == ItemID.FishronBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkPisces>(), new Fraction(1, 1));
            }
            if (item.Is<ProvidenceBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkProfaned>(), new Fraction(1, 1));
            }
            if (item.type == ItemID.EyeOfCthulhuBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkSagittarius>(), new Fraction(1, 2));
                itemLoot.Add(ModContent.ItemType<BookMarkVirgo>(), new Fraction(1, 2));
            }
            if (item.Is<AstrumAureusBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkScorpio>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.PlanteraBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkSilva>(), new Fraction(1, 2));
                itemLoot.Add(ModContent.ItemType<LashingBramblerod>(), new Fraction(4, 5));
            }
            if (item.Is<SlimeGodBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkTaurus>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.GolemBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkTerra>(), new Fraction(1, 2));
            }
            if (item.type == ItemID.KingSlimeBossBag)
            {
                itemLoot.Add(ModContent.ItemType<BookMarkRoyal>(), new Fraction(1, 2));
            }
            if (item.Is<CruiserBag>())
            {
                itemLoot.Add(ModContent.ItemType<BookMarkVoid>(), new Fraction(1, 1));
            }

            if (item.type == ItemID.PlanteraBossBag)
            {
                itemLoot.Add(ModContent.ItemType<ToyGuitar>(), new Fraction(1, 5));
            }
            if (item.type == ModContent.ItemType<StormWeaverBag>())
            {
                itemLoot.Add(ItemDropRule.ByCondition(new IsDeathMode(), ModContent.ItemType<HeartOfStorm>()));
            }
            if (item.type == ModContent.ItemType<AstrumDeusBag>())
            {
                itemLoot.Add(ItemDropRule.ByCondition(new IsDeathMode(), ModContent.ItemType<DeusCore>()));
            }
            if (item.type == ModContent.ItemType<BrimstoneWaifuBag>())
            {
                itemLoot.Add(ModContent.ItemType<EvilFriend>(), new Fraction(4, 9));
            }
            if (item.type == ModContent.ItemType<YharonBag>())
            {
                itemLoot.Add(ModContent.ItemType<Vitalfeather>(), new Fraction(1, 4));
            }
            if (item.type == ModContent.ItemType<AstrumAureusBag>())
            {
                itemLoot.Add(ModContent.ItemType<NightProjection>(), new Fraction(4, 9));
            }
            if (item.type == ModContent.ItemType<PolterghastBag>())
            {
                itemLoot.Add(ModContent.ItemType<AnimaSola>(), new Fraction(1, 2));
            }
            if (item.type == ModContent.ItemType<AquaticScourgeBag>())
            {
                itemLoot.Add(ModContent.ItemType<AquaticFlute>(), new Fraction(1, 3));
            }
            if (item.type == ModContent.ItemType<DesertScourgeBag>())
            {
                itemLoot.Add(ModContent.ItemType<DustyWhistle>(), new Fraction(1, 4));
            }
            if (item.type == ModContent.ItemType<CalamitasCloneBag>())
            {
                itemLoot.Add(ModContent.ItemType<FriendBox>(), new Fraction(1, 10));
            }
            if (item.type == ItemID.PlanteraBossBag)
            {
                itemLoot.Add(ItemDropRule.ByCondition(new IsDeathMode(), ModContent.ItemType<SilvasCrown>()));
            }
            if (item.type == ItemID.KingSlimeBossBag)
            {
                itemLoot.Add(ModContent.ItemType<SlimeYoyo>(), new Fraction(4, 10));
            }
            if (item.type == ItemID.DeerclopsBossBag)
            {
                itemLoot.Add(ModContent.ItemType<Antler>(), new Fraction(4, 10));
            }
            if (item.type == ModContent.ItemType<HydrothermalCrate>())
            {
                itemLoot.Add(ModContent.ItemType<EnduranceCard>(), new Fraction(1, 5));
            }
            if (item.type == ItemID.IronCrate || item.type == ItemID.IronCrateHard)
            {
                itemLoot.Add(ModContent.ItemType<AuraCard>(), new Fraction(1, 10));
            }
            if (item.type == ItemID.OasisCrate || item.type == ItemID.OasisCrateHard)
            {
                itemLoot.Add(ModContent.ItemType<InspirationCard>(), new Fraction(3, 10));
            }
            if (item.type == ModContent.ItemType<StarterBag>())
            {
                static bool getsDH(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("polaris");
                }
                ;
                static bool getsWyrm(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("polaris") || playerName.ToLower().Contains("妖龙") || playerName.ToLower().Contains("wyrm");
                }
                ;
                itemLoot.AddIf(getsDH, ModContent.ItemType<DustyStar>());
                itemLoot.AddIf(getsWyrm, ModContent.ItemType<AbyssLantern>());
                static bool getsAH(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("ahi") || playerName.ToLower().Contains("fr9");
                }
                ;
                itemLoot.AddIf(getsAH, ModContent.ItemType<GalaxyGrapeSoda>());

                static bool getsDD(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("dream") || playerName.ToLower().Contains("梦");
                }
                ;
                itemLoot.AddIf(getsDD, ModContent.ItemType<DreamCatcher>());


                static bool getsCHA(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("cha") || playerName.ToLower().Contains("lost");
                }
                ;
                itemLoot.AddIf(getsCHA, ModContent.ItemType<ToyKnife>());

                static bool getsAN(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("rat") || playerName.ToLower().Contains("ant");
                }
                ;
                itemLoot.AddIf(getsAN, ModContent.ItemType<Antler>());

                static bool getsSW(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("away") || playerName.ToLower().Contains("weaver");
                }
                ;
                itemLoot.AddIf(getsSW, ModContent.ItemType<CrimsonNight>());

                static bool getsMO(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("mo");
                }
                ;
                itemLoot.AddIf(getsMO, ModContent.ItemType<MosHat>());

                itemLoot.AddIf((info) => (info.player.name.ToLower().Contains("ylg") || info.player.name.ToLower().Contains("烟玉")), ModContent.ItemType<YanyusHat>());

                itemLoot.AddIf((info) => (info.player.name.ToLower().Contains("sora")), ModContent.ItemType<MysteriousBook>());

                itemLoot.AddIf((info) => (info.player.name.ToLower().Contains("心斩狂歌")), ModContent.ItemType<LostChubbyBird>());

                itemLoot.AddIf((info) => (info.player.name.ToLower().Contains("nicholas")), ModContent.ItemType<PineappleDog>());

                itemLoot.AddIf((info) => (info.player.name.ToLower().Contains("lily")||info.player.name.Contains("莉莉")), ModContent.ItemType<LostHeirloom>());

                if (ModLoader.TryGetMod("MagicStorage", out Mod magicStorage))
                {
                    ModItem i;
                    if (magicStorage.TryFind<ModItem>("CraftingAccess", out i))
                    {
                        itemLoot.Add(i.Type, 1);
                    }
                    if (magicStorage.TryFind<ModItem>("StorageHeart", out i))
                    {
                        itemLoot.Add(i.Type, 1);
                    }
                    if (magicStorage.TryFind<ModItem>("StorageUnit", out i))
                    {
                        itemLoot.Add(i.Type, 1, 10, 10);
                    }

                }
            }
        }
        public class IsDeathMode : IItemDropRuleCondition, IProvideItemConditionDescription
        {
            public bool CanDrop(DropAttemptInfo info) => CalamityWorld.death;
            public bool CanShowItemDropInUI() => CalamityWorld.death;
            public string GetConditionDescription() => Language.GetTextValue("Mods.CalamityEntropy.DeathMode");
        }
    }
}
