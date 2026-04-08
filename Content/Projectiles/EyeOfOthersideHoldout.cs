using CalamityMod;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    #region proj

    public class GhostFireMagic : ModProjectile
    {
        public bool ableToHit = true;

        public NPC target;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 200;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage()
        {
            return ableToHit ? null : false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.CanBeChasedBy() ? null : false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0] += 1f / (float)(Projectile.extraUpdates + 1);
            if (Projectile.penetrate < 200)
            {
                if (Projectile.timeLeft > 60)
                {
                    Projectile.timeLeft = 60;
                }
                Projectile.velocity *= 0.88f;
            }
            else if (Projectile.localAI[0] < 60f)
            {
                Projectile.velocity *= 0.93f;
            }
            else
            {
                FindTarget(player);
            }
            if (Projectile.timeLeft <= 20)
            {
                ableToHit = false;
            }
        }
        public void FindTarget(Player player)
        {
            float num = 3000f;
            bool flag = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC nPC = Main.npc[player.MinionAttackTargetNPC];
                if (nPC.CanBeChasedBy(Projectile))
                {
                    float num2 = Vector2.Distance(nPC.Center, Projectile.Center);
                    if (num2 < num)
                    {
                        num = num2;
                        flag = true;
                        target = nPC;
                    }
                }
            }

            if (!flag)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(Projectile))
                    {
                        float num3 = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (num3 < num)
                        {
                            num = num3;
                            flag = true;
                            target = nPC2;
                        }
                    }
                }
            }

            if (!flag)
            {
                Projectile.velocity *= 0.98f;
            }
            else
            {
                KillTheThing(target);
            }
        }

        public void KillTheThing(NPC npc)
        {
            Projectile.velocity = Projectile.SuperhomeTowardsTarget(npc, 50f / (float)(Projectile.extraUpdates + 1), 60f / (float)(Projectile.extraUpdates + 1), 1f / (float)(Projectile.extraUpdates + 1));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float amount = (float)Math.Cos((float)Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + (float)i / (float)Projectile.oldPos.Length * MathF.PI) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.Cyan, Color.LightBlue, amount) * 0.4f;
                color.A = 0;
                Vector2 position = Projectile.oldPos[i] + value.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-28f, -28f);
                Color color2 = color;
                Color color3 = color * 0.5f;
                float num = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * (MathF.PI * 2f));
                num *= MathHelper.Lerp(0.15f, 1f, 1f - (float)i / (float)Projectile.oldPos.Length);
                if (Projectile.timeLeft <= 60)
                {
                    num *= (float)Projectile.timeLeft / 60f;
                }

                Vector2 vector = new Vector2(1f) * num;
                Vector2 vector2 = new Vector2(1f) * num * 0.7f;
                color2 *= num;
                color3 *= num;
                Main.EntitySpriteDraw(value, position, null, color2, 0f, value.Size() * 0.5f, vector * 0.6f, SpriteEffects.None);
                Main.EntitySpriteDraw(value, position, null, color3, 0f, value.Size() * 0.5f, vector2 * 0.6f, SpriteEffects.None);
            }

            return false;
        }
    }
    #endregion
    public class EyeOfOthersideHoldout : ModProjectile
    {
        public class SparkleParticle
        {
            public bool kill = false;
            public float rot;
            public float size;
            public float addSize = 1;
            public void update()
            {
                size += addSize;
                addSize -= 0.12f;
                if (size < 0)
                {
                    kill = true;
                }
            }

            public SparkleParticle()
            {
                rot = CEUtils.randomRot();
                size = 0;
            }
        }
        public List<SparkleParticle> particles = new List<SparkleParticle>();
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player owner = Projectile.owner.ToPlayer();
            if (Projectile.ai[0]++ > 5 && Projectile.ai[0] % 5 == 0)
            {
                if (owner.CheckMana(12, true))
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.35f) * 0.6f, ModContent.ProjectileType<GhostFireMagic>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Main.projectile[p].DamageType = Projectile.DamageType;
                    }
                    if (!Main.dedServ)
                    {
                        CEUtils.PlaySound("soulshine", Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center, 8, 0.3f);
                    }
                    particles.Add(new SparkleParticle());
                }
                else
                {
                    Projectile.Kill();
                }
            }
            if (!owner.channel)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft = 3;
            }
            foreach (SparkleParticle particle in particles)
            {
                particle.update();
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].kill)
                {
                    particles.RemoveAt(i);
                }
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 nv = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.One) * 42;
                if (nv != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = nv;

            }
            if (Projectile.velocity.X > 0)
            {
                Projectile.direction = 1;
                owner.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
                owner.direction = -1;
            }
            Player player = Projectile.owner.ToPlayer();
            Projectile.Center = owner.MountedCenter + player.gfxOffY * Vector2.UnitY + Projectile.velocity.SafeNormalize(Vector2.Zero) * 28;
            owner.itemRotation = Projectile.rotation * Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
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
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public float counter = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.ToRadians(90), texture.Size() / 2, Projectile.scale, SpriteEffects.None);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D light = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Glow").Value;
            Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition * Projectile.scale, null, Color.LightBlue * 0.7f, 0, light.Size() / 2, 0.5f * Projectile.scale * (1 + (float)Math.Cos((counter) * 0.02f) * 0.2f), SpriteEffects.None, 0);
            Texture2D spark = CEUtils.getExtraTex("Sparkle");
            foreach (SparkleParticle p in particles)
            {
                Main.spriteBatch.Draw(spark, Projectile.Center - Main.screenPosition * Projectile.scale, null, Color.White * 0.6f, p.rot, spark.Size() / 2, 0.06f * Projectile.scale * p.size, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(spark, Projectile.Center - Main.screenPosition * Projectile.scale, null, Color.White * 0.6f, p.rot + MathHelper.PiOver2, spark.Size() / 2, 0.06f * Projectile.scale * p.size, SpriteEffects.None, 0);

            }

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
    }

}