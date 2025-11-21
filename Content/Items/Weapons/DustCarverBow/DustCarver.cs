using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityOverhaul.OtherMods.ImproveGame;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Content.Items.Weapons.DustCarverBow
{
    public class DustCarver : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 150;
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DustCarverHeld>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] < 1 && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, (Main.MouseWorld - player.Center).normalize() * Item.shootSpeed, Item.shoot, 0, 0, player.whoAmI);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
    public class DustCarverHeld : ModProjectile
    {
        public float Charging = 0;
        public int ShootDelay = 0;
        public int ShootEffect = 0;
        public bool active = false;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.width = Projectile.height = 12;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(active);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            active = reader.ReadBoolean();
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            Projectile.StickToPlayer();
            player.SetHandRot(Projectile.rotation);

            if(!(player.HeldItem.ModItem is DustCarver))
            {
                Projectile.Kill();
                return;
            }
            Vector2 particlePos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 32;
            if (sParticle != null)
            {
                sParticle.Position = particlePos;
                sParticle.Rotation = Projectile.rotation;
            }
            if (sParticle2 != null)
            {
                sParticle2.Position = particlePos;
                sParticle2.Rotation = Projectile.rotation;
            }
            if (Main.myPlayer == Projectile.owner) {
                bool fl = active;
                active = Main.mouseLeft && !player.mouseInterface;
                if(active != fl)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.timeLeft = 5;
            if(ShootEffect > 0)
                ShootEffect--;

            float chargeAdd = 1f / player.HeldItem.useTime;
            if (Charging > 0)
            {
                player.itemTime = player.itemAnimation = 3;
                Charging += chargeAdd;
            }
            else
            {
                if (active)
                {
                    Charging += chargeAdd;
                }
            }
            if(Charging >= 1)
            {
                Charging = 0;
                ShootDelay = player.HeldItem.useTime / 4;
                sParticle = new HeavenfallStar2() { Inverse = true};
                EParticle.spawnNew(sParticle, particlePos, Vector2.Zero, new Color(255, 40, 40), 4f, 1, true, BlendState.Additive, Projectile.rotation, 18);
                sParticle2 = new HeavenfallStar2() { Inverse = true };
                EParticle.spawnNew(sParticle2, particlePos, Vector2.Zero, new Color(255, 160, 160), 3.5f, 1, true, BlendState.Additive, Projectile.rotation, 18);
                CEUtils.PlaySound("DustCarverShoot", Main.rand.NextFloat(1.6f, 2f), Projectile.Center, 6, 0.6f);
                CEUtils.PlaySound("CarverShoot2", Main.rand.NextFloat(1.4f, 1.8f), Projectile.Center, 6, 0.6f);
                
                if (Main.myPlayer == Projectile.owner)
                {
                    player.PickAmmo(player.HeldItem, out int projID, out float shootSpeed, out int damage, out float kb, out var ammoID, false);
                    int p = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center, Projectile.rotation.ToRotationVector2() * shootSpeed, projID, damage, kb, Projectile.owner);
                    Projectile projectile = p.ToProj();
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = -1;
                    DustCarverArrorGProj mproj = projectile.GetGlobalProjectile<DustCarverArrorGProj>();
                    mproj.active = true;
                    CEUtils.SyncProj(p);
                    projectile.MaxUpdates *= 8;
                    projectile.penetrate += 2;
                }
            }
        }
        public EParticle sParticle;
        public EParticle sParticle2;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D eye = this.getTextureGlow();
            SpriteEffects effect = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 origin = Projectile.velocity.X > 0 ? new Vector2(tex.Width / 2, 83) : new Vector2(tex.Width / 2, tex.Height - 83);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, effect, 0);
            Main.spriteBatch.SetBlendState(BlendState.NonPremultiplied);
            Color eyeColor = Color.White;
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition, null, eyeColor, Projectile.rotation, origin, Projectile.scale, effect, 0);

            float stringOffset = 0;
            if(Charging > 0.5f)
            {
                stringOffset -= CEUtils.Parabola((Charging - 0.5f), 1) * 70 * Projectile.scale;
            }
            Vector2 p1 = Projectile.Center - Projectile.rotation.ToRotationVector2() * 22 + new Vector2(0, Projectile.velocity.X > 0 ? 62 : 82).RotatedBy(Projectile.rotation);
            Vector2 p2 = Projectile.Center - Projectile.rotation.ToRotationVector2() * 22 + new Vector2(0, Projectile.velocity.X <= 0 ? -62 : -82).RotatedBy(Projectile.rotation);
            Vector2 pc = Projectile.Center - Projectile.rotation.ToRotationVector2() * (22 - stringOffset);
            CEUtils.drawLine(p1, pc, Color.Crimson, 2, 2);
            CEUtils.drawLine(p2, pc, Color.Crimson, 2, 2);

            if (Charging > 0) {
                Texture2D star = CEUtils.getExtraTex("StarTexture_White");
                Vector2 sScale = Charging < 0.5f ? new Vector2(1, 0.8f) : new Vector2(1.8f, 0.7f);
                float sOffset = Charging < 0.5f ? 2 * (0.5f - Charging) : 0;
                float sAlpha = Charging / 0.5f * 0.4f + 0.6f;
                if (sAlpha > 1)
                    sAlpha = 1;
                Color c1 = new Color(60, 26, 26).MultiplyRGBA(new Color(1, 1, 1, sAlpha));
                Color c2 = new Color(255, 230, 230).MultiplyRGBA(new Color(1, 1, 1, sAlpha));

                float w = 64 * Projectile.scale;
                Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * (14 - stringOffset);
                Main.spriteBatch.Draw(star, pos + new Vector2(0, sOffset * w).RotatedBy(Projectile.rotation) - Main.screenPosition, null, c1, Projectile.rotation, star.Size() / 2f, sScale * 0.3f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(star, pos + new Vector2(0, sOffset * -w).RotatedBy(Projectile.rotation) - Main.screenPosition, null, c1, Projectile.rotation, star.Size() / 2f, sScale * 0.3f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(star, pos + new Vector2(0, sOffset * w).RotatedBy(Projectile.rotation) - Main.screenPosition, null, c2, Projectile.rotation, star.Size() / 2f, sScale * 0.26f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(star, pos + new Vector2(0, sOffset * -w).RotatedBy(Projectile.rotation) - Main.screenPosition, null, c2, Projectile.rotation, star.Size() / 2f, sScale * 0.26f, SpriteEffects.None, 0);
            }
            
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }

    public class DustCarverArrorGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool active = false;
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter writer)
        {
            writer.Write(active);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader reader)
        {
            active = reader.ReadBoolean();
        }
        public List<Vector2> oldPos = new();
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Crimson, Color.DarkRed, MathHelper.Clamp(completionRatio * 0.8f, 0f, 1f)) * projectile.Opacity;
        }
        public Projectile projectile;
        public float WidthFunction(float completionRatio)
        {
            float num = 22;
            float num2 = ((!(completionRatio < 0.1f)) ? MathHelper.Lerp(num, 0f, Utils.GetLerpValue(0.1f, 1f, completionRatio, clamped: true)) : ((float)Math.Sin(completionRatio / 0.1f * (MathF.PI / 2f)) * num + 0.1f));
            return num2 * projectile.Opacity * projectile.scale
            ;
        }
        public override bool PreAI(Projectile projectile)
        {
            if(active)
            {
                oldPos.Insert(0, projectile.Center + projectile.velocity.normalize() * 24);
                if (oldPos.Count > 24)
                    oldPos.RemoveAt(oldPos.Count - 1);
                return false;
            }
            return true;
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            this.projectile = projectile;
            if (!active)
            {
                return true;
            }
            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            PrimitiveRenderer.RenderTrail(oldPos, new PrimitiveSettings(WidthFunction, ColorFunction, (float _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            Main.spriteBatch.ExitShaderRegion();
            Texture2D glow2 = CEUtils.getExtraTex("SpearArrowGlow2");
            Texture2D arrow = CEUtils.getExtraTex("DustArrow");
            float rot = projectile.velocity.ToRotation();
            Main.spriteBatch.Draw(arrow, projectile.Center - Main.screenPosition, null, Color.White * 0.6f, rot, arrow.Size() / 2f, projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(glow2, projectile.Center - Main.screenPosition, null, new Color(230, 4, 4), rot, glow2.Size() / 2f, new Vector2(2, 0.8f) * projectile.scale * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(glow2, projectile.Center - Main.screenPosition, null, new Color(255, 180, 180), rot, glow2.Size() / 2f, new Vector2(2, 0.8f) * projectile.scale * 0.3f, SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(active)
            {
                CEUtils.PlaySound("CarverHit", Main.rand.NextFloat(1.4f, 1.6f), target.Center, 6, 0.2f);
                CEUtils.PlaySound("GrassSwordHit0", Main.rand.NextFloat(1.4f, 1.8f), target.Center, 6, 0.25f);
                CEUtils.PlaySound("bne_hit", Main.rand.NextFloat(1.2f, 1.4f), target.Center, 4, 0.8f);
                for (int i = 0; i < 32; i++)
                {
                    float p = Main.rand.NextFloat();
                    Color clr = new Color(255, 24, 24);
                    var vel = projectile.velocity.normalize().RotatedBy(0.25f * p * (Main.rand.NextBool() ? 1 : -1)) * 64 * (1.2f - p) * Main.rand.NextFloat(0.2f, 1);
                    EParticle.NewParticle(new HeavenfallStar2() { drawScale = new Vector2(0.4f, 1)}, target.Center, vel, clr, (1.2f - p) * 1.2f, 1, true, BlendState.Additive, vel.ToRotation(), 24);
                    EParticle.NewParticle(new HeavenfallStar2() { drawScale = new Vector2(0.4f, 1) }, target.Center, vel, new Color(255, 200, 200), (1.2f - p) * 0.7f * 1.2f, 1, true, BlendState.Additive, vel.ToRotation(), 24);
                }
                EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 50, 50), 1, 1, true, BlendState.Additive, 0, 6);
                EParticle.spawnNew(new ShineParticle(), target.Center, Vector2.Zero, new Color(255, 255, 255), 0.6f, 1, true, BlendState.Additive, 0, 6);

            }
        }
    }
}