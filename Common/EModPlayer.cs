using CalamityEntropy.Content.BeesGame;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.DimDungeon;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Accessories.EvilCards;
using CalamityEntropy.Content.Items.Armor.Marivinium;
using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.HBProj;
using CalamityEntropy.Content.Projectiles.SamsaraCasket;
using CalamityEntropy.Content.Projectiles.VoidEchoProj;
using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Content.UI;
using CalamityEntropy.Content.UI.Poops;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Audio;
using SubworldLibrary;
using System;
using System.Collections.Generic;
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
        public int itemTime = 0;
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
        public bool oracleDeckInInv = false;
        public bool taintedDeckInInv = false;
        public bool hasSM = false;
        public bool summonerVF;
        public bool magiVF;
        public bool rogueVF;
        public bool meleeVF;
        public bool rangerVF;
        public bool VFSet;
        public bool VFLeg;
        public bool VFHelmRanged;
        public bool VFHelmMagic;
        public bool VFHelmSummoner;
        public bool VFHelmRogue;
        public bool VFHelmMelee;
        public bool mariviniumBody = false;
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
        public bool brokenAnkh = false;
        public bool reincarnationBadge = false;
        public bool nihShell = false;
        public List<Poop> poops = new List<Poop>();
        public int MaxPoops = 19;
        public Poop PoopHold = null;
        public bool _holdingPoop;
        public int holyGroundTime = 0;
        public float dodgeChance = 0;
        public bool mawOfVoid = false;
        public float serviceWhipDamageBonus = 0;
        public bool deusCore = false;
        public bool heartOfStorm = false;
        public int ffinderCd = 0;
        public float temporaryArmor = 0;
        public IEnumerable<KeyValuePair<string, Vector2>> homes = new Dictionary<string, Vector2>();
        public bool holdingPoop { get { return _holdingPoop; } set { if (Player.whoAmI == Main.myPlayer && value != _holdingPoop) { syncHoldingPoop = true; } _holdingPoop = value; } }
        public float CasketSwordRot { get { return (float)effectCount * 0.12f; } }
        public float VoidCharge
        {
            get { return voidcharge; }
            set
            {
                if (value < voidcharge)
                {
                    voidcharge = value;
                }
                else if (vfcd <= 0)
                {
                    voidcharge = value; vfcd = 8;
                }
            }
        }
        public bool CRing = false;
        public bool LastStand = false;
        public int lastStandCd = 0;

        public bool accWispLantern = false;
        public bool visualWispLantern = false;

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            deusCoreBloodOut = 0;
            if (immune > 0)
            {
                if (Player.statLife < 6)
                {
                    Player.statLife = 6;
                }
                return false;
            }
            if (SacredJudgeShields > 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0)
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
            voidcharge = 0; VoidInspire = 0; lastStandCd = 0; mantleCd = 0; magiShieldCd = 0; sJudgeCd = 2;
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
        public bool FrailCard = false;
        public bool BarrenCard = false;
        public bool TarnishCard = false;
        public bool ConfuseCard = false;
        public bool PerplexedCard = false;
        public bool SacrificeCard = false;
        public bool NothingCard = false;
        public bool FoolCard = false;
        public bool EvilDeck = false;

        public int lifeRegenPerSec = 0;
        public int lifeRegenCD = 60;
        public float light = 0;
        public float AttackVoidTouch = 0;
        public float DebuffImmuneChance = 0;
        public float shootSpeed = 1;
        public float enhancedMana = 0;
        public bool sacrMask = false;
        public int voidshadeBoostTime = 0;
        public float mawOfVoidCharge = 0;
        public bool mawOfVoidUsing = false;

        public bool revelation = false;
        public float revelationCharge = 0;
        public bool revelationUsing = false;
        public int deusCoreBloodOut = 0;
        public int summonCrit = 0;
        public float meleeDamageReduce = 0;
        public bool isUsingItem()
        {
            return Main.mouseLeft && !Player.mouseInterface && Player.HeldItem.damage > 0 && Player.HeldItem.active;
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage *= (1 - meleeDamageReduce);
        }
        public bool MariviniumSet = false;
        public override void ResetEffects()
        {
            visualMagiShield = false;
            MariviniumSet = false;
            meleeDamageReduce = 0;
            deusCore = false;
            heartOfStorm = false;
            summonCrit = 0;
            mawOfVoid = false;
            revelation = false;
            wyrmPhantom = false;
            cHat = false;
            dodgeChance = 0;
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
            brokenAnkh = false;
            holyMantle = false;
            nihShell = false;
            mariviniumBody = false;
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
            GreedCard = false;
            FrailCard = false;
            BarrenCard = false;
            TarnishCard = false;
            ConfuseCard = false;
            PerplexedCard = false;
            SacrificeCard = false;
            NothingCard = false;
            FoolCard = false;
            EvilDeck = false;
            holyMoonlight = false;
            oracleDeckInInv = false;
            taintedDeckInInv = false;
            summonerVF = false;
            magiVF = false;
            rogueVF = false;
            meleeVF = false;
            rangerVF = false;
            VFSet = false;
            VFLeg = false;
            VFHelmRanged = false;
            VFHelmMagic = false;
            VFHelmSummoner = false;
            VFHelmRogue = false;
            VFHelmMelee = false;
            SCrown = false;
            GreedCard = false;
            enhancedMana = 0;
            ArchmagesMirror = false;
            damageReduce = 1;
            moveSpeed = 0;
            DebuffImmuneChance = 0;
            reincarnationBadge = false;
            visualWispLantern = false;
            accWispLantern = false;
        }
        public int crSky = 0;
        public int NihSky = 0;
        public int llSky = 0;
        public int magiShieldCd = 0;
        public int sJudgeCd = 30 * 60;
        public float rBadgeCharge = 12;
        public int nihShellCount = 0;
        public int nihShellCd = 0;
        public override void FrameEffects()
        {
            if (CrPlush)
            {
                Player.head = EquipLoader.GetEquipSlot(base.Mod, "CruiserPlush", EquipType.Head);
            }
        }
        public int BlackFlameCd = 0;
        public bool rBadgeActive = false;
        public bool[] tileSolid = null;
        public bool[] solidTop = null;
        public bool[] tilePlatform = null;
        public bool cUp = false;
        public bool cDown = false;
        public bool cLeft = false;
        public bool cRight = false;
        public float rbDotDist = 0;
        public override void PreUpdate()
        {
            if (Player.HeldItem.ModItem is EntropyBook eb)
            {
                eb.CheckSpawn(Player);
            }
            temporaryArmor *= 0.994f;
            if (temporaryArmor > 0)
            {
                temporaryArmor -= 0.002f;
            }
            if (temporaryArmor < 0)
            {
                temporaryArmor = 0;
            }
            ffinderCd--;
            if (syncHoldingPoop)
            {
                syncHoldingPoop = false;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)CEMessageType.PoopSync);
                    packet.Write(Player.whoAmI);
                    packet.Write(holdingPoop);
                    packet.Send();
                }
            }
            if (Player.dead) { return; }
            if (rBadgeActive)
            {
                Player.maxFallSpeed = 99;
                rbDotDist += (1 - rbDotDist) * 0.06f;
                Player.velocity *= 0f;
                float speed = 20f;
                if (cUp)
                {
                    Player.velocity.Y -= speed;
                }
                if (cDown)
                {
                    Player.velocity.Y += speed;
                }
                if (cLeft)
                {
                    Player.direction = -1;
                    Player.velocity.X -= speed;
                }
                if (cRight)
                {
                    Player.direction = 1;
                    Player.velocity.X += speed;
                }
                if (!Player.velocity.Equals(Vector2.Zero) && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket pack = Mod.GetPacket();
                    pack.Write((byte)CEMessageType.PlayerSetPos);
                    pack.Write(Player.whoAmI);
                    pack.WriteVector2(Player.Center);
                    pack.Send();
                }
                Player.velocity.Y += 0.0001f;
                resetTileSets = true;
                tileSolid = (bool[])Main.tileSolid.Clone();
                solidTop = (bool[])Main.tileSolidTop.Clone();
                tilePlatform = (bool[])TileID.Sets.Platforms.Clone();
                for (int type = 0; type < TileID.Sets.Platforms.Length; type++)
                {
                    if (TileID.Sets.Platforms[type])
                    {
                        Main.tileSolid[type] = false;
                        Main.tileSolidTop[type] = false;
                        TileID.Sets.Platforms[type] = false;
                    }
                }
            }
            else
            {
                rbDotDist += (-rbDotDist) * 0.06f;
            }
            if (BlackFlameCd > 0)
            {
                BlackFlameCd--;
            }
            if (TarnishCard && Main.myPlayer == Player.whoAmI)
            {
                if (Player.channel)
                {
                    if (BlackFlameCd < 1)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.One) * 14, ModContent.ProjectileType<BlackFire>(), Player.GetWeaponDamage(Player.HeldItem) / 6 + 1, 2, Player.whoAmI);
                        BlackFlameCd = 60;
                    }
                }
            }
            if (voidshadeBoostTime > 0)
            {
                voidshadeBoostTime--;
            }
            if (HolyShield)
            {
                mantleCd = HolyMantle.Cooldown;
            }
            else
            {
                mantleCd--;
            }
            if (!HolyShield && holyMantle)
            {
                if (mantleCd <= 0)
                {
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
            if (SacredJudgeShields < 2)
            {
                sJudgeCd -= 1;
                if (sJudgeCd <= 0)
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
                foreach (Projectile p in Main.projectile)
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
            if (NihSky > 0)
            {
                NihSky--;
            }
            if (llSky > 0)
            {
                llSky--;
            }
            if (VFLeg && !Player.controlLeft && !Player.controlRight)
            {
                Player.velocity.X *= 0.96f;
            }

        }
        public List<Vector2> daPoints = new List<Vector2>();
        public Vector2 daLastP = Vector2.Zero;

        public override void PostUpdateRunSpeeds()
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<WyrmDash>()] > 0)
            {
                Player.maxFallSpeed = 9999;
            }
            if (Player.Entropy().inspirationCard)
            {
                Player.runAcceleration *= 1.10f;
                Player.maxRunSpeed *= 1.2f;
            }
            if (Player.Entropy().oracleDeck)
            {
                Player.runAcceleration *= 1.10f;
                Player.maxRunSpeed *= 1.15f;
            }
            if (MagiShield > 0)
            {
                Player.runAcceleration *= 1f;
                Player.maxRunSpeed *= 1.1f;
            }
            if (VFSet)
            {
                Player.maxRunSpeed *= 1.05f;
            }
            if (VFHelmRogue)
            {
                Player.runAcceleration *= 0.50f;
                Player.maxRunSpeed *= 1.12f;
            }
            if (CRing)
            {
                Player.runAcceleration *= 1.05f;
                Player.maxRunSpeed *= 1.05f;
            }
            Player.runAcceleration *= 1f + 0.10f * VoidCharge;
            Player.maxRunSpeed *= 1f + 0.30f * VoidCharge;
            Player.runAcceleration *= 1f + moveSpeed;
            Player.maxRunSpeed *= 1f + moveSpeed;
            if (CalamityEntropy.EntropyMode)
            {
                Player.maxRunSpeed *= 0.96f;
                if (HitTCounter > 0)
                {
                    Player.maxRunSpeed *= 0.96f;
                }
            }
        }
        public int scHealCD = 60;
        public int HitTCounter = 0;

        public override void PostUpdateMiscEffects()
        {
            if (SubworldSystem.IsActive<VOIDSubworld>())
            {
                Player.gravity = 0;
            }
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.ModProjectile is JewelSapphire)
                {
                    if (p.Distance(Player.Center) <= 130)
                    {
                        Player.lifeRegen += 16;
                        Player.endurance += 0.1f;
                        Player.GetDamage(DamageClass.Generic) += 0.15f;
                        Player.GetAttackSpeed(DamageClass.Generic) += 0.15f;
                    }
                }
            }
            float d = damageReduce - 1;
            if (Player.Entropy().enduranceCard)
            {
                d += 0.05f;
            }
            if (Player.Entropy().oracleDeck)
            {
                d += 0.10f;
            }
            if (Player.Entropy().VFHelmMelee)
            {
                d += 0.05f;
            }
            Player.endurance += d;
            if (rBadgeActive)
            {
                Player.gravity = 0;
            }

            manaNorm = Player.statManaMax2;
            Player.statManaMax2 += (int)(Player.statManaMax2 * enhancedMana);
            if (Player.statMana > manaNorm)
            {
                Player.GetDamage(DamageClass.Magic) += (Player.statMana - manaNorm) * 0.0025f;
            }
            if (CalamityEntropy.EntropyMode)
            {
                Player.statLifeMax2 = (int)(Player.statLifeMax2 * 0.8f);
                Player.statDefense *= 0.7f;
                Player.lifeRegen /= 2;
                lifeRegenPerSec /= 2;
                if (HitTCounter > 0)
                {
                    Player.lifeRegen = 0;
                    lifeRegenPerSec = 0;
                }
                List<DamageClass> dmgClasses = new List<DamageClass>() { ModContent.GetInstance<AverageDamageClass>(), ModContent.GetInstance<DefaultDamageClass>(), ModContent.GetInstance<GenericDamageClass>(), ModContent.GetInstance<MagicDamageClass>(), ModContent.GetInstance<MagicSummonHybridDamageClass>(), ModContent.GetInstance<MeleeDamageClass>(), ModContent.GetInstance<MeleeNoSpeedDamageClass>(), ModContent.GetInstance<MeleeRangedHybridDamageClass>(), ModContent.GetInstance<NoneTypeDamageClass>(), ModContent.GetInstance<RangedDamageClass>(), ModContent.GetInstance<RogueDamageClass>(), ModContent.GetInstance<StealthDamageClass>(), ModContent.GetInstance<SummonDamageClass>(), ModContent.GetInstance<SummonMeleeSpeedDamageClass>(), ModContent.GetInstance<ThrowingDamageClass>(), ModContent.GetInstance<TrueMeleeDamageClass>(), ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>() };
                for (int i = 0; i < dmgClasses.Count; i++)
                {
                    DamageClass dc = dmgClasses[i];
                    if (Player.GetDamage(dc).Additive > 1)
                    {
                        float tv = (float)Math.Pow(Player.GetDamage(dc).Additive, 0.8f);
                        Player.GetDamage(dc) -= (Player.GetDamage(dc).Additive - tv);
                    }
                    if (Player.GetCritChance(dc) > 50)
                    {
                        Player.GetCritChance(dc) = 50;
                    }
                }
            }
            Player.statDefense += (int)temporaryArmor;
            HitTCounter--;
        }
        public int manaNorm = 0;
        public int deusCoreAdd = 0;
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            deusCoreAdd = 0;
            modifiers.ModifyHurtInfo += EPHurtModifier;
        }
        public int immune = 0;
        public bool cHat = false;
        public override void OnHurt(Player.HurtInfo info)
        {
            HitTCounter = 300;
            Player.immuneTime = (int)(Player.immuneTime * 0.5f);
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (immune > 0)
            {
                deusCoreBloodOut -= deusCoreAdd;
                return true;
            }
            if (dodgeChance > Main.rand.NextDouble())
            {
                deusCoreBloodOut -= deusCoreAdd;
                return true;
            }
            return false;
        }
        public bool PetsHat { get { return cHat || DateTime.Now.Month == 12; } }
        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (noCsDodge)
            {
                return false;
            }
            if (Main.rand.NextDouble() < dodgeChance)
            {
                deusCoreBloodOut -= deusCoreAdd;
                immune = 60;
                return true;
            }
            if (info.Damage > Player.statLifeMax2 / 20 && MariviniumShieldCount > 0)
            {
                immune = 90;
                MariviniumShieldCount--;
                Player.Heal(140);
                Util.Util.PlaySound("crystalShieldBreak", 1, Player.Center, 1, 0.7f);
                Player.AddBuff(ModContent.BuffType<AbyssalWrath>(), 600);
                for (int i = 0; i < 42; i++)
                {
                    Dust.NewDust(Player.Center, 1, 1, DustID.BlueCrystalShard, Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6), Scale: 2);
                }
                return true;
            }
            if (HolyShield && info.Damage > 1)
            {
                immune = 120;
                HolyShield = false;

                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);
                if (holyMantle)
                {
                    Player.AddCooldown("HolyMantleCooldown", HolyMantle.Cooldown, true);
                }
                deusCoreBloodOut -= deusCoreAdd;
                return true;
            }
            if (info.Damage * (2 - damageReduce) - Player.statDefense > 16)
            {
                if (SacredJudgeShields > 0 && info.Damage < 80 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0)
                {
                    immune = 120;
                    SacredJudgeShields -= 1;
                    Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);
                    deusCoreBloodOut -= deusCoreAdd;
                    return true;
                }
                if (SacredJudgeShields > 1 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0)
                {
                    immune = 120;
                    SacredJudgeShields -= 2;
                    Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);
                    deusCoreBloodOut -= deusCoreAdd;
                    return true;
                }
            }
            return false;
        }

        public bool noCsDodge = false;
        private void EPHurtModifier(ref Player.HurtInfo info)
        {
            noCsDodge = false;
            if (SCrown)
            {
                Player.Calamity().defenseDamageRatio = 0f;
            }
            if (HolyShield)
            {
                return;
            }
            if (MariviniumShieldCount > 0)
            {
                return;
            }
            bool setToOne = false;
            if (!setToOne)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.ModProjectile is PoopWulfrumProjectile w && Util.Util.getDistance(Player.Center, p.Center) < PoopWulfrumProjectile.shieldDistance)
                    {
                        if (w.shield > 0)
                        {
                            if (w.shield > info.Damage)
                            {
                                w.shield -= info.Damage;
                                info.Damage = 0;
                                setToOne = true;
                                p.netUpdate = true;
                                noCsDodge = true;
                            }
                            else
                            {
                                info.Damage -= w.shield;
                                w.shield = 0;
                                p.netUpdate = true;
                            }
                        }
                    }
                }
            }
            if (!setToOne && SacredJudgeShields > 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<SacredJudge>()] > 0 && SacredJudgeShields < 2)
            {
                SacredJudgeShields--;
                info.Damage -= 80;
                Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, Player.whoAmI);
                immune = 120;
            }

            if (!setToOne && MagiShield > 0)
            {
                if (MagiShield >= info.Damage)
                {
                    MagiShield -= info.Damage;
                    info.Damage = 0;
                    noCsDodge = true;
                    setToOne = true;
                }
                else
                {
                    info.Damage -= MagiShield;
                    MagiShield = 0;
                }
                if (MagiShield == 0)
                {
                    for (int i = 0; i < Player.buffType.Length; i++)
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
            if (!setToOne)
            {
                if (info.Damage > 30)
                {
                    if (nihShellCount > 0)
                    {
                        nihShellCount--;
                        info.Damage = (int)(info.Damage * 0.85f);
                        Util.Util.PlaySound("shielddown", 1, Player.Center);
                        for (int i = 0; i < 24; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(Player.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(5, 9), Main.rand.NextFloat(0.8f, 2.4f), new Color(165, 58, 222), 40));
                        }
                    }
                }
            }
            if (deusCore && info.Damage > 1)
            {
                deusCoreBloodOut += info.Damage;
                deusCoreAdd = info.Damage;
                info.Damage = 0;
            }
        }

        public int OracleDeckHealCd = 0;
        public int effectCount = 0;
        public int shielddamagecd = 0;
        public int noItemTime = 0;
        public bool syncHoldingPoop = false;
        public int damageRecord = 0;


        public bool VSoundsPlayed = false;
        public override void PostUpdate()
        {
            if (CalamityEntropy.EntropyMode && HitTCounter > 0)
            {
                Player.AddBuff(ModContent.BuffType<NoHeal>(), HitTCounter);
            }
            if (MariviniumSet)
            {
                if (MariviniumShieldCount < MariviniumHelmet.MaxShield)
                {
                    MariviniumShieldCd--;
                    if (MariviniumShieldCd <= 0)
                    {
                        MariviniumShieldCd = MariviniumHelmet.ShieldCd;
                        MariviniumShieldCount++;
                    }
                }
                else
                {
                    MariviniumShieldCd = MariviniumHelmet.ShieldCd;
                }
            }
            else
            {
                MariviniumShieldCount = 0;
                MariviniumShieldCd = MariviniumHelmet.ShieldCd;

            }
            if (bloodTrCD > 0)
                bloodTrCD--;
            if (deusCoreBloodOut < 0)
                deusCoreBloodOut = 0;
            if (deusCoreBloodOut > 0 && Main.GameUpdateCount % 2 == 0)
            {
                int dmgApply = (int)(deusCoreBloodOut * 0.01f + 1);
                if (dmgApply > deusCoreBloodOut)
                {
                    dmgApply = deusCoreBloodOut;
                }
                Player.statLife -= dmgApply;
                if (Player.statLife < 1)
                {
                    Player.Hurt(PlayerDeathReason.ByCustomReason(Player.name + Mod.GetLocalization("KilledByAstral").Value), dmgApply, 0, false, true, 0, false, 0);
                }
                deusCoreBloodOut -= dmgApply;
            }
            itemTime--;
            if (Player.HasBuff<VoidVirus>())
            {
                Player.statDefense -= 30;
                Player.lifeRegen = 0;
                lifeRegenPerSec = 0;
            }
            if (VFHelmSummoner)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<VoidMonster>()] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<VoidMonster>(), (int)(Player.GetTotalDamage(DamageClass.Summon).ApplyTo(4080)), 4, Player.whoAmI);
                }
            }
            serviceWhipDamageBonus *= 0.995f;
            if (serviceWhipDamageBonus > 0.009f)
            {
                Player.ClearBuff(ModContent.BuffType<ServiceBuff>());
                Player.AddBuff(ModContent.BuffType<ServiceBuff>(), (int)(serviceWhipDamageBonus * 10000));
            }
            if (nihShellCd > 0)
            {
                nihShellCd--;
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                if (mawOfVoid)
                {
                    if (mawOfVoidUsing)
                    {
                        mawOfVoidCharge -= 1f / 60f;
                        if (mawOfVoidCharge <= 0)
                        {
                            mawOfVoidCharge = 0;
                            mawOfVoidUsing = false;
                        }
                    }
                    else
                    {
                        if (isUsingItem())
                        {
                            mawOfVoidCharge += 0.01f;
                            if (mawOfVoidCharge >= 1)
                            {
                                mawOfVoidCharge = 1;
                                if (Main.mouseRight)
                                {
                                    mawOfVoidUsing = true;
                                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<BloodRing>(), (int)Player.GetDamage(Player.GetBestClass()).ApplyTo(MawOfTheVoid.Damage), 0, Player.whoAmI);
                                }
                            }
                        }
                        else
                        {
                            if (mawOfVoidCharge == 1)
                            {
                                mawOfVoidUsing = true;
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<BloodRing>(), (int)Player.GetDamage(Player.GetBestClass()).ApplyTo(MawOfTheVoid.Damage), 0, Player.whoAmI);
                            }
                            else
                            {
                                mawOfVoidCharge -= 0.01f;
                                if (mawOfVoidCharge <= 0)
                                {
                                    mawOfVoidCharge = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    mawOfVoidCharge = 0;
                }
                if (revelation)
                {
                    if (revelationUsing)
                    {
                        revelationCharge -= 1f / 60f;
                        if (revelationCharge <= 0)
                        {
                            revelationCharge = 0;
                            revelationUsing = false;
                        }
                    }
                    else
                    {
                        if (isUsingItem())
                        {
                            revelationCharge += 0.01f;
                            if (revelationCharge >= 1)
                            {
                                revelationCharge = 1;
                                if (Main.mouseRight)
                                {
                                    revelationUsing = true;
                                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<HolyBeamRevelation>(), (int)Player.GetDamage(Player.GetBestClass()).ApplyTo(TheRevelation.Damage), 0, Player.whoAmI);
                                }
                            }
                        }
                        else
                        {
                            if (revelationCharge == 1)
                            {
                                revelationUsing = true;
                                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<HolyBeamRevelation>(), (int)Player.GetDamage(Player.GetBestClass()).ApplyTo(TheRevelation.Damage), 0, Player.whoAmI);
                            }
                            else
                            {
                                revelationCharge -= 0.01f;
                                if (revelationCharge <= 0)
                                {
                                    revelationCharge = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    revelationCharge = 0;
                }
            }
            if (holyGroundTime > 0)
            {
                holyGroundTime--;
            }
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<PhantomWyrm>()] <= 0 && wyrmPhantom)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Util.Util.randomVec(14), ModContent.ProjectileType<PhantomWyrm>(), (int)(Player.GetTotalDamage(DamageClass.Summon).ApplyTo(Ystralyn.PhantomDamage)), 1, Player.whoAmI);
                }
            }
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.ModProjectile is WhipOfServiceProjectile wos)
                {
                    if (p.Colliding(p.getRect(), Player.getRect()) && p.owner != Player.whoAmI)
                    {
                        if (wos.hitCd[Player.whoAmI] <= 0)
                        {
                            float s = Player.Calamity().adrenaline;
                            if (Player.whoAmI == Main.myPlayer)
                            {
                                Player.Hurt(PlayerDeathReason.ByPlayerItem(p.owner, p.owner.ToPlayer().HeldItem), p.owner.ToPlayer().HeldItem.damage, 0, true, false);
                            }
                            serviceWhipDamageBonus += 0.07f;
                            wos.hitCd[Player.whoAmI] = 1000;
                            Player.Calamity().rage += Player.Calamity().rageMax / 20f;
                            if (Player.Calamity().rage > Player.Calamity().rageMax)
                            {
                                Player.Calamity().rage = Player.Calamity().rageMax;
                            }
                        }

                    }
                }
            }

            if (accWispLantern || visualWispLantern)
            {
                if (Player.whoAmI == Main.myPlayer && Player.ownedProjectileCounts[ModContent.ProjectileType<WispLanternProj>()] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<WispLanternProj>(), 0, 0, Player.whoAmI);
                }
            }

            if (!nihShell)
            {
                nihShellCount = 0;
            }
            if (holdingPoop)
            {
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
                Player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi);
            }
            if (noItemTime > 0)
            {
                noItemTime--;
            }
            if (brokenAnkh)
            {
                if (Player.whoAmI == Main.myPlayer && !Main.dedServ) {
                    if (!holdingPoop && CEKeybinds.PoopHoldHotKey is not null && CEKeybinds.PoopHoldHotKey.JustPressed)
                    {
                        if (PoopHold is not null)
                        {
                            PoopsUI.holdAnmj = 0.2f;
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, new Vector2(0, 0), PoopHold.ProjectileType(), 80, 3, Player.whoAmI);
                            holdingPoop = true;
                            PoopHold = null;
                            Util.Util.PlaySound("poop_itemthrow");
                        }
                        else
                        {
                            if (poops.Count > 0)
                            {
                                PoopsUI.holdAnmj = 0.2f;
                                PoopHold = poops[0];
                                poops.RemoveAt(0);
                            }
                        }
                    }
                    if (!Main.dedServ && !holdingPoop && CEKeybinds.ThrowPoopHotKey is not null && CEKeybinds.ThrowPoopHotKey.JustPressed)
                    {
                        if (poops.Count > 0)
                        {
                            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, new Vector2(0, 0), poops[0].ProjectileType(), 80, 3, Player.whoAmI);
                            holdingPoop = true;
                            poops.RemoveAt(0);
                            Util.Util.PlaySound("poop_itemthrow");
                        }
                    }
                }
            }
            if (holdingPoop)
            {
                noItemTime = 12;
                if (Player.whoAmI == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        holdingPoop = false;
                    }
                }
            }
            if (resetTileSets)
            {
                resetTileSets = false;
                Main.tileSolid = tileSolid;
                Main.tileSolidTop = solidTop;
                TileID.Sets.Platforms = tilePlatform;
                tileSolid = null;
                solidTop = null;
                tilePlatform = null;
            }
            if (reincarnationBadge)
            {
                if (!Player.HasBuff<NOU>() && Player.ownedProjectileCounts[ModContent.ProjectileType<RbCircle>()] < 1)
                {
                    if (Main.myPlayer == Player.whoAmI)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<RbCircle>(), 0, 0, Player.whoAmI);
                    }
                }
                if (!Main.dedServ && CalamityKeybinds.AscendantInsigniaHotKey.JustPressed || (rBadgeActive && (Player.controlJump || rBadgeCharge <= 0)))
                {
                    rBadgeActive = !rBadgeActive;
                    if (rBadgeActive)
                    {
                        Player.mount.Dismount(Player);
                        SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/AscendantActivate"), Player.Center);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/AscendantOff"), Player.Center);
                        Player.velocity *= 0.2f;
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModPacket pack = Mod.GetPacket();
                        pack.Write((byte)CEMessageType.PlayerSetRB);
                        pack.Write(Player.whoAmI);
                        pack.Write(rBadgeActive);
                        pack.Send();
                    }
                }
                if (Player.controlMount)
                {
                    if (rBadgeActive)
                    {
                        rBadgeActive = false;
                        SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/AscendantOff"), Player.Center);
                        Player.velocity *= 0.2f;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            ModPacket pack = Mod.GetPacket();
                            pack.Write((byte)CEMessageType.PlayerSetRB);
                            pack.Write(Player.whoAmI);
                            pack.Write(rBadgeActive);
                            pack.Send();
                        }
                    }
                }
                if (rBadgeActive)
                {
                    rBadgeCharge -= 0.025f;
                }
                else
                {
                    rBadgeCharge += 0.01f;
                    if (rBadgeCharge > 12)
                    {
                        rBadgeCharge = 12;
                    }
                }
            }
            else
            {
                rBadgeActive = false;
            }

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
                if (!VSoundsPlayed && voidcharge >= 1)
                {
                    VSoundsPlayed = true;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/PhantomHeartUse"));
                }
            }
            if (VoidInspire <= 0 && voidcharge < 1)
            {
                VSoundsPlayed = false;
            }
            if (!Main.dedServ && VoidCharge >= 1 && CalamityKeybinds.ArmorSetBonusHotKey.JustPressed)
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
                    int magiShieldAddCount = (int)(Player.statManaMax2 * 0.5f);
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
                        Player.ApplyDamageToNPC(n, (int)Player.GetTotalDamage(DamageClass.Melee).ApplyTo(1200), 0, 0, false, Util.CUtil.rogueDC);

                    }
                }
                daPoints.Add(Player.Center - Player.velocity * 0.5f);
                daPoints.Add(Player.Center);
            }
            VaMoving--;
            if (Player.Entropy().oracleDeck)
            {
                if (OracleDeckHealCd <= 0)
                {
                    OracleDeckHealCd = 300;
                    if (Util.Util.getDistance(Main.LocalPlayer.Center, Player.Center) < 40)
                    {
                        if (Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2)
                        {
                            Main.LocalPlayer.Heal(10);
                        }
                    }
                }
            }
            if (OracleDeckHealCd > 0)
            {
                OracleDeckHealCd--;
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
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<HadopelagicEchoII>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<HadopelagicEchoIIProj>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<HadopelagicEchoIIProj>(), Player.HeldItem.damage, 0, Player.whoAmI);
                    }
                }
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<RailPulseBow>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<RailPulseBowProjectile>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<RailPulseBowProjectile>(), Player.HeldItem.damage, 0, Player.whoAmI);
                    }
                }
                if (!Player.HeldItem.IsAir && Player.HeldItem.type == ModContent.ItemType<Oblivion>())
                {
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<OblivionHoldout>()] <= 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<OblivionHoldout>(), Player.HeldItem.damage, 0, Player.whoAmI);
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
                            Player.ApplyDamageToNPC(npc, (int)Player.GetTotalDamage(Util.CUtil.rogueDC).ApplyTo(460 + 50 * daCount), 0, 0, false, Util.CUtil.rogueDC);
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
                                    int dmg = 0;
                                    for (int i = daCount; i > 0; i--)
                                    {
                                        dmg += 460 + 50 * i;
                                    }
                                    Player.ApplyDamageToNPC(n, (int)Player.GetTotalDamage(Util.CUtil.rogueDC).ApplyTo(dmg), 0, 0, false, Util.CUtil.rogueDC);
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
        public float attackSpeed = 0;
        public int bloodTrCD = 0;
        public int MariviniumShieldCount = 0;
        public int MariviniumShieldCd = 12 * 60;
        public bool visualMagiShield = false;

        public override void PostUpdateEquips()
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.ModProjectile is WispLanternProj wl)
                {
                    wl.applyEffects();
                }
            }
            if (holyGroundTime > 0)
            {
                Player.GetAttackSpeed(DamageClass.Generic) += 1;
                Player.GetDamage(DamageClass.Generic) += 1;
            }
            if (MariviniumSet)
            {
                int crit = (int)(Player.GetTotalCritChance(DamageClass.Default) + 5E-06f); ;
                if (Player.HeldItem.type != ItemID.None)
                {
                    crit = Player.GetWeaponCrit(Player.HeldItem);
                }
                if (crit > 100)
                {
                    Player.GetDamage(DamageClass.Default) += (crit - 100) * 0.01f;
                }
            }
            Player.GetDamage(DamageClass.Generic) += serviceWhipDamageBonus;
            Player.GetCritChance(DamageClass.Generic) += (int)(serviceWhipDamageBonus * 180);
            Player.pickSpeed -= serviceWhipDamageBonus * 4.3f;
            Player.GetAttackSpeed(DamageClass.Generic) += serviceWhipDamageBonus * 2.2f;
            if (Player.pickSpeed < 0)
            {
                Player.pickSpeed = 0;
            }
            if (Player.HeldItem.ModItem is HorizonssKey)
            {
                Player.maxMinions -= 8;
                if (Player.maxMinions < 1)
                {
                    Player.maxMinions = 1;
                }
            }
            if (holyGroundTime > 0)
            {
                dodgeChance += 0.5f;
            }
            Player.manaCost *= ManaCost;
            if (Player.Entropy().SCrown)
            {
                Player.Calamity().defenseDamageRatio = 0;
            }
            if (Player.Entropy().wisdomCard)
            {
                Player.manaCost *= 0.8f;
            }
            if (Player.Entropy().oracleDeck)
            {
                Player.manaCost *= 0.7f;
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
                Player.GetDamage(DamageClass.Generic) += Player.maxMinions * (EvilDeck ? 0.03f : 0.02f);
            }
            if (VoidInspire > 0)
            {
                Player.GetDamage(DamageClass.Generic) += 0.5f;
                Player.Calamity().infiniteFlight = true;
            }
            if (ArchmagesMirror)
            {
                enhancedMana += 0.05f;
            }
            if (EvilDeck)
            {
                lifeRegenPerSec = (int)(lifeRegenPerSec * 0.3f);
            }

        }
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
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<BloodRing>()] > 0)
            {
                Player.headRotation = MathHelper.ToRadians(Player.direction * -(45 + 16f * (float)(Math.Cos(Main.GameUpdateCount * 0.1f))));
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
        public bool resetTileSets = false;
        public bool wyrmPhantom = false;

        public override void SetControls()
        {
            if (rBadgeActive)
            {
                cDown = Player.controlDown;
                cLeft = Player.controlLeft;
                cRight = Player.controlRight;
                cUp = Player.controlUp;
                Player.controlDown = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlUp = false;
            }
            if (BeeGame.Active)
            {
                Player.controlDown = false;
                Player.controlJump = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlUp = false;
            }
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            if (Player.Calamity().ZoneAstral)
            {
                if (Main.rand.NextBool(10))
                {
                    itemDrop = ModContent.ItemType<GreedCard>();
                }
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
            if (EBookStackItems != null)
            {
                int BookMarks = EBookStackItems.Count;
                foreach (var item in EBookStackItems)
                {
                    if (item.type != ItemID.None)
                    {
                        tag["EntropyBookMarks"] = BookMarks;
                        Mod.Logger.Info("Saved book marks:" + BookMarks.ToString());
                        for (int i = 0; i < BookMarks; i++)
                        {
                            tag["EntropyBookMark" + i.ToString()] = ItemIO.Save(EBookStackItems[i]);
                        }
                        break;
                    }
                }
            }
        }
        public List<Item> EBookStackItems = null;
        public override void LoadData(TagCompound tag)
        {
            var boost = tag.GetList<string>("EntropyBoosts");
            CruiserLoreUsed = boost.Contains("CruiserLore");
            EBookStackItems = new List<Item>();

            if (tag.ContainsKey("EntropyBookMarks"))
            {
                int BookMarks = (int)tag["EntropyBookMarks"];
                for (int i = 0; i < BookMarks; i++)
                {
                    var item = ItemIO.Load((TagCompound)tag["EntropyBookMark" + i.ToString()]);
                    if (item.type != ItemID.None)
                    {
                        Mod.Logger.Info("Loaded mark:" + item.Name);
                        EBookStackItems.Add(item);
                    }
                }
            }

        }
    }
}
