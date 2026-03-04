using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class CrystalSpike : RogueWeapon
    {
        public const int MAXSTICK = 12;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.damage = 14;
            Item.ArmorPenetration = 6;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<CrystalSpikeThrow>();
            Item.shootSpeed = 12f;
            Item.DamageType = CEUtils.RogueDC;
        }

        public override float StealthDamageMultiplier => 1;
        public override float StealthVelocityMultiplier => 1;
        public override float StealthKnockbackMultiplier => 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                ReturnAllSpikes(player);
            }
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0);

            if (player.Calamity().StealthStrikeAvailable())
            {
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
            }
            return false;
        }
        public void ReturnAllSpikes(Player player)
        {
            int type = ModContent.ProjectileType<CrystalSpikeReturning>();
            int tm = 0;
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == player.whoAmI && p.type == Item.shoot && p.ModProjectile is CrystalSpikeThrow cst && cst.StickNPC >= 0)
                {
                    Projectile.NewProjectile(p.GetSource_FromThis(), p.Center, Vector2.Zero, type, p.damage, p.knockBack * 2, player.whoAmI, tm);
                    p.Kill();
                    tm += 2;
                }
            }
            if (tm > 0)
                CEUtils.PlaySound("flashback", 1.6f, player.Center, 6, 0.55f);
        }
    }

    public class CrystalSpikeThrow : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/CrystalSpike";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, true, -1);
            Projectile.width = Projectile.height = 14;
            Projectile.timeLeft = 120 * 4;
            Projectile.MaxUpdates = 3;
        }
        public int StickNPC = -1;
        public Vector2 offset = Vector2.Zero;
        public Vector2 vel = Vector2.Zero;
        public int counter = 0;
        public override void AI()
        {
            if (counter == 0)
                CEUtils.PlaySound((Projectile.ai[0] == 1 ? "crystalsound" : "bne") + (Main.rand.NextBool() ? "2" : "3"), Main.rand.NextFloat(1.4f, 1.6f), Projectile.Center, 6, 0.7f)
;            counter++;
            if (StickNPC == -1)
            {
                if (counter > 46)
                {
                    Projectile.velocity.Y += 0.16f;
                    Projectile.velocity *= 0.998f;
                    Projectile.velocity.X *= 0.98f;
                }
                if (Projectile.ai[0] == 0)
                    EParticle.spawnNew(new CrystalGlow(), Projectile.Center, Vector2.Zero, Color.MediumPurple, 0.6f, 1, true, BlendState.Additive, 0, 20);
                for(float i = 0; i < 1; i+=0.5f)
                {
                    var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.PurpleTorch);
                    d.position = Vector2.Lerp(Projectile.Center - Projectile.velocity, Projectile.Center, i) + CEUtils.randomPointInCircle(5);
                    d.velocity = Projectile.velocity * Main.rand.NextFloat();
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if(vel == Vector2.Zero)
                vel = Projectile.velocity;
            if (StickNPC >= 0)
                if (!StickNPC.ToNPC().active || StickNPC.ToNPC().dontTakeDamage)
                    StickNPC = -1;
            if(StickNPC >= 0)
            {
                Projectile.Center = StickNPC.ToNPC().Center + offset;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if(counter == 0)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StickNPC);
            writer.WriteVector2(offset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StickNPC = reader.ReadInt32();
            offset = reader.ReadVector2();
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (StickNPC >= 0)
                return false;
            return null;
        }
        public override bool ShouldUpdatePosition()
        {
            return StickNPC < 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("truemoonlighthit", Main.rand.NextFloat(2f, 2.4f), target.Center, 60, 0.6f);
            int sum = 0;
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == Projectile.owner && p.type == Projectile.type && p.ModProjectile is CrystalSpikeThrow cs && cs.StickNPC == target.whoAmI)
                    sum++;
            }
            if (Projectile.ai[0] == 1 || sum >= CrystalSpike.MAXSTICK)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * -1, ModContent.ProjectileType<CrystalSpikePop>(), 0, 0, Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                if (StickNPC < 0)
                {
                    StickNPC = target.whoAmI;
                    offset = Projectile.Center - target.Center;
                    Projectile.timeLeft = 36 * 60 * Projectile.MaxUpdates;
                }
            }
            CEUtils.SyncProj(Projectile.whoAmI);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if(Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, oldVelocity * -1, ModContent.ProjectileType<CrystalSpikePop>(), 0, 0, Projectile.owner);
            }
            return true;
        }
    }
    public class CrystalSpikePop : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/CrystalSpike";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            Projectile.Opacity = Projectile.timeLeft / 60f;
            Projectile.velocity *= 0.984f;
            Projectile.velocity.Y += 0.36f;
            Projectile.rotation += Projectile.velocity.X * 0.04f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
    }
    public class CrystalSpikeReturning : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/CrystalSpike";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 10000;
        }
        public int counter = 0;
        public Vector2 spawn = Vector2.Zero;
        public Vector2 ofst = CEUtils.randomPointInCircle(180);
        public override void AI()
        {
            if (spawn == Vector2.Zero)
                spawn = Projectile.Center;
            Vector2 target = Projectile.GetOwner().Center;
            counter++;
            Vector2 mid = (spawn + target) * 0.5f + ofst;
            if (counter == 36 && Projectile.ai[0] == 0 && Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.GetOwner().Center, Vector2.Zero, ModContent.ProjectileType<SwingSmear>(), 0, 0, Projectile.owner);
            if (counter <= 36f)
            {
                Vector2 pos = CEUtils.Bezier(new List<Vector2>() { spawn, mid, target }, counter / 36f);
                Vector2 offset = pos - Projectile.Center;
                Projectile.rotation += offset.X * 0.05f;
                EParticle.spawnNew(new CrystalGlow(), Projectile.Center, Vector2.Zero, Color.MediumPurple, 0.6f, 1, true, BlendState.Additive, 0, 20);
                for (float i = 0; i < 1; i += 0.5f)
                {
                    var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.PurpleTorch);
                    d.position = Vector2.Lerp(Projectile.Center, pos, i) + CEUtils.randomPointInCircle(5);
                    d.velocity = offset * Main.rand.NextFloat();
                }
                Projectile.Center = pos;
            }
            else
            {
                if (counter - 36 > Projectile.ai[0])
                {
                    Projectile.Kill();
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 vel = (Main.MouseWorld - Projectile.GetOwner().MountedCenter).normalize() * 15;
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.GetOwner().MountedCenter + vel.normalize().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10, 10), vel, ModContent.ProjectileType<CrystalSpikeThrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                        p.ToProj().Calamity().stealthStrike = true;
                        CEUtils.SyncProj(p);
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (counter > 36)
                return false;
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
    }
    public class SwingSmear: ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Default, false, -1);
            Projectile.timeLeft = 55;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            Projectile.Center = Projectile.GetOwner().GetDrawCenter();
            Projectile.rotation += 0.66f;
            if (Main.myPlayer == Projectile.owner)
                Main.LocalPlayer.direction = Main.MouseWorld.X > Main.LocalPlayer.Center.X ? 1 : -1;
            Projectile.GetOwner().SetHandRotWithDir(Projectile.rotation * Projectile.GetOwner().direction, Projectile.GetOwner().direction);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, null, Color.MediumPurple * 1.5f * (Projectile.timeLeft / 55f), Projectile.rotation * Projectile.GetOwner().direction, Projectile.GetTexture().Size() * 0.5f, Projectile.scale * 0.42f, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
