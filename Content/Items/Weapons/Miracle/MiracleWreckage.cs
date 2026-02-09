using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Content.Items.Weapons.Miracle
{
    public class MiracleWreckage : ModItem
    {
        public const int MaxPlugged = 6;
        public override void SetDefaults()
        {
            Item.damage = 2368;
            Item.DamageType = ModContent.GetInstance<MeleeDamageClass>();
            Item.width = 48;
            Item.height = 60;
            Item.useTime = 46;
            Item.useAnimation = 46;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MiracleWreckageHeldAnm>();
            Item.shootSpeed = 16f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int heldType = ModContent.ProjectileType<MiracleWreckageHeld>();
            if (player.ownedProjectileCounts[heldType] > 0)
                return false;
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, heldType, damage, knockback, player.whoAmI);
                return false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<DevilsDevastation>().
                AddIngredient<FadingRunestone>(2).
                AddTile<AbyssalAltarTile>().
                Register();
        }
    }
    public class MiracleWreckageHeldAnm : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Miracle/MiracleWreckage";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.width = Projectile.height = 90;
            Projectile.timeLeft = 4;
            Projectile.light = 1;
            Projectile.MaxUpdates = 3;
        }
        public float rot = 0;
        public float rotVel = 0;
        public override void AI()
        {
            Projectile.timeLeft = 4;
            Player player = Projectile.GetOwner();
            float speed = player.GetTotalAttackSpeed(DamageClass.Melee);
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                rot = -0.7f;
                rotVel = -0.12f;
                Projectile.ai[1] = Projectile.velocity.X > 0 ? 1 : -1;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + rot * Projectile.ai[1];
            player.SetHandRotWithDir(Projectile.rotation, Math.Sign(Projectile.ai[1]));
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.GetDrawCenter();
            player.Calamity().mouseWorldListener = true;
            Projectile.velocity = (player.Calamity().mouseWorld - Projectile.Center).normalize() * Projectile.velocity.Length();
            rot += rotVel * speed;
            rotVel *= (float)Math.Pow(0.94f, speed);

            if (Projectile.ai[0] * speed > 60)
            {
                if (Projectile.localAI[1] == 0)
                {
                    Projectile.localAI[1] = 1;
                    rotVel = 0.3f;
                }
            }
            if (Projectile.ai[0] * speed > 72)
            {
                if (Projectile.ai[2] == 0)
                {
                    Projectile.ai[2] = 1;
                    if(Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<MiracleWreckageThrow>(), Projectile.damage, 6, Projectile.owner);
                    }
                    rot = 0;
                    rotVel = 0;
                }
            }
            if (Projectile.ai[0] * speed > 88)
            {
                if (Projectile.ai[2] <= 1)
                {
                    Projectile.ai[2] = 2;
                }
            }
            if (Projectile.ai[0] * speed > 92)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<MiracleWreckageThrow>(), Projectile.damage, 6, Projectile.owner);
                }
                Projectile.Kill();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 origin = new Vector2(0, tex.Height);
            float rot = Projectile.rotation + MathHelper.PiOver4;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, rot, origin, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
    }
    public class MiracleWreckageThrow: ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Miracle/MiracleWreckage";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.width = Projectile.height = 90;
            Projectile.timeLeft = 80;
            Projectile.light = 1;
            Projectile.MaxUpdates = 3;
        }
        public int Hit = 0; //1 for npc  2 for tile
        public Vector2 offset = Vector2.Zero;
        public int target = -1;
        public uint hitTime = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Hit);
            writer.WriteVector2(offset);
            writer.Write(target);
            writer.Write(hitTime);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Hit = reader.ReadInt32();
            offset = reader.ReadVector2();
            target = reader.ReadInt32();
            hitTime = reader.ReadUInt32();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Hit == 0 ? null : false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 30;
            Projectile.velocity = oldVelocity;
            if (Hit == 0)
            {
                hitTime = Main.GameUpdateCount;
                Projectile.velocity *= 0.1f;
                Hit = 2;
                HitEffect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 90);
            }
            return false;
        }
        public void Update()
        {
            if (Main.myPlayer == Projectile.owner)
                CEUtils.SyncProj(Projectile.whoAmI);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Hit == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<MiracleExplosion>(), Projectile.damage, 0, Projectile.owner);
                CEUtils.PlaySound("DemonSwordImpact2", Main.rand.NextFloat(0.9f, 1.2f), target.Center);
                hitTime = Main.GameUpdateCount;
                Projectile.timeLeft = 36 * 60;
                uint last = uint.MaxValue;
                Projectile lastProj = null;
                int amount = 0;
                foreach(Projectile p in Main.ActiveProjectiles)
                {
                    if(p.type == Projectile.type && p.whoAmI != Projectile.whoAmI && p.owner == Projectile.owner)
                    {
                        if(p.ModProjectile is MiracleWreckageThrow mw)
                        {
                            if(mw.Hit == 1 && mw.target == target.whoAmI)
                            {
                                amount++;
                                if (mw.hitTime < last)
                                {
                                    lastProj = p;
                                    last = mw.hitTime;
                                }
                            }
                        }
                    }
                }
                if (amount > MiracleWreckage.MaxPlugged)
                {
                    if (lastProj != null)
                    {
                        lastProj.rotation = Projectile.velocity.ToRotation();
                        ((MiracleWreckageThrow)lastProj.ModProjectile).PopOut();
                    }
                }
                Hit = 1;
                this.target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                HitEffect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 90);
            }
        }
        public void PopOut()
        {
            for (int i = 0; i < 16; i++)
            {
                Color clr = Main.rand.NextBool() ? new Color(200, 200, 255) : new Color(190, 140, 255);
                GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(0.12f) * Main.rand.NextFloat(32, 64), false, 16, Main.rand.NextFloat(0.5f, 1) * 0.08f, clr, new Vector2(0.16f, 1)));
            }
            if(Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(0.5f) * 3, ModContent.ProjectileType<MiracleWreckagePopOut>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.Kill();
        }
        
        public void HitEffect(Vector2 position)
        {
            for(int i = 0; i < 16; i++)
            {
                Color clr = Main.rand.NextBool() ? new Color(200, 200, 255) : new Color(190, 140, 255);
                GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(position, Projectile.rotation.ToRotationVector2().RotatedByRandom(0.3f) * -1 * Main.rand.NextFloat(12, 36), false, 16, Main.rand.NextFloat(0.5f, 1) * 0.08f, clr, new Vector2(0.16f, 1)));
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return Hit != 1;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                CEUtils.PlaySound("DemonSwordSwing1", Main.rand.NextFloat(1.5f, 1.9f), Projectile.Center);
            }
            Projectile.localAI[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Hit == 1)
            {
                if (!target.ToNPC().active)
                { PopOut(); return; }
                if (Projectile.timeLeft < 30)
                {
                    StrokeAlpha = float.Lerp(StrokeAlpha, 0, 0.01f);
                    Projectile.Opacity -= 1 / 30f;
                    Projectile.velocity *= 0.95f;
                }
                else
                    StrokeAlpha = float.Lerp(StrokeAlpha, 1, 0.3f);
                Projectile.Center = target.ToNPC().Center + offset;
            }
            else if (Hit == 2)
            {
                StrokeAlpha *= 0.99f;
                Projectile.Opacity -= 1 / 30f;
            }
            else
            {
                Color clr = Main.rand.NextBool() ? new Color(200, 200, 255) : new Color(190, 140, 255);
                GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center - Projectile.rotation.ToRotationVector2() * 90 + Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-16, 16), Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(12, 36), false, 16, Main.rand.NextFloat(0.5f, 1) * 0.02f, clr, new Vector2(0.16f, 1) * Projectile.Opacity * 1f));
                for (float j = 0; j < 1; j += 0.5f)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        clr = Main.rand.NextBool() ? new Color(235, 40, 180) : new Color(255, 40, 180);
                        Vector2 offset = Projectile.velocity * -j;
                        Vector2 safeVel = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                        Vector2 dustVel = safeVel.RotatedBy(MathHelper.ToRadians(70 * (i == 0 ? 1 : -1))) * 10;
                        Vector2 ofs2 = -dustVel * j * (Projectile.localAI[0] % Projectile.MaxUpdates);
                        if (true)
                        {
                            VelChangingSpark spark = new VelChangingSpark(Projectile.Center + ofs2 + offset + safeVel * 120, -dustVel, -Projectile.velocity, "CalamityMod/Particles/BloomCircle", 5, 0.2f, (clr) * 0.95f, new Vector2(1.2f, 1f), true, false, 0, false, 1.0f, 0.22f);
                            GeneralParticleHandler.SpawnParticle(spark);

                            VelChangingSpark spark2 = new VelChangingSpark(Projectile.Center + ofs2 + offset + safeVel * 120, -dustVel, -Projectile.velocity, "CalamityMod/Particles/BloomCircle", 10, 0.2f, clr * 0.65f, new Vector2(1.2f, 1f), true, false, 0, false, 1.0f, 0.22f);
                            GeneralParticleHandler.SpawnParticle(spark2);
                        }
                        float rot = Projectile.rotation + (MathHelper.TwoPi * i);
                        Vector2 vel = (Utils.MoveTowards(-Projectile.velocity, new Vector2(0, -130).RotatedBy(rot).RotatedBy(-1.3f * Projectile.direction), (Utils.GetLerpValue(5, 2, Projectile.velocity.Length(), true))));
                        if (i == 0)
                        {
                            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + offset + new Vector2(0, -70).RotatedBy(rot), Main.rand.NextBool(4) ? 278 : ModContent.DustType<LightDust>());
                            dust2.noGravity = (dust2.type == 278 ? false : true);
                            dust2.scale = dust2.type == 278 ? 0.75f : 0.9f;
                            dust2.color = Main.rand.NextBool() ? Color.BlueViolet : clr;
                            dust2.velocity = (vel * 2).RotatedByRandom(0.4f);
                            dust2.position = Projectile.Center;
                        }
                    }
                }
                if (Projectile.timeLeft < 30)
                {
                    StrokeAlpha = float.Lerp(StrokeAlpha, 0, 0.01f);
                    Projectile.Opacity -= 1 / 30f;
                    Projectile.velocity *= 0.95f;
                }
                else
                {
                    StrokeAlpha = float.Lerp(StrokeAlpha, 1, 0.1f);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Vector2 origin = tex.Size() / 2f;
            float rotation = Projectile.rotation + MathHelper.PiOver4;
            for(float i = 0; i < 360; i += 60)
            {
                float rot = MathHelper.ToRadians(i) + Main.GlobalTimeWrappedHourly * 16;
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + CEUtils.randomPointInCircle(2) + rot.ToRotationVector2() * 4, null, Color.White * StrokeAlpha * Projectile.Opacity, rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public float StrokeAlpha = 0;
    }
    public class MiracleExplosion : ModProjectile
    {
        public int endTime = 25;

        public ref float time => ref Projectile.ai[0];

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 700;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5 ;
            Projectile.scale = 0.55f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool VisualsOrange = false;
            Color FXColor = Main.rand.NextBool() ? Color.MediumVioletRed : Color.Violet;

            if (time < (float)endTime)
            {
                GeneralParticleHandler.SpawnParticle(new GlowSquareParticle(Projectile.Center, Vector2.Zero, false, 32, 28, FXColor, true, Main.rand.NextFloat(-0.2f, 0.2f)));
                time = endTime;
                float lerpValue = Utils.GetLerpValue(35f, 0f, time, clamped: true);
                float num = 3f;
                float num2 = 360f / num;
                for (int i = 0; (float)i < num; i++)
                {
                    MathHelper.ToRadians((float)i * num2);
                    Vector2 vector = CalamityUtils.RandomVelocity(100f, 70f, 250f, 0.04f);
                    vector *= Main.rand.NextFloat(15f, 30f) * lerpValue;
                    GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center + vector * 2.5f, -vector * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, affectedByGravity: false, 14, Main.rand.NextFloat(1.1f, 1.25f) - 0.2f * lerpValue, FXColor));
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vector * 2.5f, 278, -vector * Main.rand.NextFloat(0.08f, 0.12f) * 1.5f, 0, default(Color), Main.rand.NextFloat(0.4f, 0.6f));
                    dust.noGravity = true;
                    dust.color = FXColor;
                }
            }

            if (time >= (float)endTime)
            {
                Projectile.scale *= 1.15f;
                for (int j = 0; j < 3; j++)
                {
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, FXColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0.7f * (float)(j + 1) * Projectile.scale, 1f * Projectile.scale, 18));
                    GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0.35f * (float)(j + 1) * Projectile.scale, 0.5f * Projectile.scale, 18));
                }

                for (int k = 0; k < 6; k++)
                {
                    GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center, new Vector2((!VisualsOrange) ? 1 : 0, VisualsOrange ? 1 : 0) * -5f * ((k % 2 != 0) ? 1 : (-1)), affectedByGravity: false, 15, (0.08f - (float)k * 0.01f) * Projectile.scale, FXColor, new Vector2(5f, 0.8f), quickShrink: true, glow: false, 1.2f));
                }

                if (time == (float)endTime)
                {
                    if (VisualsOrange)
                    {
                        GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, FXColor, "CalamityMod/Particles/GlowSquareParticleBig", Vector2.One, MathF.PI / 4f, 0f, 2.2f, 22));
                        GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, FXColor, "CalamityMod/Particles/GlowSquareParticleBig", Vector2.One, MathF.PI / 4f, 0f, 1.2f, 47));
                    }
                    else
                    {
                        GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, FXColor, "CalamityMod/Particles/HighResHollowCircleHardEdge", Vector2.One, MathF.PI / 4f, 0f, 0.3f, 22));
                    }

                    for (int l = 0; l < 30; l++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(Projectile.Center, VisualsOrange ? 267 : 278, Vector2.One.RotatedByRandom(100.0) * Main.rand.NextFloat(2.5f, 15f));
                        dust2.scale = Main.rand.NextFloat(0.85f, 1.15f) * (VisualsOrange ? 1f : 1.2f);
                        dust2.noGravity = VisualsOrange;
                        dust2.color = Color.Lerp(Color.White, FXColor, 0.5f);
                        if (VisualsOrange)
                        {
                            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.One.RotatedByRandom(100.0) * Main.rand.NextFloat(5.5f, 20f), FXColor, "CalamityMod/Particles/GlowSquareParticle", Vector2.One, MathF.PI / 4f, Main.rand.NextFloat(1.3f, 3.8f), 0.2f, 38));
                        }
                        else
                        {
                            GeneralParticleHandler.SpawnParticle(new CustomSpark(Projectile.Center, Vector2.One.RotatedByRandom(100.0) * Main.rand.NextFloat(5.5f, 20f), "CalamityMod/Particles/Sparkle", affectedByGravity: false, 38, Main.rand.NextFloat(2.2f, 4.8f), FXColor, new Vector2(0.4f, Main.rand.NextFloat(0.9f, 1.4f)), useAddativeBlend: true, glowCenter: true, 0f, fadeIn: false, affectedByLight: false, 0.1f));
                        }
                    }
                }
            }

            time += 1f;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    public class MiracleWreckageHeld : ModProjectile
    {
        public class MWParticle
        {
            public Vector2 offset = Vector2.Zero;
            public Vector2 velocity;
            public Color color;
            public float scale = Main.rand.NextFloat(1f, 1.6f);
            public MWParticle(Vector2 vel)
            {
                velocity = vel;
                int rcl = Main.rand.Next(2);
                if (rcl == 0)
                {
                    color = Main.rand.NextBool() ? Color.Purple : Color.Pink;
                }
                else
                {
                    color = Main.rand.NextBool() ? Color.MediumPurple : Color.Red;
                }
                
                color *= 0.6f;
            }
            public int counter = 0;
            public float alpha = 1;
            public void update()
            {
                counter++;
                offset += velocity;
                if (counter > 4)
                {
                    alpha *= 0.7f;
                }
                scale *= 0.99f;
            }
        }
        public List<MWParticle> particles = new List<MWParticle>();
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Miracle/MiracleWreckage";
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 100000;
            Projectile.MaxUpdates = 14;
            Projectile.light = 1;
        }
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int count = 0;
            int type = ModContent.ProjectileType<MiracleWreckageThrow>();
            foreach(Projectile p in Main.ActiveProjectiles)
            {
                if(p.type == type && p.owner == Projectile.owner)
                {
                    count++;
                    if (p.ModProjectile is MiracleWreckageThrow mw)
                        mw.PopOut();
                    for(int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.velocity.normalize().RotatedByRandom(1) * Main.rand.NextFloat(38, 46), ModContent.ProjectileType<MiracleVortex>(), Projectile.damage / 2, 0, Projectile.owner);
                    }
                }
            }
            if(count >= 6)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<Blackhole>(), Projectile.damage / 16, 0, Projectile.owner);
            }
            ScreenShaker.AddShake(new ScreenShaker.ScreenShake(-(target.Center - Projectile.Center).normalize(), 26));
            CEUtils.PlaySound("DemonSwordInsaneImpact", Main.rand.NextFloat(0.8f, 1.4f), target.Center);
            CEUtils.PlaySound("HalleysInfernoHit", Main.rand.NextFloat(0.6f, 1.12f), target.Center, 4, 1f * CEUtils.WeapSound, path: "CalamityMod/Sounds/Item/");
            for (int i = 0; i < 32; i++)
            {
                Color clr = Main.rand.NextBool() ? new Color(240, 240, 255) : new Color(210, 160, 255);
                EParticle.NewParticle(new ShadeDashParticle() { c1 = clr, c2 = clr, TL = 12 }, target.Center + CEUtils.randomPointInCircle(26),
                    (target.Center - Projectile.Center).normalize().RotatedByRandom(0.2f) * Main.rand.NextFloat(10, 64), Color.White, 1, 1, true, BlendState.NonPremultiplied, 0, 16);
                ;

            }
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(220, 220, 255), 3, 1, true, BlendState.Additive, 0, 6);
            EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 255, 255), 1.6f, 1, true, BlendState.Additive, 0, 6);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 8f;
        }
        public float length = 0;
        public int Dir = -1;
        public int swing = 0;
        public float vsAlpha = 0;
        public float rotVel = 0;
        public bool rcl = true;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Dir);
            writer.Write(swing);
            writer.Write(rotVel);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Dir = reader.ReadInt32();
            swing = reader.ReadInt32();
            rotVel = reader.ReadSingle();
        }
        public bool flag = false;
        public bool flag2 = true;
        public override void AI()
        {
            if(flag2)
            {
                flag2 = false;
                Dir = Projectile.velocity.X > 0 ? -1 : 1;
            }
            if(!flag)
                length = float.Lerp(length, 1, 0.01f);
            Player owner = Projectile.GetOwner();
            Projectile.timeLeft = 3;
            owner.Calamity().mouseWorldListener = true;
            Projectile.Center = Projectile.GetOwner().MountedCenter;
            float rot = (owner.Calamity().mouseWorld - Projectile.Center).ToRotation();
            float targetRot = rot + Dir * 2f;
            if (Projectile.localAI[1]++ == 0)
                Projectile.rotation = rot + Dir * 1.2f;
            float speed = owner.GetTotalAttackSpeed(DamageClass.Melee);
            if(swing < 0)
                Projectile.rotation = CEUtils.RotateTowardsAngle(Projectile.rotation, targetRot, 0.01f, false);
            if (flag)
            {
                length *= 0.995f;
                length -= 0.002f;
                if (length <= 0.04f)
                    Projectile.Kill();
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (!rcl && Main.mouseRight && swing < -16)
                {
                    flag = true;
                }
                rcl = Main.mouseRight;
            }
            Projectile.rotation += rotVel * speed;
            rotVel *= (float)(Math.Pow(0.987f, speed));
            if(swing < 0)
                Projectile.velocity = rot.ToRotationVector2() * 16;
            if (Main.myPlayer == Projectile.owner && !flag)
            {
                if (swing-- < -24 * Projectile.MaxUpdates && Main.mouseLeft)
                {
                    Dir *= -1;
                    Projectile.ResetLocalNPCHitImmunity();
                    swing = (int)(36 * Projectile.MaxUpdates / speed);
                    rotVel = Dir * 0.062f;
                    CEUtils.PlaySound("DemonSwordSwing1", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        CEUtils.SyncProj(Projectile.whoAmI);
                }
            }
            if (Projectile.localAI[1] % 5 == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(0.2f, float.Max(0.2f, length)) * 410, Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(12, 18) * length, true, 32, Main.rand.NextFloat(0.04f, 0.054f) * length, Color.MediumVioletRed, new Vector2(0.5f, 1)));
                }
            }
            if (swing > 0)
            {
                if (swing > 25 * Projectile.MaxUpdates)
                { vsAlpha += 0.05f * speed; if (vsAlpha > 1) vsAlpha = 1; }
                if (swing < 20 * Projectile.MaxUpdates)
                    vsAlpha = swing / (20f * Projectile.MaxUpdates);
            }
            else
                vsAlpha = 0;

            for (int i = particles.Count - 1; i >= 0; i--)
                {
                    particles[i].update();
                    if (particles[i].counter > 60 * (Projectile.ai[1] + 0.5f))
                    {
                        particles.RemoveAt(i);
                    }

                }
            for (int i = 0; i < 1; i++)
            {
                particles.Add(new MWParticle(new Vector2(Main.rand.NextFloat(11, 12) * length, 0).RotatedByRandom(0.02f)) { offset = CEUtils.randomPointInCircle(10) });
                particles[particles.Count - 1].scale *= 1.3f * length;
            }
            if (odr.Count > 2600)
            {
                odr.RemoveAt(0);
            }
            if (Projectile.velocity.X > 0)
            {
                owner.direction = 1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            else
            {
                owner.direction = -1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            odr.Add(Projectile.rotation);
            if (odr.Count > 44)
            {
                odr.RemoveAt(0);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D trail = CEUtils.getExtraTex("StreakGoop");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            float MaxUpdateTimes = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;

            {
                for (int i = 0; i < odr.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(220 * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(140 * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color2"].SetValue((Color.LightBlue).ToVector4());
                    shader.Parameters["color1"].SetValue((Color.MediumPurple).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(vsAlpha);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }
            {
                ve.Clear();
                for (int i = 0; i < odr.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(1000 * (0.5f + Projectile.ai[1]) * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(Projectile.Center - Main.screenPosition + (new Vector2(800 * (0.5f + Projectile.ai[1]) * Projectile.scale, 0).RotatedBy(odr[i])),
                          new Vector3((i) / ((float)odr.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;
                    Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color2"].SetValue((new Color(240, 200, 255)).ToVector4());
                    shader.Parameters["color1"].SetValue((Color.Red).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GameUpdateCount * 2);
                    shader.Parameters["alpha"].SetValue(vsAlpha);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }


            int dir = (int)(Projectile.ai[0]);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            float MaxUpdateTime = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;

            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor, rot, origin, Projectile.scale * scale, effect);

            Texture2D g = CEUtils.getExtraTex("Glow");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Texture2D c = CEUtils.getExtraTex("SemiCircularSmear");
            float alphac = vsAlpha * 0.65f;
            float crot = Projectile.rotation + Dir * -1.2f;
            Main.spriteBatch.Draw(c, Projectile.Center - Main.screenPosition, null, Color.MediumPurple * alphac, crot, c.Size() / 2f, 13f * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(c, Projectile.Center - Main.screenPosition, null, Color.Red * alphac * 0.5f, crot, c.Size() / 2f, 5f * (Projectile.ai[1] + 0.5f), SpriteEffects.None, 0);

            Main.spriteBatch.Draw(c, Projectile.Center - Main.screenPosition, null, Color.Pink * alphac * 0.6f, crot, c.Size() / 2f, 10 * (Projectile.ai[1] + 0.5f), SpriteEffects.None, 0);
            foreach (var p in particles)
            {
                Main.spriteBatch.Draw(g, Projectile.Center + p.offset.RotatedBy(Projectile.rotation) - Main.screenPosition, null, p.color, Projectile.rotation + p.velocity.ToRotation(), new Vector2(40, 128), new Vector2(1f, 0.16f) * p.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (swing < 0)
                return false;
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * scale * 900 * (Projectile.ai[1] + 0.5f), targetHitbox, 420);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.scale * scale * 860 * (Projectile.ai[1] + 0.5f), 128, DelegateMethods.CutTiles);
        }
    }
    public class MiracleShoot : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.MaxUpdates = 4;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft < 16)
                return false;
            return CanHitNPC(target);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Opacity = Projectile.timeLeft / 30f;

            Projectile.velocity *= 0.96f;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trail = CEUtils.getExtraTex("MotionTrail5");
            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<Vector2> p1 = new List<Vector2>();
            List<Vector2> p2 = new List<Vector2>();
            for (float i = -1; i <= 1; i += 0.01f)
            {
                p2.Add(((i * 1f).ToRotationVector2() * new Vector2(1.2f, 1)).RotatedBy(Projectile.rotation) * 10);
                p1.Add(((i * 1.6f).ToRotationVector2() * new Vector2(1.2f, 1)).RotatedBy(Projectile.rotation) * 320);
            }
            for (int i = 0; i < p1.Count; i++)
            {
                Color b = new Color(230, 220, 255);
                ve.Add(new ColoredVertex(Projectile.Center + Projectile.rotation.ToRotationVector2() * -180 - Main.screenPosition + p1[i],
                      new Vector3((i) / ((float)p1.Count - 1), 1, 1),
                      b));
                ve.Add(new ColoredVertex(Projectile.Center + Projectile.rotation.ToRotationVector2() * -180 - Main.screenPosition + p2[i],
                      new Vector3((i) / ((float)p1.Count - 1), 0, 1),
                      b));
            }
            if (ve.Count >= 3)
            {
                var gd = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail4", AssetRequestMode.ImmediateLoad).Value;
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                shader.Parameters["color2"].SetValue(Color.AliceBlue.ToVector4());
                shader.Parameters["color1"].SetValue((Color.MediumPurple).ToVector4());
                shader.Parameters["alpha"].SetValue(Projectile.Opacity);
                shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 200);
                gd.Textures[1] = CEUtils.getExtraTex("PatchyTallNoise");
                shader.CurrentTechnique.Passes["EffectPass"].Apply();

                gd.Textures[0] = trail;

                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);

            }
            return false;
        }
    }

}
