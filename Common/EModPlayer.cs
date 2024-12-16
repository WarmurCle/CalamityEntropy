using System.Collections.Generic;
using CalamityEntropy.Content.BeesGame;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.HBProj;
using CalamityEntropy.Content.Projectiles.SamsaraCasket;
using CalamityEntropy.Content.Projectiles.VoidEchoProj;
using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Common
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
        public bool hasSM = false;
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
        public bool ArchmagesMirror = false;
        public float damageReduce = 1;
        public float moveSpeed = 0;
        public float ManaCost = 1;
        public float Thorn = 0;
        public float WingSpeed = 1;
        public bool GodHeadVisual = false;
        public bool samsaraCasketOpened = false;
        public int sCasketLevel = 0;
        public bool AWraith = false;
        public int SacredJudgeShields = 2;
        public float screenShift = 0;
        public bool holyMantle = false;
        public int mantleCd = 0;
        public bool HolyShield = false;
        public Vector2 screenPos = Vector2.Zero;
        public int WeaponBoost = 0;
        public bool CrPlush = false;
        public float CasketSwordRot { get { return (float)effectCount * 0.12f; } }
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

        public bool CRing = false;
        public bool LastStand = false;
        public int lastStandCd = 0;

        

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if(immune > 0)
            {
                if(Player.statLife < 6)
                {
                    Player.statLife = 6;
                }
                return false;
            }
            if(SacredJudgeShields > 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0)
            {
                SacredJudgeShields -= 1;
                if (Player.statLife < 6)
                {
                    Player.statLife = 6;
                }
                immune = 120;
                return false;
            }
            if (LastStand && lastStandCd <= 0)
            {
                playSound = false;
                genDust = false;
                lastStandCd = 4000;
                Player.Heal(100);
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/holyshield_shatter") { Volume = 0.6f }, Player.Center);
                return false;
            }
            
            return true;
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            voidcharge = 0;VoidInspire = 0;lastStandCd = 0;mantleCd = 0;magiShieldCd = 0;sJudgeCd = 2;
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (SCrown)
            {
                Player.Calamity().nextHitDealsDefenseDamage = false;
            }
            if (Thorn > 0)
            {
                Player.ApplyDamageToNPC(npc, (int)(hurtInfo.Damage * Thorn), 0, 0, false);
            }
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (SCrown)
            {
                Player.Calamity().nextHitDealsDefenseDamage = false;
            }
        }
        public bool SCrown;

        public int VoidInspire = 0;

        public bool GreedCard = false;
        public int lifeRegenPerSec = 0;
        public int lifeRegenCD = 60;
        public float light = 0;
        public float AttackVoidTouch = 0;
        public float DebuffImmuneChance = 0;
        public float shootSpeed = 1;
        public float enhancedMana = 0;
        public bool sacrMask = false;
        public override void ResetEffects()
        {
            sacrMask = false;
            GodHeadVisual = true;
            shootSpeed = 1;
            WeaponBoost = 0;
            Thorn = 0;
            WingSpeed = 1;
            AttackVoidTouch = 0;
            light = 0;
            CRing = false;
            lifeRegenPerSec = 0;
            Godhead = false;
            ManaCost = 1;
            auraCard = false;
            LastStand = false;
            holyMantle = false;
            CrPlush = false;
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
            SCrown = false;
            GreedCard = false;
            enhancedMana = 0;
            ArchmagesMirror = false;
            damageReduce = 1;
            moveSpeed = 0;
            DebuffImmuneChance = 0;
        }
        public int crSky = 0;
        public int llSky = 0;
        public int magiShieldCd = 0;
        public int sJudgeCd = 30 * 60;
        public override void FrameEffects()
        {
            if (CrPlush)
            {
                Player.head = EquipLoader.GetEquipSlot(base.Mod, "CruiserPlush", EquipType.Head);
            }
        }
        public override void PreUpdate()
        {
            if (HolyShield)
            {
                mantleCd = HolyMantle.Cooldown;
            }
            else
            {
                mantleCd--;
            }
            if(!HolyShield && holyMantle)
            {
                if(mantleCd <= 0) {
                    mantleCd = HolyMantle.Cooldown;
                    HolyShield = true;
                }
            }
            screenShift = screenShift + (0 - screenShift) * 0.06f;
            if (immune > 0)
            {
                Player.immune = true;
                Player.immuneTime = immune;
                immune--;
            }
            if(SacredJudgeShields < 2)
            {
                sJudgeCd -= 1;
                if(sJudgeCd <= 0)
                {
                    sJudgeCd = 30 * 60;
                    SacredJudgeShields += 1;
                }
            }
            lastStandCd--;
            Lighting.AddLight(Player.Center, light, light, light);
            bool sm = Player.HasBuff(ModContent.BuffType<SoyMilkBuff>());
            if (hasSM && !sm)
            {
                foreach(Projectile p in Main.projectile)
                {
                    if (p.active && p.owner == Player.whoAmI)
                    {
                        p.Kill();
                    }
                }
            }
            hasSM = sm;
            if (crSky > 0)
            {
                crSky--;
            }
            if (llSky > 0)
            {
                llSky--;
            }
            if(VFSet && !Player.controlLeft && !Player.controlRight)
            {
                Player.velocity.X *= 0.92f;
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
            if (CRing)
            {
                Player.runAcceleration *= 1.05f;
                Player.maxRunSpeed *= 1.05f;
            }
            Player.runAcceleration *= 1f + VoidCharge;
            Player.maxRunSpeed *= 1f + VoidCharge;
            Player.runAcceleration *= 1f + moveSpeed;
            Player.maxRunSpeed *= 1f + moveSpeed;
        }
        public int scHealCD = 60;
        public override void PostUpdateMiscEffects()
        {
            Player.manaCost *= ManaCost;
            if (Player.Entropy().SCrown)
            {
                Player.Calamity().defenseDamageRatio = 0;
            }
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
            manaNorm = Player.statManaMax2;
            if (ArchmagesMirror)
            {
                enhancedMana += 0.25f;
            }
            Player.statManaMax2 += (int)(Player.statManaMax2 * enhancedMana);
            if (Player.statMana > manaNorm)
            {
                Player.GetDamage(DamageClass.Magic) += 0.25f;
            }

        }
        public int manaNorm = 0;
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {

            modifiers.ModifyHurtInfo += EPHurtModifier;
            float d = 2 - damageReduce;
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
        public int immune = 0;

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if(immune > 0)
            {
                return true;
            }
            
            if (HolyShield && info.Damage * (2 - damageReduce) - Player.statDefense > 16)
            {
                immune = 120;
                HolyShield = false;
                
                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);
                if (holyMantle)
                {
                    if (info.Damage * (2 - damageReduce) - Player.statDefense < 60)
                    {
                        mantleCd = 6 * 60;
                        Player.AddCooldown("HolyMantleCooldown", mantleCd, true);
                    }
                    else
                    {
                        Player.AddCooldown("HolyMantleCooldown", HolyMantle.Cooldown, true);
                    }
                }
                return true;
            }
            if (info.Damage * (2 - damageReduce) - Player.statDefense > 16)
            {
                if (SacredJudgeShields > 0 && info.Damage * (2 - damageReduce) - Player.statDefense < 80 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0)
                {
                    immune = 120;
                    SacredJudgeShields -= 1;
                    Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);
                    return true;
                }
                if (SacredJudgeShields > 1 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0)
                {
                    immune = 120;
                    SacredJudgeShields -= 2;
                    Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);

                    return true;
                }
            }
            return false;
        }


        private void EPHurtModifier(ref Player.HurtInfo info)
        {
            if(SacredJudgeShields > 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0)
            {
                SacredJudgeShields--;
                info.Damage -= 80;
                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);

                immune = 120;
            }
            if (SCrown)
            {
                Player.Calamity().defenseDamageRatio = 0f;
            }
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

        public bool VSoundsPlayed = false;
        public override void PostUpdate()
        {
            if (AWraith)
            {
                Player.ManageSpecialBiomeVisuals("HeatDistortion", true);
            }
            AWraith = false;
            scHealCD--;
            if (scHealCD < 0 && Player.statLife < Player.statLifeMax2 && SCrown)
            {
                scHealCD = 60;
                Player.Heal(8);
            }
            if (Player.statLife < Player.statLifeMax2 && lifeRegenPerSec > 0)
            {
                lifeRegenCD--;
                if (lifeRegenCD <= 0)
                {
                    lifeRegenCD = 60;
                    Player.Heal(lifeRegenPerSec);
                }
            }
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
            else
            {
                if (!VSoundsPlayed && voidcharge >= 1) {
                    VSoundsPlayed = true;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/PhantomHeartUse"));
                }
            }
            if (VoidInspire <= 0 && voidcharge < 1)
            {
                VSoundsPlayed = false;
            }
            if (VoidCharge >= 1 && CalamityKeybinds.ArmorSetBonusHotKey.JustPressed)
            {
                if (VoidInspire <= 0)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/OmegaBlueAbility"), Player.Center);
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
                if (MagiShield > 0)
                {
                    magiShieldCd = 30 * 60;
                }
                if (magiShieldCd > 0)
                {
                    if (MagiShield <= 0)
                    {
                        magiShieldCd -= 1;
                    }
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
                        if (Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2)
                        {
                            Main.LocalPlayer.Heal(30);
                        }
                    }
                }
            }
            if (OracleDeskHealCd > 0)
            {
                OracleDeskHealCd--;
            }
            if (Player.whoAmI == Main.myPlayer && !Player.HasBuff(ModContent.BuffType<NOU>()))
            {
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<VoidEcho>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<VoidEchoProj>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<VoidEchoProj>(), Player.HeldItem.damage, 0, Player.whoAmI);
                    }
                }
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<Mercy>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<HB>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<HB>(), Player.HeldItem.damage, 0, Player.whoAmI);
                    }
                }
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<GhostdomWhisper>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<GhostdomWhisperHoldout>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<GhostdomWhisperHoldout>(), Player.HeldItem.damage, 0, Player.whoAmI);
                    }
                }
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<RailPulseBow>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<RailPulseBowProjectile>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<RailPulseBowProjectile>(), Player.HeldItem.damage, 0, Player.whoAmI);
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
                        SoundEffect se = ModContent.Request<SoundEffect>("CalamityEntropy/Assets/Sounds/da3").Value;
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
            Main.screenPosition = Vector2.Lerp(Main.screenPosition, screenPos - Main.ScreenSize.ToVector2() / 2, screenShift);

            var shaker = Main.rand;
            Main.screenPosition += new Vector2(shaker.Next((int)-CalamityEntropy.Instance.screenShakeAmp * 8, (int)CalamityEntropy.Instance.screenShakeAmp * 8 + 1), shaker.Next((int)-CalamityEntropy.Instance.screenShakeAmp, (int)CalamityEntropy.Instance.screenShakeAmp + 1));
        
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
            if (Player.HasBuff(ModContent.BuffType<StealthState>()) || DarkArtsTarget.Count > 0 || VaMoving > 0 || proj.Entropy().vdtype >= 0 || proj.ModProjectile is GodSlayerRocketProjectile)
            {
                return false;
            }
            return base.CanBeHitByProjectile(proj);
        }

        public override void SetControls()
        {
            if (BeeGame.Active)
            {
                Player.controlDown = false;
                Player.controlJump = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlUp = false;
            }
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