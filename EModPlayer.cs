﻿using CalamityEntropy.Buffs;
using CalamityEntropy.Cooldowns;
using CalamityEntropy.Items;
using CalamityEntropy.Projectiles;
using CalamityEntropy.Projectiles.Cruiser;
using CalamityEntropy.Projectiles.VoidEchoProj;
using CalamityEntropy.Tiles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Prefixes;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy
{
    public class EModPlayer : ModPlayer
    {
        public bool CruiserLoreUsed = false;
        public bool Godhead = false;
        public bool auraCard = false;
        public int brillianceCard = 0;
        public bool enduranceCard = false;
        public bool entityCard = false;
        public bool inspirationCard = false;
        public bool metropolisCard = false;
        public bool radianceCard = false;
        public bool temperanceCard = false;
        public bool wisdomCard = false;
        public bool oracleDeck = false;
        public bool holyMoonlight = false;
        public int MagiShield;
        public static int localPlayerMaxShield = 0;
        public int HMRegenCd = 0;
        public int pot_time = 0;
        public int pot_amp = 0;
        public bool oracleDeskInInv = false;

        public bool summonerVF;
        public bool magiVF;
        public bool rougeVF;
        public bool meleeVF;
        public bool rangerVF;
        public bool VFSet;
        public bool VFLeg;
        public bool VFHelmRanged;
        public bool VFHelmMagic;
        public bool VFHelmSummoner;
        public bool VFHelmRouge;
        public bool VFHelmMelee;
        public int vfcd = 0;
        public float voidcharge = 0;
        public float VoidCharge 
        { 
            get { return voidcharge; } 
            set { 
                if (value < voidcharge) { 
                    voidcharge = value;
                } else if (vfcd <= 0) { 
                    voidcharge = value; vfcd = 8;
                }
            }
        }
        public int VoidInspire = 0;

        public bool GreedCard = false;
        public override void ResetEffects()
        {
            Godhead = false;
            auraCard = false;
            if (brillianceCard > 0)
            {
                brillianceCard -= 1;
            }
            enduranceCard = false;
            entityCard = false;
            inspirationCard = false;
            metropolisCard = false;
            radianceCard = false;
            temperanceCard = false;
            wisdomCard = false;
            oracleDeck = false;
            holyMoonlight = false;
            oracleDeskInInv = false;
            summonerVF = false;
            magiVF = false;
            rougeVF = false;
            meleeVF = false;
            rangerVF = false;
            VFSet = false;
            VFLeg = false;
            VFHelmRanged = false;
            VFHelmMagic = false;
            VFHelmSummoner = false;
            VFHelmRouge = false;
            VFHelmMelee = false;

            GreedCard = false;
        }
        public int crSky = 0;
        public int llSky = 0;
        public int magiShieldCd = 0;
        public override void PreUpdate()
        {
            
            if (crSky > 0)
            {
                crSky--;
            }
            if (llSky > 0)
            {
                llSky--;
            }

        }
        public List<Vector2> daPoints = new List<Vector2>();
        public Vector2 daLastP = Vector2.Zero;
        public override void PostUpdateRunSpeeds()
        {
            if (Player.Entropy().inspirationCard)
            {
                Player.runAcceleration *= 1.2f;
                Player.maxRunSpeed *= 1.2f;
            }
            if (Player.Entropy().oracleDeck)
            {
                Player.runAcceleration *= 1.3f;
                Player.maxRunSpeed *= 1.3f;
            }
            if (MagiShield > 0)
            {
                Player.runAcceleration *= 1.1f;
                Player.maxRunSpeed *= 1.1f;
            }
            if (VFSet)
            {
                Player.runAcceleration *= 1.2f;
                Player.maxRunSpeed *= 1.2f;
            }
            if (VFHelmRouge)
            {
                Player.runAcceleration *= 1.15f;
                Player.maxRunSpeed *= 1.15f;
            }
            Player.runAcceleration *= 1f + VoidCharge;
            Player.maxRunSpeed *= 1f + VoidCharge;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Player.Entropy().wisdomCard)
            {
                Player.manaCost *= 0.6f;
            }
            if (Player.Entropy().oracleDeck)
            {
                Player.manaCost *= 0.4f;
            }
            if (MagiShield > 0)
            {
                Player.manaCost *= 0.9f;
            }
            if (VFHelmMagic)
            {
                Player.manaCost *= 0.75f;
            }
            if (GreedCard)
            {
                Player.GetDamage(DamageClass.Generic) += Player.maxMinions * 0.02f;
            }
            if (VoidInspire > 0)
            {
                Player.GetDamage(DamageClass.Generic) += 0.5f;
                Player.Calamity().infiniteFlight = true;
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            modifiers.ModifyHurtInfo += EPHurtModifier;
            float d = 1;
            if (Player.Entropy().enduranceCard)
            {
                d -= 0.18f;
            }
            if (Player.Entropy().oracleDeck)
            {
                d -= 0.2f;
            }
            if (Player.Entropy().VFHelmMelee)
            {
                d -= 0.1f;
            }
            modifiers.SourceDamage *= d;
        }
        
        

        private void EPHurtModifier(ref Player.HurtInfo info)
        {
            if (MagiShield > 0)
            {
                if (MagiShield >= info.Damage)
                {
                    MagiShield -= info.Damage;
                    info.Damage = 0;
                }
                else
                {
                    info.Damage -= MagiShield;
                    MagiShield = 0;
                    
                }
                if (MagiShield == 0)
                {
                    for(int i = 0; i < Player.buffType.Length; i++)
                    {
                        if (Player.buffTime[i] > 0 && Main.debuff[Player.buffType[i]])
                        {
                            Player.buffTime[i] = 0;
                        }
                    }
                    Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MoonlightShieldBreak>(), 8000, 0, Player.whoAmI);
                    Player.statMana = Player.statManaMax2;
                }
            }
        }

        public int OracleDeskHealCd = 0;
        public int effectCount = 0;
        public int shielddamagecd = 0;
        
        public override void PostUpdate()
        {
            vfcd--;
            if (VoidInspire > 0)
            {
                Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                VoidInspire--;
                if (VoidInspire == 0)
                {
                    VoidCharge = 0;
                }
            }
            if (VoidCharge >= 1 && CalamityKeybinds.ArmorSetBonusHotKey.JustPressed)
            {
                if (VoidInspire <= 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Sounds/OmegaBlueAbility"), Player.Center);
                    VoidInspire = 600;
                    Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<VoidWraith>(), 2000, 0, Player.whoAmI);
                }
            }
            
            if (!VFSet)
            {
                if (VoidCharge > 0)
                {
                    VoidCharge = 0;
                }
            }
            if (VoidCharge > 1)
            {
                VoidCharge = 1;
            }
            if (pot_time > 0)
            {
                pot_time--;
            }
            else
            {
                pot_amp = 0;
            }
            effectCount += 1;
            if (HMRegenCd > 0)
            {
                HMRegenCd -= 1;
            }
            if (MagiShield > 0 && shielddamagecd <= 0)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && n.getRect().Intersects(Player.getRect()) && !n.dontTakeDamage)
                    {
                        NPC.HitInfo h = n.CalculateHitInfo(MagiShield, 0, false, 5);
                        Player.StrikeNPCDirect(n, h);
                        shielddamagecd = 30;
                    }
                }
            }
            shielddamagecd--;
            if (holyMoonlight)
            {
                if (magiShieldCd > 0)
                {
                    magiShieldCd -= 1;
                }
                else
                {
                    int magiShieldAddCount = (int)(Player.statManaMax2 * 0.75f);
                    magiShieldCd = 30 * 60;
                    if (MagiShield < magiShieldAddCount)
                    {
                        MagiShield = magiShieldAddCount;
                        MagiShieldMax = magiShieldAddCount;
                        localPlayerMaxShield = magiShieldAddCount;
                    }
                }
                if (Player.Calamity().cooldowns.TryGetValue(MoonlightShield.ID, out var value4))
                {
                    value4.timeLeft = MagiShieldMax - MagiShield;
                    
                }
                else
                {
                    Player.AddCooldown(MoonlightShield.ID, MagiShieldMax);
                }
            }
            else
            {
                MagiShield = 0;

            }
            if (MagiShield <= 0)
            {
                if (Player.Calamity().cooldowns.TryGetValue(MoonlightShield.ID, out var value4))
                {
                    for (int i = 0; i < Player.Calamity().cooldowns.Count; i++)
                    {
                        Player.Calamity().cooldowns.Remove(MoonlightShield.ID);
                    }
                }
            }
            List<Point> EdgeTiles = new List<Point>();
            Collision.GetEntityEdgeTiles(EdgeTiles, Player);
            foreach (Point touchedTile in EdgeTiles)
            {
                Tile tile = Framing.GetTileSafely(touchedTile);
                if (!tile.HasTile || !tile.HasUnactuatedTile)
                    continue;
                float auricRejectionKB = Player.noKnockback ? 20f : 40f;
                if (tile.TileType == ModContent.TileType<AuricBoulderTile>())
                {
                    // Cut grappling hooks so the player is surely thrown
                    Player.RemoveAllGrapplingHooks();


                    var yeetVec = Vector2.Normalize(Player.Center - touchedTile.ToWorldCoordinates());
                    Player.velocity += yeetVec * auricRejectionKB;
                    Player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AuricRejection").Format(Player.name)), 460, 0);
                    Player.AddBuff(BuffID.Electrified, 300);
                    
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/TeslaShoot1"));
                }
            }
            if (VaMoving > 0)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.Hitbox.Intersects(Player.Hitbox) && n.active)
                    {
                        Player.ApplyDamageToNPC(n, (int)Player.GetTotalDamage(DamageClass.Melee).ApplyTo(1200), 0, 0, false, Util.CUtil.rougeDC);

                    }
                }
                daPoints.Add(Player.Center - Player.velocity * 0.5f);
                daPoints.Add(Player.Center);
            }
            VaMoving--;
            if (Player.Entropy().oracleDeck) {
                if (OracleDeskHealCd <= 0)
                {
                    OracleDeskHealCd = 300;
                    if (Util.Util.getDistance(Main.LocalPlayer.Center, Player.Center) < 1600)
                    {
                        Main.LocalPlayer.Heal(30);
                    }
                }
            }
            if (OracleDeskHealCd > 0)
            {
                OracleDeskHealCd--;
            }
            if (Player.whoAmI == Main.myPlayer)
            {
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<VoidEcho>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<VoidEchoProj>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<VoidEchoProj>(), Player.HeldItem.damage, 0, Player.whoAmI);
                    }
                }
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<GhostdomWhisper>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<GhostdomWhisperHoldout>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<GhostdomWhisperHoldout>(), Player.HeldItem.damage, 0, Player.whoAmI);
                    }
                }
            }
            if (Player.HasBuff(ModContent.BuffType<StealthState>()))
            {
            }
            else
            {

                if (DarkArtsTarget.Count > 0)
                {
                    Player.velocity *= 0;
                    if (daCd <= 0)
                    {
                        daCd = 4;

                        if (DarkArtsTarget[0] is NPC npc)
                        {
                            npc.Entropy().daTarget = false;
                            int od = npc.defense;
                            npc.defense = 0;
                            float or = npc.Calamity().DR;
                            npc.Calamity().DR = 0;
                            Player.ApplyDamageToNPC(npc, (int)Player.GetTotalDamage(Util.CUtil.rougeDC).ApplyTo(460 + 50 * daCount), 0, 0, false, Util.CUtil.rougeDC);
                            npc.defense = od;
                            npc.Calamity().DR = or;
                            daLastP = npc.Center;
                        }
                        else
                        {
                            if (DarkArtsTarget[0] is Projectile proj)
                            {
                                proj.Entropy().daTarget = false;
                                proj.Kill();
                                daLastP = proj.Center;
                            }
                        }
                        SoundEffect se = ModContent.Request<SoundEffect>("CalamityEntropy/Sounds/da3").Value;
                        if (se != null) { se.Play(Main.soundVolume, 0, 0); }
                        daCount++;
                        if (DarkArtsTarget.Count > 0)
                            DarkArtsTarget.RemoveAt(0);
                        if (DarkArtsTarget.Count > 0)
                        {
                            daPoints.Add(Vector2.Lerp(daLastP, DarkArtsTarget[0].Center, (float)(4 - daCd) / 2f));

                        }
                        if (DarkArtsTarget.Count == 0)
                        {
                            foreach (NPC n in Main.npc)
                            {
                                if (Util.Util.getDistance(n.Center, Player.Center) < 140)
                                {
                                    int od = n.defense;
                                    n.defense = 0;
                                    float or = n.Calamity().DR;
                                    n.Calamity().DR = 0;
                                    Player.ApplyDamageToNPC(n, (int)Player.GetTotalDamage(Util.CUtil.rougeDC).ApplyTo(460 + 50 * (2 + daCount)), 0, 0, false, Util.CUtil.rougeDC);
                                    n.defense = od;
                                    n.Calamity().DR = or;
                                }
                            }
                            daCount = 0;
                            daPoints.Add(Vector2.Lerp(daLastP,
                                Player.Center, 0.2f));
                            daPoints.Add(Vector2.Lerp(daLastP,
                                                            Player.Center, 0.4f));
                            daPoints.Add(Vector2.Lerp(daLastP,
                                                            Player.Center, 0.6f));
                            daPoints.Add(Vector2.Lerp(daLastP,
                                                            Player.Center, 0.8f));
                            daPoints.Add(Vector2.Lerp(daLastP,
                                Player.Center, 1f));

                        }
                    }
                }

            }
            if (daCd > 0)
            {
                daCd--;
            }
            if (DarkArtsTarget.Count == 0 || daPoints.Count > 60)
            {
                if (daPoints.Count > 0)
                    daPoints.RemoveAt(0);
            }
        }
        public int daCd = 0;
        public int daCount = 0;
        public List<Entity> DarkArtsTarget = new List<Entity>();
        public int VaMoving = 0;
        public int twinSpawnIndex = -1;
        private int MagiShieldMax = 1;

        public override void ModifyScreenPosition()
        {
            var shaker = Main.rand;
            Main.screenPosition += new Vector2(shaker.Next(-CalamityEntropy.Instance.screenShakeAmp * 8, CalamityEntropy.Instance.screenShakeAmp * 8 + 1), shaker.Next(-CalamityEntropy.Instance.screenShakeAmp, CalamityEntropy.Instance.screenShakeAmp + 1));
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff(ModContent.BuffType<StealthState>()) || DarkArtsTarget.Count > 0)
            {
                a = 0.4f;
                r = 0.2f;
                g = 0.2f;
                b = 0.2f;
            }
        }
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<RuneSongProj>()] > 0)
            {
                
            }
        }
        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            health.Base = CruiserLoreUsed.ToInt() * CruiserLore.LifeBoost;

            mana = StatModifier.Default;
            mana.Base = CruiserLoreUsed.ToInt() * CruiserLore.ManaBoost;
        }
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (Player.HasBuff(ModContent.BuffType<StealthState>()) || DarkArtsTarget.Count > 0 || VaMoving > 0)
            {
                return false;
            }
            return base.CanBeHitByNPC(npc, ref cooldownSlot);
        }
        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (Player.HasBuff(ModContent.BuffType<StealthState>()) || DarkArtsTarget.Count > 0 || VaMoving > 0)
            {
                return false;
            }
            return base.CanBeHitByProjectile(proj);
        }
        
        public override void Initialize()
        {
            CruiserLoreUsed = false;
        }


        public override void SaveData(TagCompound tag)
        {
            var boost = new List<string>();
            boost.AddWithCondition("CruiserLore", CruiserLoreUsed);
            tag["EntropyBoosts"] = boost;
        }

        public override void LoadData(TagCompound tag)
        {
            var boost = tag.GetList<string>("EntropyBoosts");
            CruiserLoreUsed = boost.Contains("CruiserLore");
        }
    }
}