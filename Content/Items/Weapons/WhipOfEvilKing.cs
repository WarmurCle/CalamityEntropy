using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class WhipOfEvilKing : BaseWhipItem, IPriceFromRecipe, IGetFromStarterBag
    {
        public override int TagDamage => 3;
        public override float TagCritChance => 0.05f;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<EvilKingWhipProjectile>(), 6, 4, 8f, 26);
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.width = 44;
            Item.height = 38;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientBoneDust>()
                .AddIngredient(ItemID.Silk, 8)
                .AddRecipeGroup(CERecipeGroups.evilBar, 5)
                .AddIngredient(ItemID.GoldCoin, 99)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public bool OwnAble(Player player, ref int count)
        {
            if (player.Entropy().drCrystals == null) return false;
            return player.Entropy().drCrystals[0];
        }
    }
    public class EvilKingWhipProjectile : BaseWhip
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.MaxUpdates = 4;
            this.segments = 30;
            this.rangeMult = 1;
        }
        public override string getTagEffectName => "EvilKingWhip";
        public override Color StringColor => new Color(24, 24, 24);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override int handleHeight => 18;
        public override int segHeight => 16;
        public override int endHeight => 24;
        public override int segTypes => 1;
    }
    public class Spade : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, false, -1);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 40;
            Projectile.width = Projectile.height = 30;
        }
        public override void AI()
        {
            Projectile.ai[0] = float.Lerp(Projectile.ai[0], 1, 0.3f);
            Projectile.ai[1] = float.Lerp(Projectile.ai[1], 1, 0.1f);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Demonite);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Vector2 origin = tex.Size() / 2f;
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
            {
                Vector2 offset = i.ToRotationVector2() * 4;
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, null, Color.Black * Projectile.ai[1], 0, origin, Projectile.scale * Projectile.ai[0], SpriteEffects.None);
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.ai[1], 0, origin, Projectile.scale * Projectile.ai[0], SpriteEffects.None);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 30;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft += 20;
            if (Projectile.timeLeft > 80)
                Projectile.timeLeft = 80;
            EParticle.spawnNew(new AbyssalLine() { lx = 0.8f, xadd = 0.62f, spawnColor = Color.White, endColor = Color.Black }, Projectile.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, 0, 32);
            EParticle.spawnNew(new AbyssalLine() { lx = 0.8f, xadd = 0.62f, spawnColor = Color.White, endColor = Color.Black }, Projectile.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, MathHelper.PiOver2, 32);
            Projectile.ai[0] += 0.16f;
        }
    }
}
