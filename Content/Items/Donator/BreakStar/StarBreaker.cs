using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.BreakStar
{
    public class StarBreaker : ModItem, IDevItem
    {
        public string DevName => "锯角";

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * 2f, null, Color.Blue with { A = 0 }, rotation, tex.Size() / 2, scale, 0, 0f);

            spriteBatch.Draw(tex, position, iFrame, Color.White, rotation, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.Blue);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Nadir>()
                .AddIngredient<FadingRunestone>()
                .AddIngredient<GalacticaSingularity>(4)
                .AddTile<VoidWellTile>()
                .Register();
        }

        public override void SetDefaults()
        {
            Item.width = 256;
            Item.height = 256;
            Item.damage = 3600;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<StarBreakerHeld>();
            Item.shootSpeed = 42;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.crit = 12;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                var p = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, (Main.MouseWorld - player.Center).normalize() * Item.shootSpeed, Item.shoot, player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI).ToProj();
                p.originalDamage = Item.damage;

            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
    public class StarBreakerHeld : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(ModContent.GetInstance<TrueMeleeDamageClass>(), false, -1);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 60;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 480 * Projectile.scale, Projectile.Center, targetHitbox, 140);
        }
        public override void CutTiles()
        {
            if (Projectile.GetOwner().channel)
                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 480 * Projectile.scale, 128, DelegateMethods.CutTiles);
        }
        public Rope rope = null;
        public int AttackCount = 0;
        public float AttackTime = 0;
        public int AttackDelay = 0;
        public float RotP = 0;
        public float BaseScale = 0;
        public float num = 0;
        public override bool? CanHitNPC(NPC target)
        {
            return (Projectile.GetOwner().channel) ? null : false;
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            if (player.HeldItem.ModItem is StarBreaker)
            {
                Projectile.damage = (int)player.GetTotalDamage(Projectile.DamageType).ApplyTo(Projectile.originalDamage);
                Projectile.CritChance = player.GetWeaponCrit(player.HeldItem);
            }
            player.direction = Math.Sign(Projectile.velocity.X);

            Projectile.StickToPlayer(player.channel ? 1 : 0.25f);

            Projectile.rotation += RotP;
            Projectile.Center = player.HandPosition.Value;
            Projectile.Center += Projectile.rotation.ToRotationVector2() * -160 * Projectile.scale;
            player.direction = (Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1);
            player.itemRotation = (Projectile.velocity * player.direction).ToRotation();
            if (BaseScale == 0)
                BaseScale = Projectile.scale;

            if (Main.myPlayer == Projectile.owner)
            {
                if (Main.mouseLeft && !Main.LocalPlayer.mouseInterface)
                {
                    player.channel = true;
                }
            }
            if (!player.dead && player.HeldItem.ModItem is StarBreaker)
            {
                Projectile.timeLeft = 2;
            }
            if (AttackCount % 4 == 3 && AttackDelay <= 0)
            {
                Projectile.scale = BaseScale * 1.5f;
            }
            else
            {
                Projectile.scale = BaseScale;
            }
            if (player.channel)
            {
                player.itemTime = player.itemAnimation = 3;
                AttackDelay--;
                if (AttackDelay < 0 || AttackTime > 0)
                {
                    int ItemTime = player.HeldItem.useTime;
                    float add = player.GetTotalAttackSpeed(Projectile.DamageType) * (1f / ItemTime) * (AttackCount % 4 == 3 ? 2 : 3f);
                    if (AttackTime == 0)
                    {
                        for (int i = 0; i < 4; i++)
                            SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 10 }, Projectile.Center);
                        if (!(AttackCount % 4 == 3))
                            RotP = Main.rand.NextFloat(-0.3f, 0.3f);
                        num = (RotP * -2f) / (1 / add);
                        Projectile.ResetLocalNPCHitImmunity();
                        PlaySound = false;
                        GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(Projectile.Center + Projectile.velocity.normalize() * 500 * Projectile.scale, player.velocity + Projectile.velocity.normalize() * -32 * Projectile.scale, new Color(255, 80, 80), new Vector2(0.3f, 1), Projectile.velocity.ToRotation(), 0.1f, 1f * Projectile.scale, 16));
                    }
                    RotP += num;
                    AttackTime += add;

                    if (AttackTime > 1)
                        AttackTime = 1;
                    Projectile.Center += Projectile.rotation.ToRotationVector2() * 150 * Projectile.scale * CEUtils.Parabola(AttackTime, 1);//(AttackTime > 0.5f ? (1 - (AttackTime - 0.5f) * 2) : AttackTime * 2);

                    if (AttackTime >= 1)
                    {
                        AttackTime = 0;
                        AttackCount++;
                        if (AttackCount % 4 == 3 || AttackCount % 4 == 0)
                            AttackDelay = 5;
                    }
                }
                else { RotP = 0; }
            }
            else
            {
                RotP = 0;
                AttackDelay = 0;
                AttackCount = 0;
                AttackTime = 0;
            }


            Vector2 ropePoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * 234 * Projectile.scale;
            if (rope == null)
            {
                rope = new Rope(ropePoint, 10, 11f * Projectile.scale, new Vector2(0, 0.5f), 0.2f, 64);
            }
            Vector2 lst = rope.Start;
            rope.gravity = (Vector2.Lerp(Vector2.UnitY, -Projectile.rotation.ToRotationVector2(), 0.56f)).RotatedBy((float)Math.Sin(Main.GameUpdateCount * 0.028f) * 0.3f) * 0.6f;
            for (float r = 0.5f; r <= 1; r += 0.5f)
            {
                rope.Start = Vector2.Lerp(lst, ropePoint, r);
                rope.Update();
            }
            var points = rope.GetPoints();
            odp.Clear();
            odp.Add(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                for (float j = 0.25f; j <= 1f; j += 0.25f)
                {
                    odp.Add(Vector2.Lerp(points[i - 1], points[i], j));
                }

            }
        }
        public List<Vector2> odp = new();
        public bool PlaySound = false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 80 + target.defense / 2;
            if (AttackCount % 4 == 3)
            {
                modifiers.SetCrit();
                modifiers.SourceDamage *= 3;
                modifiers.ArmorPenetration += 200;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<MarkedforDeath>(180);

            if (!PlaySound)
            {
                ScreenShaker.AddShake(new ScreenShaker.ScreenShake(Projectile.velocity.normalize() * -4, 3));
                PlaySound = true;
                CEUtils.PlaySound("spearImpact", Main.rand.NextFloat(0.8f, 1.4f), target.Center);
            }
            for (int i = 0; i < 32; i++)
            {
                float sparkScale2 = Main.rand.NextFloat(1.4f, 2.4f);
                Vector2 sparkVelocity2 = Projectile.rotation.ToRotationVector2().RotatedByRandom(0.2f) * 64 * Main.rand.NextFloat(0.2f, 1);
                if (Main.rand.NextBool(5))
                {
                    AltSparkParticle spark = new AltSparkParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2 * (1f), false, 12, sparkScale2 * (1.4f), Color.Crimson);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                else
                {
                    LineParticle spark = new LineParticle(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), sparkVelocity2, false, 8, sparkScale2 * (Projectile.frame == 7 ? 1.4f : 1f), Main.rand.NextBool() ? Color.Red : new Color(255, 190, 190));
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (odp != null)
            {
                float w = 7 * Projectile.scale;
                int xp = 0;
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = Lighting.GetColor((odp[0] / 16).ToPoint());
                b = b * Projectile.GetOwner().Entropy().alpha;
                ve.Add(new ColoredVertex(new Vector2(xp, 0) + odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * w,
                          new Vector3((float)0, 1, 1),
                          b));
                ve.Add(new ColoredVertex(new Vector2(xp, 0) + odp[0] - Main.screenPosition + (odp[1] - odp[0]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * w,
                      new Vector3((float)0, 0, 1),
                      b));
                for (int i = 1; i < odp.Count; i++)
                {
                    b = Lighting.GetColor((odp[i] / 16).ToPoint());
                    ve.Add(new ColoredVertex(new Vector2(xp, 0) + odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * w,
                          new Vector3((float)(i + 1) / odp.Count, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(new Vector2(xp, 0) + odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * w,
                          new Vector3((float)(i + 1) / odp.Count, 0, 1),
                          b));

                }
                Texture2D texst = this.getTextureAlt("String");
                var gd = Main.graphics.GraphicsDevice;
                gd.Textures[0] = texst;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            var tex = Projectile.GetTexture();
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver4, new Vector2(32, tex.Height - 32), Projectile.scale, 0f, 0f);
            Texture2D arrow = CEUtils.getExtraTex("SpearArrow");
            Texture2D glow = CEUtils.getExtraTex("SpearArrowGlow");
            float arrowAlpha = CEUtils.Parabola(AttackTime, 1) - 0.6f;
            if (arrowAlpha < 0)
                arrowAlpha = 0;
            arrowAlpha /= 0.4f;
            arrowAlpha *= 0.7f;
            Color applyAlpha(Color bc)
            {
                return new Color(bc.R, bc.G, bc.B, (int)(255 * arrowAlpha));
            }
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(arrow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 330 + CEUtils.randomPointInCircle(24), null, applyAlpha(Color.Red), Projectile.rotation + Main.rand.NextFloat(-0.22f, 0.22f), arrow.Size() / 2f, new Vector2(2f, 1) * Projectile.scale * 0.4f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 360, null, new Color(255, 16, 16) * arrowAlpha, Projectile.rotation, arrow.Size() / 2f, new Vector2(1.8f, 1) * Projectile.scale * 0.6f * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(arrow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 360, null, applyAlpha(Color.Red), Projectile.rotation, arrow.Size() / 2f, new Vector2(1.8f, 1) * Projectile.scale * 0.6f * Projectile.scale, SpriteEffects.None, 0);


            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 360, null, Color.White * arrowAlpha * 2, Projectile.rotation, arrow.Size() / 2f, new Vector2(1.7f, 0.4f) * Projectile.scale * 0.6f * Projectile.scale, SpriteEffects.None, 0);
            //Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            //Main.spriteBatch.Draw(arrow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 360, null, applyAlpha(Color.White), Projectile.rotation, arrow.Size() / 2f, new Vector2(1.7f, 0.4f) * Projectile.scale * 0.6f * Projectile.scale, SpriteEffects.None, 0);

            //Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 440, null, Color.White * arrowAlpha * 0.5f, Projectile.rotation, arrow.Size() / 2f, new Vector2(1.8f, 1f) * Projectile.scale * 0.24f * Projectile.scale, SpriteEffects.None, 0);
            //Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            //Main.spriteBatch.Draw(arrow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 440, null, applyAlpha(Color.White), Projectile.rotation, arrow.Size() / 2f, new Vector2(1.8f, 1f) * Projectile.scale * 0.24f * Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();
            List<Vector2> points = new();
            for (int i = AttackCount % 4 == 3 ? 100 : 250; i <= 450; i += AttackCount % 4 == 3 ? 50 : 25)
            {
                points.Add(Projectile.Center + Projectile.rotation.ToRotationVector2() * i * Projectile.scale + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * Main.rand.Next(-60, 61) * ((450 - i) / (AttackCount % 4 != 3 ? 200f : 350f)));
            }
            CEUtils.DrawLines(points, Color.White * (arrowAlpha / 0.5f), 2 * Projectile.scale);
            return false;
        }
    }
}