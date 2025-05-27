using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Fractal
{
    public class FinalFractal : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 960;
            Item.crit = 5;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 48;
            Item.height = 60;
            Item.useTime = Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FinalFractalHeld>();
            Item.shootSpeed = 12f;
            Item.ArmorPenetration = 36;
        }
        public int atkType = 0;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.chaosState)
            {
                return false;
            }
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Util.PlaySound("VoidAnticipation", 1, position);
                player.AddBuff(BuffID.ChaosState, 5 * 60);
                Projectile.NewProjectile(source, position, velocity * 4, ModContent.ProjectileType<VoidSlash>(), damage * 30, 0, player.whoAmI);
                return false;
            }
            int at = 2;
            if(atkType == 0 || atkType == 2)
            {
                at = -1;
            }
            if(atkType == 1 || atkType == 3)
            {
                at = 1;
            }
            if (atkType == 5)
            {
                at = 3;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, at, 0, Main.MouseWorld.Distance(position) + 180);
            atkType += 1;
            if(atkType > 5)
            {
                atkType = 0;
            }
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<SpiritFractal>()
                .AddIngredient<VoidBar>(10)
                .AddIngredient<VoidAnnihilate>()
                .AddTile<CosmicAnvil>().Register();
        }
    }

    public class FinalFractalHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Fractal/FinalFractal";
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 22;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 100000;
            Projectile.extraUpdates = 9;
        }
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;
        public float length = 760;
        public float spawnProjCounter = 0;
        public override void AI()
        {
            Player owner = Projectile.getOwner();
            float MaxUpdateTimes = owner.itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            if (!(Projectile.ai[0] == 3 && OnNPC != null && OnNPCTime > 0))
            {
                counter += 1 * (Projectile.ai[0] == 3 ? 0.5f : 1);
            }
            else
            {
                OnNPCTime--;
            }
            if (init)
            {
                if (Projectile.ai[0] == 2)
                {
                    Util.PlaySound("sf_use", 0.6f, Projectile.Center, volume: 0.8f);
                    Util.PlaySound("CastTriangles", 1, Projectile.Center);

                }
                if (Projectile.ai[0] < 2)
                {
                    Util.PlaySound("sf_use", 1 + Projectile.ai[0] * 0.12f, Projectile.Center, volume: 0.6f);
                }
                if (Projectile.ai[0] == 3)
                {
                    Util.PlaySound("sf_use", 0.75f, Projectile.Center, volume: 1);
                }
                init = false;
            }
            
            Projectile.timeLeft = 3;
            if (Projectile.ai[0] == 2)
            {
                if (shoot)
                {
                    shoot = false;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.normalize() * 1100, Projectile.velocity.normalize() * 10, ModContent.ProjectileType<FractalLaser>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
                    }
                    CalamityEntropy.FlashEffectStrength = 0.3f;
                    Util.PlaySound("sf_shoot", 1, Projectile.Center);
                }
                float l = (float)(Math.Cos(progress * MathHelper.Pi - MathHelper.PiOver2));
                Projectile.rotation = Projectile.velocity.ToRotation();
                scale = 1f + l * 2f;
                alpha = l;
                Projectile.Center = Projectile.getOwner().MountedCenter + Projectile.velocity.normalize() * (-34 + l * 34);

            }
            else
            {
                if (Projectile.ai[0] == 3)
                {
                    Projectile.Resize(64, 64);
                    Projectile.Center = Projectile.getOwner().MountedCenter + Projectile.velocity.normalize() * (Util.Parabola(progress, length));
                    if(OnNPC != null && OnNPCTime > 0)
                    {
                        Projectile.Center = OnNPC.Center;
                        length = Util.getDistance(Projectile.Center, OnNPC.Center);
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy((OnNPC.Center - Projectile.Center).ToRotation());
                        if (!OnNPC.active)
                        {
                            OnNPCTime = 0;
                        }
                        counter = MaxUpdateTimes / 2f;
                    }
                    Projectile.rotation = (Projectile.Center - owner.Center).ToRotation();
                    alpha = 1;
                    scale = 2;
                }
                else
                {
                    float RotF = 4f;
                    alpha = 1;
                    scale = 1.8f;
                    Projectile.rotation = Projectile.velocity.ToRotation() + (RotF * -0.5f + RotF * Util.GetRepeatedCosFromZeroToOne(progress, 3)) * Projectile.ai[0] * (Projectile.velocity.X > 0 ? -1 : 1);
                    Projectile.Center = Projectile.getOwner().MountedCenter;
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
            }
            odr.Add(Projectile.rotation);
            if (odr.Count > 30)
            {
                odr.RemoveAt(0);
            }

            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;

            if (counter > MaxUpdateTimes)
            {
                Projectile.Kill();
            }
        }
        public NPC OnNPC = null;
        public int OnNPCTime = 340;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public bool playHitSound = true;
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 3)
            {
                Projectile.localNPCHitCooldown = 4 * 10;
            }
            return base.CanHitNPC(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EParticle.NewParticle(new ShineParticle(), target.Center, Vector2.Zero, new Color(225, 200, 255), 0.6f, 1, true, BlendState.Additive, 0, 12);

            if (Projectile.ai[0] == 3 && OnNPCTime > 0 && OnNPC == null)
            {
                OnNPC = target;
            }
            if (Projectile.ai[0] == 3)
            {
                scale = 2;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 ver = Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(-6, 6);
                    BasePRT particle = new PRT_Light(target.Center, ver
                        , Main.rand.NextFloat(1.3f, 1.7f), new Color(220, 180, 255), 60, 0.15f);
                    PRTLoader.AddParticle(particle);
                }
            }
            EGlobalNPC.AddVoidTouch(target, 40, 1.4f, 600, 16);
            if (playHitSound)
            {
                playHitSound = false;
                Util.PlaySound("sf_hit", 1, Projectile.Center);
                Util.PlaySound("FractalHit", 1, Projectile.Center);
                
            }
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero
            });
        }
        public bool spawnProj = true;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 3)
            {
                modifiers.SetCrit();
                modifiers.SourceDamage *= 0.5f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Projectile.getOwner();
            float MaxUpdateTimes = owner.itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            Texture2D tex = Projectile.GetTexture();
            Texture2D phantom = this.getTextureGlow();
            float texAlpha = 1;
            float phantomAlpha = 0;
            int dir = (int)(Projectile.ai[0] <= 1 ? Projectile.ai[0] : -1) * (Projectile.velocity.X > 0 ? -1 : 1);
            if (Projectile.ai[0] == 2)
            {
                dir = Math.Sign(Projectile.velocity.X);
            }
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            if(Projectile
                .ai[0] == 3)
            {
                origin = tex.Size() / 2f;
            }
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            float MaxUpdateTime = Projectile.getOwner().itemTimeMax * Projectile.MaxUpdates;
            Main.spriteBatch.UseBlendState(BlendState.Additive);

            Texture2D trail = Util.getExtraTex("MotionTrail2");
            List<Vertex> ve = new List<Vertex>();

            for (int i = 0; i < odr.Count; i++)
            {
                Color b = Color.White;
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(240 * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3((i) / ((float)ProjectileID.Sets.TrailCacheLength[Type] - 1), 1, 1),
                      b));
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(0 * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3((i) / ((float)ProjectileID.Sets.TrailCacheLength[Type] - 1), 0, 1),
                      b));
            }

            if (ve.Count >= 3)
            {
                GraphicsDevice gd = Main.spriteBatch.GraphicsDevice;
                Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/FinalFrac", AssetRequestMode.ImmediateLoad).Value;

                Main.spriteBatch.EnterShaderRegion(BlendState.NonPremultiplied, shader);
                shader.Parameters["color2"].SetValue(new Color(220, 190, 255).ToVector4());
                shader.Parameters["color1"].SetValue(new Color(140, 140, 255).ToVector4());
                shader.CurrentTechnique.Passes["EffectPass"].Apply();
                gd.Textures[0] = trail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }


            Main.spriteBatch.ExitShaderRegion();

            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha * texAlpha, rot, origin, Projectile.scale * scale * 1.1f, effect);
            Main.EntitySpriteDraw(phantom, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.White * alpha * phantomAlpha, rot, origin, Projectile.scale * scale * 1.1f, effect);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] == 3)
            {
                return null;
            }
            return Utilities.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (120) * Projectile.scale * scale, targetHitbox, 64);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (120) * Projectile.scale * scale, 84, DelegateMethods.CutTiles);
        }

    }

}