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
    public class EmberSpike : RogueWeapon
    {
        public static int MAXSTICK => 12;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[S]", MAXSTICK.ToString());
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.damage = 28;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<EmberSpikeThrow>();
            Item.shootSpeed = 12f;
            Item.DamageType = CEUtils.RogueDC;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystalSpike>()
                .AddIngredient<TectonicShard>(6)
                .AddTile(TileID.Hellforge)
                .Register();
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
            int type = ModContent.ProjectileType<EmberSpikeReturning>();
            int tm = 0;
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if (p.owner == player.whoAmI && p.type == Item.shoot && p.ModProjectile is EmberSpikeThrow cst && cst.StickNPC >= 0)
                {
                    Projectile.NewProjectile(p.GetSource_FromThis(), p.Center, Vector2.Zero, type, p.damage, p.knockBack * 2, player.whoAmI, tm);
                    p.Kill();
                    tm += 2;
                }
            }
            if (tm > 0) 
                CEUtils.PlaySound("RockCrumble", Main.rand.NextFloat(2.5f, 2.8f), player.Center, 60, 0.5f);

        }
    }

    public class EmberSpikeThrow : ModProjectile
    {
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 8;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/EmberSpike";
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
                CEUtils.PlaySound("flamethrower end", Main.rand.NextFloat(4f, 4.2f), Projectile.Center, 6, 0.3f)
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
                    EParticle.spawnNew(new CrystalGlow(), Projectile.Center, Vector2.Zero, Color.OrangeRed, 0.6f, 1, true, BlendState.Additive, 0, 20);
                else
                    EParticle.spawnNew(new CrystalGlow(), Projectile.Center, Vector2.Zero, Color.OrangeRed, 0.36f, 1, true, BlendState.Additive, 0, 8);

                for (float i = 0; i < 1; i += 0.5f)
                    {
                        var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.RedTorch);
                        d.position = Vector2.Lerp(Projectile.Center - Projectile.velocity, Projectile.Center, i) + CEUtils.randomPointInCircle(5);
                        d.velocity = Projectile.velocity * Main.rand.NextFloat(0.4f);
                        d.noGravity = true;
                        d.scale = Main.rand.NextFloat(1, 1.2f);
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
                StickNPC.ToNPC().AddBuff(BuffID.OnFire3, 180);
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
            EParticle.spawnNew(new CrystalGlow(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 10, Vector2.Zero, Color.OrangeRed * 1.3f, 1.5f, 1, true, BlendState.Additive, 0, 10);

            CEUtils.PlaySound("RockCrumble", Main.rand.NextFloat(2.4f, 2.8f), target.Center, 60, 0.4f);
            int sum = 0;
            foreach(Projectile pj in Main.ActiveProjectiles)
            {
                if (pj.owner == Projectile.owner && pj.type == Projectile.type && pj.ModProjectile is EmberSpikeThrow cs && cs.StickNPC == target.whoAmI)
                    sum++;
            }
            if (sum >= EmberSpike.MAXSTICK || Projectile.ai[0] == 1)
            {
                if(Projectile.Calamity().stealthStrike)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.6f, 1f), ModContent.ProjectileType<TectinicShardHoming>(), Projectile.damage, 4, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(-1, -0.5f), ModContent.ProjectileType<EmberSpikePop>(), 0, 0, Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                target.AddBuff(BuffID.OnFire3, 180);
                Projectile p = Projectile;
                Player player = Projectile.GetOwner();
                CEUtils.SpawnExplotionFriendly(p.GetSource_FromThis(), player, p.Center, p.damage, 120, CEUtils.RogueDC);
                float scale = 100 / 40f;
                EParticle.spawnNew(new ShineParticle(), p.Center, Vector2.Zero, Color.OrangeRed * 0.8f, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
                EParticle.spawnNew(new ShineParticle(), p.Center, Vector2.Zero, Color.Firebrick * 0.8f, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
                GeneralParticleHandler.SpawnParticle(new CustomPulse(p.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 24));
                GeneralParticleHandler.SpawnParticle(new CustomPulse(p.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
                GeneralParticleHandler.SpawnParticle(new CustomPulse(p.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 15));

                if (StickNPC < 0)
                {
                    StickNPC = target.whoAmI;
                    offset = Projectile.Center - target.Center;
                    Projectile.timeLeft = 45 * 60 * Projectile.MaxUpdates;
                }
            }
            CEUtils.SyncProj(Projectile.whoAmI);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            EParticle.spawnNew(new CrystalGlow(), Projectile.Center + oldVelocity + Projectile.rotation.ToRotationVector2() * 4, Vector2.Zero, Color.OrangeRed * 1.3f, 1.5f, 1, true, BlendState.Additive, 0, 10);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, oldVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(-1, -0.5f), ModContent.ProjectileType<EmberSpikePop>(), 0, 0, Projectile.owner);
            }
            return true;
        }
    }
    public class EmberSpikePop : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/EmberSpike";
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
    public class EmberSpikeReturning : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/EmberSpike";
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
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.GetOwner().Center, Vector2.Zero, ModContent.ProjectileType<SwingSmearEmber>(), 0, 0, Projectile.owner);
            if (counter <= 36f)
            {
                Vector2 pos = CEUtils.Bezier(new List<Vector2>() { spawn, mid, target }, counter / 36f);
                Vector2 offset = pos - Projectile.Center;
                Projectile.rotation += offset.X * 0.05f;
                EParticle.spawnNew(new CrystalGlow(), Projectile.Center, Vector2.Zero, Color.OrangeRed, 0.6f, 1, true, BlendState.Additive, 0, 20);
                for (float i = 0; i < 1; i += 0.5f)
                {
                    var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.RedTorch);
                    d.position = Vector2.Lerp(Projectile.Center, pos, i) + CEUtils.randomPointInCircle(5);
                    d.velocity = offset * Main.rand.NextFloat(0.4f);
                    d.noGravity = true;
                    d.scale = Main.rand.NextFloat(1, 1.2f);
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
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.GetOwner().MountedCenter + vel.normalize().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-10, 10), vel, ModContent.ProjectileType<EmberSpikeThrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
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
    public class SwingSmearEmber: ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Default, false, -1);
            Projectile.timeLeft = 10;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.GetOwner().ownedProjectileCounts[ModContent.ProjectileType<EmberSpikeReturning>()] > 0)
                Projectile.timeLeft = 10;
            Projectile.Center = Projectile.GetOwner().GetDrawCenter();
            Projectile.rotation += 0.66f;
            if (Main.myPlayer == Projectile.owner)
                Main.LocalPlayer.direction = Main.MouseWorld.X > Main.LocalPlayer.Center.X ? 1 : -1;
            Projectile.GetOwner().SetHandRotWithDir(Projectile.rotation * Projectile.GetOwner().direction + (Projectile.GetOwner().direction > 0 ? 0 : MathHelper.Pi) + Projectile.GetOwner().direction * -0.6f, Projectile.GetOwner().direction);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 1.25f * (Projectile.timeLeft / 10f), Projectile.rotation * Projectile.GetOwner().direction, Projectile.GetTexture().Size() * 0.5f, Projectile.scale * 0.36f, Projectile.GetOwner().direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
