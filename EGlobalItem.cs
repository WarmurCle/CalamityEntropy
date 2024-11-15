﻿using CalamityMod;
using CalamityMod.CalPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using CalamityEntropy.Items;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityEntropy.Projectiles.TwistedTwin;
using CalamityEntropy.Projectiles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.TreasureBags;
using CalamityEntropy.Buffs;
using CalamityEntropy.Items.Accessories.Cards;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityEntropy.Items.Vanity;
using CalamityMod.Items.Pets;
using Terraria.GameContent.ItemDropRules;
using CalamityEntropy.Items.Armor.VoidFaquir;

namespace CalamityEntropy
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
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Extra/style3").Value;
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

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            speed *= 1 + player.Entropy().VoidCharge * 0.5f;
            acceleration *= 1 + player.Entropy().VoidCharge * 0.5f;
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling *= 1 + player.Entropy().VoidCharge * 0.5f;
            ascentWhenRising *= 1 + player.Entropy().VoidCharge * 0.5f;
            maxAscentMultiplier *= 1 + player.Entropy().VoidCharge * 0.5f;
            maxCanAscendMultiplier *= 1 + player.Entropy().VoidCharge * 0.5f;
            constantAscend *= 1 + player.Entropy().VoidCharge * 0.5f;
        }

        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
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
            if (player.HasBuff(ModContent.BuffType<StealthState>()) || player.Entropy().DarkArtsTarget.Count > 0)
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
                        if (player.ownedProjectileCounts[ModContent.ProjectileType<TwistedTwin>()] > 0)
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
                                if (p.type == ModContent.ProjectileType<TwistedTwin>() && p.active && p.owner == Main.myPlayer)
                                {
                                    player.Entropy().twinSpawnIndex = p.whoAmI;
                                    p.ai[0] = 30;
                                    if (item.ModItem == null)
                                    {
                                        int pj = Projectile.NewProjectile(p.GetSource_FromAI(), position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwin.damageMul), knockback, Main.myPlayer);

                                        pj.ToProj().scale *= 0.8f;
                                        pj.ToProj().Entropy().OnProj = p.whoAmI;
                                        pj.ToProj().Entropy().ttindex = p.whoAmI;
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
                                        if(item.ModItem.Shoot(player, source, position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwin.damageMul), knockback))
                                        {
                                            int pj = Projectile.NewProjectile(p.GetSource_FromAI(), position + p.Center - player.Center, velocity, type, (int)(damage * TwistedTwin.damageMul), knockback, Main.myPlayer);
                                            pj.ToProj().scale *= 0.8f;
                                            pj.ToProj().Entropy().OnProj = p.whoAmI;
                                            pj.ToProj().Entropy().ttindex = p.whoAmI;
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
            if (item.type == ModContent.ItemType<CalamitasCloneBag>())
            {
                itemLoot.Add(ModContent.ItemType<FriendBox>(), new Fraction(1, 3));
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
            }
        }
    }
}
