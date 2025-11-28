using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookmarkFairy : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Fairy");
        public override EBookProjectileEffect getEffect()
        {
            return new BookmarkFairyEffect();
        }

        public override Color tooltipColor => Color.Pink;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(CERecipeGroups.fairys, 1)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        private static int projType = -1;
        public static int ProjType { get { if (projType == -1) { projType = ModContent.ProjectileType<FairyBMMinion>(); } return projType; } }
    }

    public class BookmarkFairyEffect : EBookProjectileEffect
    {
    }
    public class FairyBMMinion : EBookBaseProjectile
    {
        public int Delay = 0;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
            Projectile.width = Projectile.height = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 5;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
        }
        public int num = -1;
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            if (BookMarkLoader.HeldingBookAndHasBookmarkEffect<BookmarkFairyEffect>(player))
            {
                Projectile.timeLeft = 3;
            }
            else
            {
                return;
            }

            float DelayMult = player.GetWeaponAttackSpeed(player.HeldItem);
            Projectile.CritChance = player.GetWeaponCrit(player.HeldItem);
            Projectile.damage = (int)(player.GetWeaponDamage(player.HeldItem) * 0.6f);
            Projectile.MaxUpdates = 1;

            if (CEUtils.getDistance(Projectile.Center, player.Center) > 3000)
            {
                Projectile.Center = player.Center + CEUtils.randomPointInCircle(100);
                Projectile.velocity *= 0;
            }
            Delay--;
            NPC target = CEUtils.FindTarget_HomingProj(Projectile, Vector2.Lerp(player.Center, Projectile.Center, 0.32f), 3400);
            if (--num == 0)
                Projectile.velocity *= 0.36f;
            if (target == null)
            {
                Projectile.pushByOther(0.6f);
                if (CEUtils.getDistance(Projectile.Center, player.Center) > 60)
                {
                    Projectile.velocity += (player.Center - Projectile.Center).normalize() * 0.4f;
                    Projectile.velocity *= 0.97f;
                }
            }
            else
            {
                Projectile.pushByOther(3f);
                if (CEUtils.getDistance(Projectile.Center, target.Center) < 300 && Delay <= 0)
                {
                    EParticle.NewParticle(new DOracleSlash() { centerColor = Color.White, widthMult = 0.4f }, target.Center + (Projectile.Center - target.Center).normalize() * 80, Vector2.Zero, Color.Pink, 140, 1, true, BlendState.Additive, (target.Center - Projectile.Center).ToRotation(), 16);
                    Projectile.ResetLocalNPCHitImmunity();
                    Delay = (int)(Main.rand.Next(40, 82) / DelayMult);
                    num = 8;
                    Projectile.velocity = (target.Center + (target.Center - Projectile.Center).normalize() * 250 - Projectile.Center) / (int)(8 / DelayMult);

                }
                else
                {
                    if (CEUtils.getDistance(Projectile.Center, target.Center) > 180)
                    {
                        Projectile.velocity += (target.Center - Projectile.Center).normalize() * 1f;
                        Projectile.velocity *= 0.96f;
                    }
                    else
                    {
                        Projectile.velocity -= (target.Center - Projectile.Center).normalize() * 1f;
                        Projectile.velocity *= 0.96f;
                    }
                }
                Projectile.rotation = Math.Sign(Projectile.velocity.X);
            }
            Color value = Color.HotPink;
            Color value2 = Color.LightPink;
            int num98 = 4;
            if (Projectile.ai[2] == 1)
            {
                value = Color.LimeGreen;
                value2 = Color.LightSeaGreen;
            }
            if (Projectile.ai[2] == 2)
            {
                value = Color.RoyalBlue;
                value2 = Color.LightBlue;
            }
            if (Main.rand.NextBool(4) || num >= 0)
            {
                Dust dust17 = Dust.NewDustDirect(Projectile.Center - new Vector2(num98) * 0.5f, num98 + 4, num98 + 4, 278, 0f, 0f, 200, Color.Lerp(value, value2, Main.rand.NextFloat()), 0.65f);
                dust17.noGravity = true;
                dust17.velocity = Projectile.velocity * 0.06f;
            }

        }
        public override bool? CanHitNPC(NPC target)
        {
            return num >= 0 ? null : false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CEUtils.RequestTex("CalamityEntropy/Content/Items/Books/BookMarks/Fairy/F" + (1+(int)Projectile.ai[2]));
            Rectangle frame = new Rectangle(0, 24 * (((int)Main.GameUpdateCount / 4) % 4), tex.Width, 22);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, 0, new Vector2(12, 11), Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
        public override string Texture => CEUtils.WhiteTexPath;
    }
}