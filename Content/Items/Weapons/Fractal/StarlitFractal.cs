using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Fractal
{
    public class StarlitFractal : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 138;
            Item.crit = 5;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 48;
            Item.height = 60;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StarlitFractalHeld>();
            Item.shootSpeed = 12f;
        }
        public int atkType = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, atkType == 0 ? -1 : atkType);
            atkType*=-1;
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AbyssFractal>()
                .AddIngredient<EntropicClaymore>()
                .AddIngredient<AstralBlade>()
                .AddIngredient<AstralBar>(6)
                .AddIngredient<StarblightSoot>(4)
                .AddTile(TileID.LunarCraftingStation).Register();
        }
    }
    public class StarlitFractalHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Fractal/StarlitFractal";
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;

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
            Projectile.extraUpdates = 3;
        }
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;
        public float spawnProjCounter = 0;
        public override void AI()
        {
            Player owner = Projectile.getOwner();
            float MaxUpdateTimes = owner.itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            counter++;
            if(Main.myPlayer == Projectile.owner)
            {
                if(spawnProj && progress > 0.4f)
                {
                    int dir = (int)(Projectile.ai[0]) * (Projectile.velocity.X > 0 ? -1 : 1);
                    spawnProj = false;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 49, Projectile.velocity.RotatedBy(dir * MathHelper.PiOver2).normalize() * 3 + Util.randomPointInCircle(2), ModContent.ProjectileType<FractalStarBlade>(), Projectile.damage, Projectile.knockBack * 4, Projectile.owner, Projectile.rotation, dir);
                }
            }
            if (init)
            {
                if (Projectile.ai[0] == 2)
                {
                    Util.PlaySound("powerwhip", 1, Projectile.Center, volume: 0.6f);
                }
                if (Projectile.ai[0] < 2)
                {
                    Util.PlaySound("sf_use", 1 + Projectile.ai[0] * 0.12f, Projectile.Center, volume: 0.6f);
                }
                init = false;
            }
            odr.Add(Projectile.rotation);
            Projectile.timeLeft = 3;
            float RotF = 4f;
            alpha = 1;
            scale = 1.6f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (RotF * -0.5f + RotF * Util.GetRepeatedCosFromZeroToOne(progress, 3)) * Projectile.ai[0] * (Projectile.velocity.X > 0 ? -1 : 1);
            Projectile.Center = Projectile.getOwner().MountedCenter;


            if (odr.Count > 60)
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
            if (counter > MaxUpdateTimes)
            {
                owner.itemTime = 1;
                owner.itemAnimation = 1;
                Projectile.Kill();
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public bool playHitSound = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 460);
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = target.Center + new Vector2(0, -900) + Util.randomPointInCircle(400);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, (target.Center - pos).normalize() * 42, ModContent.ProjectileType<AstralStarMagic>(), Projectile.damage / 4, Projectile.owner).ToProj().DamageType = Projectile.DamageType;
            }
            if (playHitSound)
            {
                playHitSound = false;
                Util.PlaySound(Projectile.ai[0] == 2 ? "sf_hit1" : "sf_hit", 1, Projectile.Center, volume: 0.6f);
            }
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TrueExcalibur, new ParticleOrchestraSettings
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero
            });
        }
        public bool spawnProj = true;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            int dir = (int)(Projectile.ai[0]) * (Projectile.velocity.X > 0 ? -1 : 1);
            if (Projectile.ai[0] == 2)
            {
                dir = Math.Sign(Projectile.velocity.X);
            }
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            float MaxUpdateTime = Projectile.getOwner().itemTimeMax * Projectile.MaxUpdates;

            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * scale * 1.1f, effect);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Texture2D bs = Util.getExtraTex("SemiCircularSmear");

            Main.spriteBatch.Draw(bs, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, new Color(100, 50, 200) * (float)(Math.Cos(Util.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, 3) * MathHelper.Pi - MathHelper.PiOver2)), Projectile.rotation + MathHelper.ToRadians(32) * -dir, bs.Size() / 2f, Projectile.scale * 1.74f * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bs, Projectile.Center + Projectile.getOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.Lerp(Color.White, Color.Blue, counter / MaxUpdateTime) * (float)(Math.Cos(Util.GetRepeatedCosFromZeroToOne(counter / MaxUpdateTime, 3) * MathHelper.Pi - MathHelper.PiOver2)), Projectile.rotation + MathHelper.ToRadians(32) * -dir, bs.Size() / 2f, Projectile.scale * 1.56f * scale, SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (126) * Projectile.scale * scale, targetHitbox, 64);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (132) * Projectile.scale * scale, 84, DelegateMethods.CutTiles);
        }
    }

}