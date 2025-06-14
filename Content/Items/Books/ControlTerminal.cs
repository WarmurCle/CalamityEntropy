using CalamityEntropy.Common;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class ControlTerminal : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 190;
            Item.useAnimation = Item.useTime = 60;
            Item.crit = 10;
            Item.mana = 36;
            Item.shootSpeed = 40;
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark7").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<ControlTerminalHeld>();
        public override int SlotCount => 6;

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<VoidOde>()
                .AddIngredient<EventHorizon>()
                .AddIngredient<MiracleMatter>()
                .AddTile<DraedonsForge>()
                .Register();
        }
    }

    public class ControlTerminalHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/ControlTerminal/ControlTerminalOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/ControlTerminal/ControlTerminalPage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/ControlTerminal/ControlTerminalUI";

        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.Homing += 1f;
            m.HomingRange += 1f;
            return m;
        }
        public override float randomShootRotMax => 0;
        public override int frameChange => 2;
        public override int baseProjectileType => ModContent.ProjectileType<WhirlExobeam>();
        public override EBookProjectileEffect getEffect()
        {
            return new ControlTerminalBookBaseEffect();
        }
        public override bool Shoot()
        {
            int type = ModContent.ProjectileType<ExoWhirl>();
            ShootSingleProjectile(type, Projectile.Center, Projectile.velocity);
            return true;
        }
        public override bool canApplyShootCDModifer => false;
        public int getShootCd()
        {
            int _shotCooldown = bookItem.useTime;

            EBookStatModifer m = getBaseModifer();
            for (int i = 0; i < Projectile.getOwner().GetMyMaxActiveBookMarks(bookItem); i++)
            {
                Item it = Projectile.getOwner().Entropy().EBookStackItems[i];
                if (BookMarkLoader.IsABookMark(it))
                {
                    BookMarkLoader.ModifyStat(it, m);
                    BookMarkLoader.modifyShootCooldown(it, ref _shotCooldown);
                }
            }
            return (int)(0.6f * (float)_shotCooldown / m.attackSpeed);
        }
        public void shootBaseProj(Vector2 pos, Vector2 vel)
        {
            int type = getShootProjectileType();

            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.getOwner().Entropy().EBookStackItems.Count); i++)
            {
                var bm = Projectile.owner.ToPlayer().Entropy().EBookStackItems[i];
                if (BookMarkLoader.IsABookMark(bm))
                {
                    int pn = BookMarkLoader.ModifyProjectile(bm, type);
                    if (pn >= 0)
                    {
                        type = pn;
                    }
                }
            }
            ShootSingleProjectile(type, pos, vel, shotSpeedMul: 0.6f);
        }
    }
    public class ControlTerminalBookBaseEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            base.OnProjectileSpawn(projectile, ownerClient);
            projectile.tileCollide = false;
            if (projectile.ModProjectile is EBookBaseProjectile e)
            {
                e.gravity = 0;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 60, false);
        }
    }

    public class ExoWhirl : EBookBaseProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[base.Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[base.Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 80;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = 3;
            Projectile.light = 1;
        }
        public override bool PreAI()
        {
            Projectile.penetrate = -1;
            return base.PreAI();
        }
        public float r = 0;
        public int shootCd = 9;
        public bool ri = true;
        public override void AI()
        {
            if (ri)
            {
                ri = false;
                r = Projectile.velocity.ToRotation();
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                    Projectile.oldRot[i] = Projectile.rotation;
                }
            }
            base.AI();
            shootCd--;
            if (shootCd <= 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    var book = (ControlTerminalHeld)(ShooterModProjectile);
                    NPC target = null;

                    target = Projectile.FindTargetWithinRange(homingRange * 3);

                    if (target != null)
                    {
                        book.shootBaseProj(Projectile.Center, target.Center - Projectile.Center);
                    }
                    shootCd = book.getShootCd();
                }

            }
            if (++Projectile.localAI[0] > 18 && hitCount == 0)
            {
                hitCount++;
                Projectile.ai[0] = 120;
            }
            if (Projectile.ai[0] > 0)
            {
                Projectile.velocity *= 0.8f;
            }
            Projectile.ai[0]--;
            if (hitCount == 0)
            {
                r = Projectile.velocity.ToRotation();
            }
            else
            {
                if (Projectile.ai[0] < 10)
                {
                    if (Projectile.ai[2] < 16)
                    {
                        Projectile.ai[2] += 0.15f;
                    }
                    r = Projectile.velocity.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length() + 1f, 0).RotatedBy(CEUtils.rotatedToAngle(r, (Projectile.getOwner().Center - Projectile.Center).ToRotation(), 0.5f * Projectile.ai[2], false));
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(CEUtils.rotatedToAngle(r, (Projectile.getOwner().Center - Projectile.Center).ToRotation(), 1f * Projectile.ai[2], true));
                    Projectile.velocity *= 0.97f;
                    if (CEUtils.getDistance(Projectile.Center, Projectile.getOwner().Center) < Projectile.velocity.Length() * 1.2f)
                    {
                        Projectile.Kill();
                    }
                }
            }
            Projectile.rotation = Main.GameUpdateCount * 0.4f;
            if (Projectile.velocity.Length() < 2)
            {
                Projectile.velocity = r.ToRotationVector2() * 2;
            }
        }
        public float TrailWidth(float completionRatio)
        {
            return Utils.GetLerpValue(1f, 0.4f, completionRatio, clamped: true) * (float)Math.Sin(Math.Acos(1f - Utils.GetLerpValue(0f, 0.15f, completionRatio, clamped: true))) * Utils.GetLerpValue(0f, 0.1f, (float)base.Projectile.timeLeft / 600f, clamped: true) * 6;
        }
        public static ReLogic.Content.Asset<Texture2D> TrailTex = null;
        public Color TrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, new Color(0, 0, 255), completionRatio);
        }

        public float MiniTrailWidth(float completionRatio)
        {
            return TrailWidth(completionRatio) * 0.8f;
        }

        public Color MiniTrailColor(float completionRatio)
        {
            return Color.White;
        }

        public override void ApplyHoming()
        {
            if (hitCount == 0 && ++Projectile.ai[1] > 10)
            {
                base.ApplyHoming();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (hitCount == 1)
            {
                Projectile.ai[0] = 120;
            }
            if (Projectile.ai[0] < 6)
            {
                Projectile.ai[0] = 6;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < 4; i++)
            {
                float ra = i * MathHelper.PiOver2;
                Main.spriteBatch.EnterShaderRegion();
                if (TrailTex == null)
                {
                    TrailTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail");
                }
                Color color1 = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + (float)base.Projectile.whoAmI * 0.12f) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
                Color color2 = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + (float)base.Projectile.whoAmI * 0.12f + 0.2f) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);

                GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(TrailTex);
                GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
                GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(color1);
                GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(color2);
                GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
                GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
                Vector2[] tpos = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
                for (int k = 0; k < ProjectileID.Sets.TrailCacheLength[Type]; k++)
                {
                    tpos[k] = Projectile.oldPos[k] + (Projectile.oldRot[k] + ra).ToRotationVector2() * 37 * Projectile.scale;
                }
                PrimitiveRenderer.RenderTrail(tpos, new PrimitiveSettings(TrailWidth, TrailColor, (float _) => base.Projectile.Size * 0.5f, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);
                GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Color.White);
                GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Color.White);
                PrimitiveRenderer.RenderTrail(tpos, new PrimitiveSettings(MiniTrailWidth, MiniTrailColor, (float _) => base.Projectile.Size * 0.5f, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);
                Main.spriteBatch.ExitShaderRegion();
            }
            Main.EntitySpriteDraw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.GetTexture().Size() / 2f, Projectile.scale, SpriteEffects.None); ;
            return false;
        }
    }
    public class WhirlExobeam : EBookBaseProjectile
    {
        public int TargetIndex = -1;

        public static float MaxWidth = 30f;

        public static Asset<Texture2D> BloomTex;

        public static Asset<Texture2D> SlashTex;

        public static Asset<Texture2D> TrailTex;


        public ref float Time => ref base.Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[base.Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[base.Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            base.Projectile.width = 20;
            base.Projectile.height = 20;
            base.Projectile.friendly = true;
            base.Projectile.DamageType = DamageClass.Magic;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
            base.Projectile.extraUpdates = 1;
            base.Projectile.alpha = 255;
            base.Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
        public override void AI()
        {
            base.AI();
            if (Time >= (float)10)
            {
                if (TargetIndex >= 0)
                {
                    if (!Main.npc[TargetIndex].active || !Main.npc[TargetIndex].CanBeChasedBy())
                    {
                        TargetIndex = -1;
                    }
                    else
                    {
                        Vector2 value = base.Projectile.SafeDirectionTo(Main.npc[TargetIndex].Center) * (base.Projectile.velocity.Length() + 6.5f);
                        base.Projectile.velocity = Vector2.Lerp(base.Projectile.velocity, value, 0.08f);
                    }
                }

                if (TargetIndex == -1)
                {
                    NPC nPC = base.Projectile.Center.ClosestNPCAt(1600f, ignoreTiles: false);
                    if (nPC != null)
                    {
                        TargetIndex = nPC.whoAmI;
                    }
                    else
                    {
                        base.Projectile.velocity *= 0.99f;
                    }
                }
            }

            base.Projectile.rotation = base.Projectile.velocity.ToRotation();
            if (Main.rand.NextBool())
            {
                Color newColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.9f);
                Dust dust = Dust.NewDustPerfect(base.Projectile.Center + Main.rand.NextVector2Circular(20f, 20f) + base.Projectile.velocity, 267, base.Projectile.velocity * -2.6f, 0, newColor);
                dust.scale = 0.3f;
                dust.fadeIn = Main.rand.NextFloat() * 1.2f;
                dust.noGravity = true;
            }

            base.Projectile.scale = Utils.GetLerpValue(0f, 0.1f, (float)base.Projectile.timeLeft / 600f, clamped: true);
            if (base.Projectile.FinalExtraUpdate())
            {
                Time += 1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(in Exoblade.BeamHitSound, target.Center);
            if (Main.myPlayer == base.Projectile.owner)
            {
                int num = Projectile.NewProjectile(base.Projectile.GetSource_FromAI(), target.Center, base.Projectile.velocity * 0.1f, ModContent.ProjectileType<ExobeamSlashCreator>(), base.Projectile.damage, 0f, base.Projectile.owner, target.whoAmI, base.Projectile.velocity.ToRotation());
                if (Main.projectile.IndexInRange(num))
                {
                    Main.projectile[num].timeLeft = 20;
                }
            }

            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color white = Color.White;
            white.A = 0;
            return white * base.Projectile.Opacity;
        }

        public float TrailWidth(float completionRatio)
        {
            return Utils.GetLerpValue(1f, 0.4f, completionRatio, clamped: true) * (float)Math.Sin(Math.Acos(1f - Utils.GetLerpValue(0f, 0.15f, completionRatio, clamped: true))) * Utils.GetLerpValue(0f, 0.1f, (float)base.Projectile.timeLeft / 600f, clamped: true) * MaxWidth;
        }

        public Color TrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, new Color(0, 0, 255), completionRatio);
        }

        public float MiniTrailWidth(float completionRatio)
        {
            return TrailWidth(completionRatio) * 0.8f;
        }

        public Color MiniTrailColor(float completionRatio)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (base.Projectile.timeLeft > 595)
            {
                return false;
            }

            Texture2D value = TextureAssets.Projectile[base.Projectile.type].Value;
            float num = Utils.GetLerpValue(3f, 13f, base.Projectile.velocity.Length(), clamped: true) * 1.2f;
            Vector2 position = base.Projectile.oldPos[2] + base.Projectile.Size / 2f - Main.screenPosition;
            Color white = Color.White;
            white.A = 0;
            Main.EntitySpriteDraw(value, position, null, white, base.Projectile.rotation + MathF.PI / 4f, value.Size() / 2f, num * base.Projectile.scale, SpriteEffects.None);
            if (BloomTex == null)
            {
                BloomTex = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
            }

            Texture2D value2 = BloomTex.Value;
            Color color = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + (float)base.Projectile.whoAmI * 0.12f) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
            Color color2 = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 0.5f + (float)base.Projectile.whoAmI * 0.12f + 0.2f) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
            Vector2 position2 = base.Projectile.oldPos[2] + base.Projectile.Size / 2f - Main.screenPosition;
            white = color * 0.1f;
            white.A = 0;
            Main.EntitySpriteDraw(value2, position2, null, white, 0f, value2.Size() / 2f, 1.3f * base.Projectile.scale, SpriteEffects.None);
            Vector2 position3 = base.Projectile.oldPos[1] + base.Projectile.Size / 2f - Main.screenPosition;
            white = color * 0.5f;
            white.A = 0;
            Main.EntitySpriteDraw(value2, position3, null, white, 0f, value2.Size() / 2f, 0.34f * base.Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.EnterShaderRegion();
            if (TrailTex == null)
            {
                TrailTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail");
            }

            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(TrailTex);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(color);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(color2);
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
            PrimitiveRenderer.RenderTrail(base.Projectile.oldPos, new PrimitiveSettings(TrailWidth, TrailColor, (float _) => base.Projectile.Size * 0.5f, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Color.White);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Color.White);
            PrimitiveRenderer.RenderTrail(base.Projectile.oldPos, new PrimitiveSettings(MiniTrailWidth, MiniTrailColor, (float _) => base.Projectile.Size * 0.5f, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);
            Main.spriteBatch.ExitShaderRegion();
            Vector2 position4 = base.Projectile.oldPos[2] + base.Projectile.Size / 2f - Main.screenPosition;
            white = Color.White * 0.2f;
            white.A = 0;
            Main.EntitySpriteDraw(value2, position4, null, white, 0f, value2.Size() / 2f, 0.78f * base.Projectile.scale, SpriteEffects.None);
            Vector2 position5 = base.Projectile.oldPos[1] + base.Projectile.Size / 2f - Main.screenPosition;
            white = Color.White * 0.5f;
            white.A = 0;
            Main.EntitySpriteDraw(value2, position5, null, white, 0f, value2.Size() / 2f, 0.2f * base.Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}