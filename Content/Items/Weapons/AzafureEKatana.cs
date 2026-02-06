using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureEKatana : ModItem, IAzafureEnhancable
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 15));
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.autoReuse = true;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.damage = 120;
            Item.knockBack = 4;
            Item.crit = 15;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<AzafureEKatanaSlash>();
            Item.shootSpeed = 12;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }
        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellIndustrialComponents>(4)
                .AddIngredient(ItemID.TitaniumBar, 8)
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class AzafureEKatanaSlash : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(ModContent.GetInstance<TrueMeleeDamageClass>(), false, -1);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public float Scale => Projectile.scale * (Projectile.GetOwner().AzafureEnhance() ? 1.5f : 1);
        public List<int> OldFrames = new();
        public override void AI()
        {
            var player = Projectile.GetOwner();
            player.Calamity().mouseWorldListener = true;
            if (Projectile.ai[0]++ == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[1] == 0)
            {

                if (Projectile.frameCounter == 0 || Projectile.frameCounter == 3 || Projectile.frameCounter == 6)
                {
                    Projectile.velocity = (player.Calamity().mouseWorld - Projectile.Center).normalize() * 8;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Projectile.ResetLocalNPCHitImmunity();
                    CEUtils.PlaySound("vbapear", 3.8f, Projectile.Center, 0, 0.2f);
                    CEUtils.PlaySound("vbuse", Projectile.frameCounter / 6f + 2, Projectile.Center, 6, 0.35f);
                    shake = true;
                }
            }

            if (++Projectile.ai[1] >= 4 / (player.GetTotalAttackSpeed(Projectile.DamageType)))
            {

                Projectile.ai[1] = 0;

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 9)
                {
                    Projectile.frameCounter = 0;
                }
            }
            OldFrames.Add(Projectile.frameCounter);
            if (OldFrames.Count > 7)
                OldFrames.RemoveAt(0);
            if (player.channel && !player.dead)
            {
                Projectile.timeLeft = 2;
                player.itemTime = player.itemAnimation = 3;
                player.SetHandRot(Projectile.rotation);
            }
            Projectile.Center = player.GetDrawCenter();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame;
            SpriteEffects effect = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new Vector2(Projectile.velocity.X > 0 ? 84 : tex.Width - 84, 254 / 2);
            float rot = Projectile.rotation + (Projectile.velocity.X > 0 ? 0 : MathHelper.Pi);
            for (int i = 0; i < OldFrames.Count; i++)
            {
                frame = new Rectangle(0, OldFrames[i] * 256, tex.Width, 254);
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor * 0.16f * ((i + 1) / (float)OldFrames.Count), rot, origin, Scale, effect);
            }
            frame = new Rectangle(0, Projectile.frameCounter * 256, tex.Width, 254);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, lightColor, rot, origin, Scale, effect);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 160 * Scale, targetHitbox, 256);
        }
        public bool shake = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<MechanicalTrauma>(160);
            Player Owner = Projectile.GetOwner();
            if (target.Organic())
                SoundEngine.PlaySound(Murasama.OrganicHit with { Pitch = -Main.rand.NextFloat(0.2f, 0.36f), Volume = 0.26f }, Projectile.Center);
            else
                SoundEngine.PlaySound(Murasama.InorganicHit with { Pitch = -Main.rand.NextFloat(0.2f, 0.36f), Volume = 0.26f }, Projectile.Center);

            CEUtils.PlaySound("ExoHit1", Main.rand.NextFloat(1.7f, 2), target.Center, volume: 0.3f);
            for (int i = 0; i < 52; i++)
            {
                float p = Main.rand.NextFloat();
                EParticle.spawnNew(new EGlowOrb() { CenterScale = 0.55f }, CEUtils.randomPoint(target.Hitbox), CEUtils.randomPointInCircle(4) + Projectile.velocity.RotatedBy(0.5f * p * (Main.rand.NextBool() ? 1 : -1)) * 4f * Main.rand.NextFloat(0.2f, 1) * (1.2f - p), Color.Lerp(Color.DarkRed, Color.Firebrick, Main.rand.NextFloat()), 0.12f, 1, true, BlendState.Additive, 0, 18);
            }
            EParticle.NewParticle(new ShineParticle(), target.Center, Vector2.Zero, Color.Red, 0.7f, 1, true, BlendState.Additive, 0, 8);
            EParticle.NewParticle(new ShineParticle(), target.Center, Vector2.Zero, Color.White, 0.56f, 1, true, BlendState.Additive, 0, 8);
            float dustCount = 16;
            for (int i = 0; i <= dustCount; i++)
            {
                int dustID = Main.rand.NextBool(3) ? 182 : Main.rand.NextBool() ? 309 : 296;
                Dust dust2 = Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(target.width * 0.5f, target.height * 0.5f), dustID, Projectile.velocity.RotatedByRandom(0.8f));
                dust2.scale = Main.rand.NextFloat(0.9f, 2.4f);
                dust2.noGravity = true;
            }
            EParticle.spawnNew(new ElecParticle() { PixelShader = true }, target.Center, Vector2.Zero, new Color(255, 160, 160), 1.4f, 1, true, BlendState.Additive, 0, 26);
            EParticle.spawnNew(new ElecParticle() { PixelShader = true }, target.Center, Vector2.Zero, Color.White, 1.2f, 1, true, BlendState.Additive, 0, 26);
            target.AddBuff<GalvanicCorrosion>(8 * 60);
            target.AddBuff<Crumbling>(8 * 60);
            target.AddBuff(BuffID.Electrified, 8 * 60);
            if (shake)
                ScreenShaker.AddShake(new ScreenShaker.ScreenShake(Projectile.velocity.normalize() * -2, 4f));
            shake = false;
        }
    }
}
