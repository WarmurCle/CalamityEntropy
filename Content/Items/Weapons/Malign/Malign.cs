using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Malign
{
    public class Malign : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.damage = 58;
            Item.noMelee = true;
            Item.useAnimation = Item.useTime = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<MalignHeld>();
            Item.shootSpeed = 16f;
            Item.mana = 3;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
        }
        public override bool MagicPrefix()
        {
            return true;
        }
        public override void HoldItem(Player player)
        {
            player.CheckAndSpawnHeldProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalSerpent)
                .AddIngredient(ItemID.Ectoplasm, 6)
                .AddIngredient<AshesofCalamity>(4)
                .Register();
        }
    }
    public class MalignHeld : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
        }
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Malign/Malign";
        public Texture2D tCircle1 => this.getTextureAlt("Circle1");
        public Texture2D tCircle2 => this.getTextureAlt("Circle2");
        public Texture2D tPart1 => this.getTextureAlt("P1");
        public Texture2D tPart2 => this.getTextureAlt("P2");
        public float ActiveProgress = 0;
        public bool MousePressed = false;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(MousePressed);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            MousePressed = reader.ReadBoolean();
        }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            Projectile.ai[1]--;
            if(player.HeldItem.ModItem is Malign)
            {
                Projectile.timeLeft = 2;
                Projectile.StickToPlayer();
                player.SetHandRot(Projectile.rotation);
                if(Main.myPlayer == Projectile.owner)
                {
                    if((!player.mouseInterface && Main.mouseLeft) != MousePressed)
                    {
                        CEUtils.SyncProj(Projectile.whoAmI);
                    }
                    MousePressed = !player.mouseInterface && Main.mouseLeft;
                    if(MousePressed && ActiveProgress > 0.8f)
                    {
                        player.channel = true;
                        if(player.manaRegenDelay < 16 && player.CheckMana(player.HeldItem.mana, false))
                            player.manaRegenDelay = 16;
                        if (Projectile.ai[1] <= 0)
                        {
                            Projectile.ai[1] = player.HeldItem.useTime;
                            if(player.CheckMana(player.HeldItem.mana, true))
                                Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Projectile.rotation.ToRotationVector2() * 90, Projectile.velocity.RotatedByRandom(0.6f) * 2, ModContent.ProjectileType<MalignBullet>(), player.GetWeaponDamage(player.HeldItem), player.GetWeaponKnockback(player.HeldItem), player.whoAmI);
                        }
                    }
                }
                if(MousePressed)
                {
                    if(ActiveProgress < 1)
                    {
                        ActiveProgress = float.Lerp(ActiveProgress, 1, 0.1f);
                    }

                }
                else
                {
                    if(ActiveProgress > 0)
                    {
                        ActiveProgress = float.Lerp(ActiveProgress, 0, 0.1f);
                    }
                }
                if(MousePressed || ActiveProgress > 0.3)
                {
                    player.itemTime = player.itemAnimation = 3;
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            
            Vector2 top = Projectile.Center + Projectile.rotation.ToRotationVector2() * (56 + 10 * ActiveProgress) * Projectile.scale;
            Main.EntitySpriteDraw(tPart1, top - Main.screenPosition + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver4) * (ActiveProgress * 18 - 12), null, Color.White * 0.5f * ActiveProgress, Projectile.rotation + MathHelper.PiOver4 + 0.3f * ActiveProgress, new Vector2(0, tPart1.Height / 2), Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(tPart2, top - Main.screenPosition + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver4) * (ActiveProgress * 18 - 12), null, Color.White * 0.5f * ActiveProgress, Projectile.rotation + MathHelper.PiOver4 + -0.3f * ActiveProgress, new Vector2(tPart2.Width / 2, tPart2.Height), Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, new Vector2(6, tex.Height - 6), Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(tCircle1, Projectile.Center + Projectile.rotation.ToRotationVector2() * 94 * Projectile.scale - Main.screenPosition, null, Color.White * (ActiveProgress * ActiveProgress * ActiveProgress * 0.8f), Main.GlobalTimeWrappedHourly * 16, tCircle1.Size() / 2f, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(tCircle2, Projectile.Center + Projectile.rotation.ToRotationVector2() * 94 * Projectile.scale - Main.screenPosition, null, Color.White * (ActiveProgress * ActiveProgress * ActiveProgress * 0.8f), Main.GlobalTimeWrappedHourly * -16, tCircle2.Size() / 2f, Projectile.scale, SpriteEffects.None);
            CEUtils.DrawGlow(Projectile.Center + Projectile.rotation.ToRotationVector2() * 94 * Projectile.scale, new Color(255, 200, 255) * 0.66f * (ActiveProgress * ActiveProgress * ActiveProgress), 1.2f);
            return false;
        }
    }
    public class MalignLaser : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.light = 2;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.localAI[2]++ == 0)
            {
                for(float i = 0; i <= 1; i += 0.005f)
                {
                    EParticle.spawnNew(new HeavenfallStar2() { drawScale = new Vector2(0.2f, 1f) }, Projectile.Center + Projectile.velocity * i, Vector2.Zero, new Color(255, 40, 255), CEUtils.CustomLerp2(1 - i) + 0.1f, 1, true, BlendState.Additive, Projectile.velocity.ToRotation(), 16);
                }
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(255, 200, 255), 0.6f, 1, true, BlendState.Additive, 0, 16);
                for(int i = 0; i < 3; i++)
                {
                    Vector2 v = Projectile.velocity.RotateRandom(0.4f);
                    GeneralParticleHandler.SpawnParticle(new LineParticle(Projectile.Center - v * 0.1f, v * -0.01f, false, 16, 2, new Color(255, 190, 255)));
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity, targetHitbox, 32);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
        }
    }
    public class MalignBullet : ModProjectile
    {
        public TrailParticle trail;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, true, 1);
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 24;
            Projectile.light = 1;
        }
        public override void AI()
        {
            if (Projectile.localAI[2]++ == 0)
            {
                CEUtils.PlaySound("malignShoot", Main.rand.NextFloat(1f, 1.6f), Projectile.Center, volume:0.84f);
            }
            if(Main.myPlayer == Projectile.owner)
            {
                if(Projectile.timeLeft == 23 || Projectile.timeLeft == 13)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<MalignLightning>(), Projectile.damage / 2, Projectile.knockBack / 4, Projectile.owner);
                }
            }
            if(trail == null)
            {
                trail = new TrailParticle() { maxLength = 20};
                EParticle.spawnNew(trail, Projectile.Center, Vector2.Zero, new Color(255, 190, 255), 1.6f, 1, true, BlendState.Additive);
            }
            EParticle.spawnNew(new Content.Particles.ELineParticle(2f, 0.8f, 0.8f), Projectile.Center + CEUtils.randomPointInCircle(8), Projectile.velocity.RotatedByRandom(0.2f), new Color(255, 190, 255) * 0.8f, 2, 1, true, BlendState.Additive, Projectile.velocity.ToRotation(), 6);
            
            trail.AddPoint(Projectile.Center + Projectile.velocity);
            trail.TimeLeftMax = trail.Lifetime = 13;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if(Projectile.timeLeft < 22)
            {
                NPC target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 800);
                if (target != null)
                {
                    Projectile.velocity = CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 0.02f, true).ToRotationVector2() * Projectile.velocity.Length();
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D glow = this.getTextureGlow();
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, new Color(255, 95, 255), Projectile.velocity.ToRotation(), glow.Size() / 2f, Projectile.scale * 0.14f,SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(255, 95, 255), Projectile.velocity.ToRotation(), tex.Size() / 2f, Projectile.scale * 0.1f, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.normalize() * 500, ModContent.ProjectileType<MalignLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, new Color(255, 190, 255), 1, 1, true, BlendState.Additive, 0, 12);
        }
    }

}
