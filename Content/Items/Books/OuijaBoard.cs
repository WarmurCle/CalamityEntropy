using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items.LoreItems;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class OuijaBoard : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 18;
            Item.useTime = Item.useAnimation = 25;
            Item.shootSpeed = 10;
        }
        public override int HeldProjectileType => ModContent.ProjectileType<OuijaBoardHeld>();
        public override int SlotCount => 1; 
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/OB").Value;


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 19)
                .AddRecipeGroup(CERecipeGroups.gems, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class OuijaBoardHeld : EntropyBookDrawingAlt
    {
        public override string OpenAnimationPath => $"{EntropyBook.BaseFolder}/OuijaBoard2";
        public override string PageAnimationPath => $"{EntropyBook.BaseFolder}/OuijaBoard2";
        public override string UIOpenAnimationPath => $"{EntropyBook.BaseFolder}/OuijaBoard2";
        public override int OpenAnmCount => 1;
        public override int PageAnmCount => 1;
        public override int UIOpenAnmCount => 1;
        public override int baseProjectileType => ModContent.ProjectileType<OuijaSpirit>();
        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.Homing += 0.05f;
            return m;
        }
        public override void AI()
        {
            base.AI();
            Projectile.GetOwner().Calamity().mouseWorldListener = true;
        }
        public override void playPageSound()
        {
            CEUtils.PlaySound("SoulSpawn" + Main.rand.Next(2).ToString(), Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
        }
    }
    public class OuijaSpirit : EBookBaseProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.light = 0.15f;
            Projectile.timeLeft = 280;
            Projectile.tileCollide = false;
        }
        public int FacingTime = 38;
        public override void ApplyHoming()
        {
            if(FacingTime <= 0)
                base.ApplyHoming();
        }
        public override void OnKill(int timeLeft)
        {
            GeneralParticleHandler.SpawnParticle(new CalamityMod.Particles.ImpactParticle(Projectile.Center, 0, 9, 0.4f, new Color(255, 255, 255)));
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi).RotatedByRandom(1.2f);
            base.AI();
            if (Projectile.localAI[0]++ > 8)
            {
                if (FacingTime-- > 0)
                {
                    if (CEUtils.getDistance(Projectile.Center, Projectile.GetOwner().Calamity().mouseWorld) < 42)
                        FacingTime = 0;
                    float rott = (Projectile.GetOwner().Calamity().mouseWorld - Projectile.Center).ToRotation();
                    Projectile.velocity = CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), rott, 0.06f, true).ToRotationVector2() * Projectile.velocity.Length();
                    Projectile.velocity = CEUtils.RotateTowardsAngle(Projectile.velocity.ToRotation(), rott, 0.08f, false).ToRotationVector2() * Projectile.velocity.Length();
                }
            }
            if (Projectile.localAI[0] == 150)
                Projectile.tileCollide = true;
            for (float i = 0; i < 1; i += 0.25f)
                EParticle.spawnNew(new GlowLightParticle() { lightColor = Color.White * 0.14f}, Projectile.Center + Projectile.velocity * i, CEUtils.randomPointInCircle(2), new Color(160, 160, 200), Main.rand.NextFloat(0.6f, 1f), 1, true, BlendState.Additive, 0, 24);
            
        }
    }
}
