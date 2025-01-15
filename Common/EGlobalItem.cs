using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Accessories.Cards;
using CalamityEntropy.Content.Items.Armor.VoidFaquir;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.Vanity;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Items.Weapons.CrystalBalls;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
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

        public override void SetDefaults(Item entity)
        {
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
            if(item.type == ItemID.SantaHat)
            {
                player.Entropy().cHat = true;
            }
            if (armorPrefix != null)
            {
                armorPrefix.updateEquip(player, item);
                player.statDefense += (int)(Math.Round(item.defense * armorPrefix.AddDefense()));
            }
        }
        public override void UpdateVanity(Item item, Player player)
        {
            if (item.type == ItemID.SantaHat)
            {
                player.Entropy().cHat = true;
            }
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (player.channel || player.whoAmI != Main.myPlayer || item.pick > 0 || item.axe > 0 || !player.Entropy().TarnishCard)
            {
                return null;
            }
            var mp = player.Entropy();
            if (mp.BlackFlameCd <= 0 && player.whoAmI == Main.myPlayer)
            {
                mp.BlackFlameCd = 4;
                Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One) * 14, ModContent.ProjectileType<BlackFire>(), player.GetWeaponDamage(item) / 5 + 1, 2, player.whoAmI);
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

        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (Util.Util.IsArmor(item) && Main.rand.NextDouble() < ModContent.GetInstance<ServerConfig>().CraftArmorWithPrefixChance) {
                ArmorPrefix armorPrefix = ArmorPrefix.RollPrefixToItem(item);
                if (armorPrefix != null)
                {
                    item.Entropy().armorPrefix = armorPrefix;
                    item.Entropy().armorPrefixName = armorPrefix.RegisterName();
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
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
                if (Main.LocalPlayer.Entropy().VFHelmRouge)
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
                for (int i = 0; i < player.inventory.Count(); i++) {
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
            if (EModSys.noItemUse)
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
            if (!Main.dedServ)
            {
                if (item.DamageType != DamageClass.Summon)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        if(player.Entropy().WeaponBoost > 0)
                        {
                            if(item.type == ModContent.ItemType<LunarKunai>())
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
                            if(type == 502)
                            {
                                for(int i = 0; i < Main.rand.Next(0, 1 + player.Entropy().WeaponBoost); i++)
                                {
                                    Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.2f), type, damage, knockback, player.whoAmI);
                                }
                            }
                            if(item.type == ModContent.ItemType<Disseminator>())
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
                                        pj.ToProj().Entropy().ttindex = p.identity;
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
                                        if(item.ModItem.Shoot(player, source, position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwinMinion.damageMul), knockback))
                                        {
                                            int pj = Projectile.NewProjectile(p.GetSource_FromAI(), position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwinMinion.damageMul), knockback, Main.myPlayer);
                                            pj.ToProj().scale *= 0.8f;
                                            pj.ToProj().Entropy().ttindex = p.identity;
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
                                    *//*if (true || (item.DamageType == DamageClass.Melee && (velocity.Length() < 2.4f || velocity.Length() > 45)) || type == ModContent.ProjectileType<PrisonOfPermafrostCircle>() || type == ModContent.ProjectileType<VBSpawner>() || type == ModContent.ProjectileType<PrisonOfPermafrostCircle>() || type == ModContent.ProjectileType<ExobladeProj>())
                                    {
                                        
                                    }*//*
                                    
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
            if(item.type == ModContent.ItemType<StellarStriker>())
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
                if (item.rare == ModContent.RarityType<GlowGreen>() || item.rare == ModContent.RarityType<GlowPurple>())
                {
                    float xa = 0;
                    for (int i = 0; i < line.Text.Length; i++)
                    {
                        string text = line.Text[i].ToString();
                        var font = FontAssets.MouseText.Value;
                        Vector2 size = font.MeasureString(text);
                        float yofs;
                        Color color = new Color(60, 255, 60);
                        if(item.rare == ModContent.RarityType<GlowPurple>())
                        {
                            color = new Color(120, 0, 180);
                        }
                        yofs = 0;
                        Color strokeColord = new Color(210, 255, 210);
                        if (item.rare == ModContent.RarityType<GlowPurple>())
                        {
                            color = new Color(255, 140, 255);
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
            if (item.type == ModContent.ItemType<BrimstoneWaifuBag>())
            {
                itemLoot.Add(ModContent.ItemType<EvilFriend>(), new Fraction(4, 9));
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
                itemLoot.Add(ModContent.ItemType<FriendBox>(), new Fraction(1, 3));
            }
            if (item.type == ItemID.WallOfFleshBossBag)
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
                };
                itemLoot.AddIf(getsDH, ModContent.ItemType<DustyStar>());

                static bool getsAH(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("ahi") || playerName.ToLower().Contains("fr9");
                };
                itemLoot.AddIf(getsAH, ModContent.ItemType<GalaxyGrapeSoda>());

                static bool getsDD(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("dream") || playerName.ToLower().Contains("梦");
                };
                itemLoot.AddIf(getsDD, ModContent.ItemType<DreamCatcher>());

                static bool getsDM(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("puslin");
                };
                itemLoot.AddIf(getsDM, ModContent.ItemType<SilverFramedGlasses>());


                static bool getsCHA(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("cha") || playerName.ToLower().Contains("lost");
                };
                itemLoot.AddIf(getsCHA, ModContent.ItemType<ToyKnife>());

                static bool getsAN(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("rat") || playerName.ToLower().Contains("ant");
                };
                itemLoot.AddIf(getsAN, ModContent.ItemType<Antler>());

                static bool getsSW(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("away") || playerName.ToLower().Contains("weaver");
                };
                itemLoot.AddIf(getsSW, ModContent.ItemType<CrimsonNight>());

                static bool getsMO(DropAttemptInfo info)
                {
                    string playerName = info.player.name;
                    return playerName.ToLower().Contains("mo");
                };
                itemLoot.AddIf(getsMO, ModContent.ItemType<MosHat>());
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
