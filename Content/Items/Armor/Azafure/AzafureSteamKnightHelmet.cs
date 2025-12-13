using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Azafure
{
    [AutoloadEquip(EquipType.Head)]
    public class AzafureSteamKnightHelmet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.defense = 19;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == Type && body.type == ModContent.ItemType<AzafureSteamKnightArmor>() && legs.type == ModContent.ItemType<AzafureSteamKnightLeggings>();
        }


        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Mod.GetLocalization("AzafureSet2").Value;
            player.GetModPlayer<AzafureSteamKnightArmorPlayer>().ArmorSetBonus = true;
            if (!ModContent.GetInstance<Config>().MariviumArmorSetOnlyProvideStealthBarWhenHoldingRogueWeapons || player.HeldItem.DamageType.CountsAsClass(CEUtils.RogueDC))
            {
                player.Calamity().wearingRogueArmor = true;
                player.Calamity().rogueStealthMax += 0.8f;
            }
        }
        public override void UpdateEquip(Player player)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AzafureHeavyHelmet>()
                .AddRecipeGroup(CERecipeGroups.AnyOrichalcumBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class AzafureSteamKnightArmorPlayer : ModPlayer
    {
        public bool ArmorSetBonus = false;
        public float durability = 1;
        public int DurabilityRegenDelay = 0;
        public bool DurabilityActive = true;
        public int DeathExplosion = -1;
        public int DeathExplosionCD = 0;
        public bool ExplosionFlag = false;
        public PlayerDeathReason dmgSource = null;
        public LoopSound chargeSnd = null;
        public override void PostUpdate()
        {
            if (chargeSnd != null && chargeSnd.timeleft <= 0)
                chargeSnd = null;
            if (chargeSnd != null)
            {
                chargeSnd.setVolume_Dist(Player.Center, 100, 1600, 1);
                chargeSnd.instance.Pitch = (1 - (DeathExplosion / 80f)) * 1.8f;
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (ExplosionFlag && DeathExplosion > 0)
                return false;
            if (ArmorSetBonus && !ExplosionFlag && DeathExplosionCD <= 0)
            {
                damageSource = PlayerDeathReason.ByCustomReason("");
            }
            return true;
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (ArmorSetBonus)
            {
                if (DeathExplosionCD <= 0 && !ExplosionFlag)
                {
                    ExplosionFlag = true;
                    DeathExplosionCD = 10 * 60;
                    DeathExplosion = 80;
                    dmgSource = damageSource;
                    Player.dead = false;
                    chargeSnd = new LoopSound(CalamityEntropy.ofCharge);
                    chargeSnd.instance.Pitch = 0;
                    chargeSnd.instance.Volume = 0;
                    chargeSnd.play();
                    chargeSnd.timeleft = 80;
                }
                else
                    ExplosionFlag = false;
            }
        }
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            return DeathExplosion < 0;
        }
        public override bool CanBeHitByProjectile(Projectile proj)
        {
            return DeathExplosion < 0;
        }
        public override void ResetEffects()
        {
            ArmorSetBonus = false;
            if (DeathExplosion > 0 && ExplosionFlag)
            {
                DeathExplosion--;
                Player.velocity *= 0;
                Player.Entropy().noItemTime = 5;
                if (DeathExplosion < 70 && DeathExplosion % 2 == 0)
                {
                    if(DeathExplosion % 6 == 0)
                        ScreenShaker.AddShake(new ScreenShaker.ScreenShake(Vector2.Zero, Utils.Remap(Main.LocalPlayer.Center.Distance(Player.Center), 4000, 1000, 0, 12)));

                    EParticle.NewParticle(new ShockParticle2(), Player.Center, Vector2.Zero, Color.White, 0.1f, 1, true, BlendState.Additive, CEUtils.randomRot());
                }
                if (DeathExplosion == 0 || DeathExplosion == 4 || DeathExplosion == 8 || DeathExplosion == 12 || DeathExplosion == 16)
                {
                    CEUtils.PlaySound("pulseBlast", 0.6f, Player.Center, 6, 1f);
                    CEUtils.PlaySound("blackholeEnd", 0.6f, Player.Center, 6, 1f);

                    GeneralParticleHandler.SpawnParticle(new PulseRing(Player.Center, Vector2.Zero, Color.Firebrick, 0.1f, 12f, 8));
                    EParticle.spawnNew(new ShineParticle(), Player.Center, Vector2.Zero, Color.Firebrick, 20f, 1, true, BlendState.Additive, 0, 16);
                    EParticle.spawnNew(new ShineParticle(), Player.Center, Vector2.Zero, Color.White, 16f, 1, true, BlendState.Additive, 0, 16);
                    ScreenShaker.AddShakeWithRangeFade(new ScreenShaker.ScreenShake(Vector2.Zero, 100), 1200);
                    CEUtils.SpawnExplotionFriendly(Player.GetSource_FromThis(), Player, Player.Center, ((int)(Player.GetBestClassDamage().ApplyTo(1500))).ApplyOldFashionedDmg(), 1200, DamageClass.Generic).ArmorPenetration = 60;
                }
                if (DeathExplosion == 0)
                {
                    DeathExplosion = -1;
                    ExplosionFlag = false;
                    Player.KillMe(PlayerDeathReason.ByCustomReason(Mod.GetLocalization("DeathExplode").Value.Replace("[PlayerName]", Player.name)), 10000, 0);
                }
            }
        }
        public override void PostUpdateEquips()
        {
            if (!ExplosionFlag || DeathExplosion == 0)
                DeathExplosion = -1;
            DeathExplosionCD--;
            if (ArmorSetBonus)
            {
                Player.Entropy().moveSpeed += 0.2f;
                DurabilityRegenDelay--;
                if (DurabilityActive)
                {
                    Player.endurance += durability * 0.3f + 0.2f;
                    Player.statDefense += (int)(durability * 36);
                    Player.noKnockback = true;
                }
                else
                {
                    DurabilityRegenDelay = -1;
                }
                if (DurabilityRegenDelay <= 0)
                {
                    durability += 0.001f;
                    if (durability >= 1)
                    {
                        durability = 1;
                        if (!DurabilityActive)
                            CEUtils.PlaySound("AuricQuantumCoolingCellInstallNew", 0.7f, Player.Center);
                        DurabilityActive = true;
                    }
                }
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (ArmorSetBonus)
            {
                if (DurabilityActive)
                {
                    CEUtils.PlaySound($"ExoHit{Main.rand.Next(1, 5)}", Main.rand.NextFloat(0.6f, 0.8f), Player.Center, 6, 0.6f);
                    if (DurabilityRegenDelay < 5 * 60)
                        DurabilityRegenDelay = 5 * 60;
                    durability -= float.Min(0.52f, info.Damage / 900f);

                    //耐久没了暂时失效
                    if (durability <= 0)
                    {
                        durability = 0;
                        DurabilityActive = false;
                        CEUtils.PlaySound("chainsaw_break", 1.3f, Player.Center, 6, 0.6f);
                        for (int i = 0; i < 16; i++)
                        {
                            EParticle.NewParticle(new EMediumSmoke(), Player.Center + CEUtils.randomPointInCircle(12), CEUtils.randomPointInCircle(16), Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()), Main.rand.NextFloat(1f, 2f), 1, true, BlendState.AlphaBlend, CEUtils.randomRot(), 120);
                        }
                    }
                }
            }
        }
        public static void DrawDuraBar(float dura)
        {
            var mplayer = Main.LocalPlayer.GetModPlayer<AzafureHeavyArmorPlayer>();
            Color color = mplayer.DurabilityActive ? Color.White : new Color(255, 80, 80) * 0.5f;
            Color color2 = mplayer.DurabilityActive ? Color.White : new Color(255, 142, 142) * 0.7f;
            Vector2 Center = Main.ScreenSize.ToVector2() * 0.5f + new Vector2(0, -48);
            if (dura < 0.32f && mplayer.DurabilityActive)
            {
                Center += new Vector2(Main.rand.NextFloat() * ((0.32f - dura) * 20), Main.rand.NextFloat() * ((0.32f - dura) * 20));
            }
            Texture2D tex1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Armor/Azafure/DurabilityBarA").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Armor/Azafure/SteamKnightBar").Value;
            Main.spriteBatch.Draw(tex2, Center, null, color, 0, tex2.Size() / 2f, 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex1, Center, new Rectangle(0, 0, (int)(tex1.Width * dura), tex1.Height), color2, 0, tex1.Size() / 2f, 1, SpriteEffects.None, 0);
        }
    }
}
