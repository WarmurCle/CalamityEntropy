using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class TabooVolume : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 125;
            Item.useAnimation = Item.useTime = 100;
            Item.crit = 10;
            Item.mana = 8;
            Item.shootSpeed = 29;
            Item.rare = ModContent.RarityType<CalamityRed>();
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark8").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<TabooVolumeHeld>();
        public override int SlotCount => 6;

        public override void AddRecipes()
        {

            CreateRecipe().AddIngredient<VoidOde>()
                .AddIngredient<Heresy>()
                .AddIngredient<AshesofAnnihilation>(6)
                .AddTile<SCalAltarLarge>()
                .Register();
        }
    }

    public class TabooVolumeHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/TabooVolume/TabooVolumeOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/TabooVolume/TabooVolumePage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/TabooVolume/TabooVolumeUI";

        public float seekerRot = 0;
        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.lifeSteal += 2;
            m.PenetrateAddition += 2;
            m.armorPenetration += 40;
            return m;
        }
        public override float randomShootRotMax => 0.01f;
        public bool SeekerShoot = false;
        public override int baseProjectileType => SeekerShoot ? ModContent.ProjectileType<BrimstoneHellblastFriendly>() : ModContent.ProjectileType<BrimstoneGigaBlastFriendly>();
        public override EBookProjectileEffect getEffect()
        {
            return new TabooVolumeBookBaseEffect();
        }
        public int seekerCd = 0;
        public override int frameChange => 3;
        public float seekerRotTarget = 0;
        public override void AI()
        {
            base.AI();
            seekerRot += (seekerRotTarget - seekerRot) * 0.1f;
            if (!active)
            {
                seekerRotTarget += 0.04f;
            }
            else
            {
                if(seekerCd-- <= 0)
                {
                    seekerCd = this.GetShootCd() / 6;
                    SeekerShoot = true;
                    this.Shoot();
                    SeekerShoot = false;
                }
            }
            if (Main.GameUpdateCount % 15 == 0 && active)
            {
                base.playTurnPageAnimation();
            }
        }
        public override void playTurnPageAnimation()
        {

        }
        public override bool Shoot()
        {
            if(SeekerShoot)
            {
                seekerRotTarget += MathHelper.ToRadians(60);
                var seekers = getSeekerPos();
                Vector2 opos = Projectile.Center;
                float oRot = Projectile.rotation;
                Vector2 oVel = Projectile.velocity;
                foreach (var sp in seekers)
                {
                    Projectile.Center = sp;
                    Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length();
                    base.Shoot();
                    EParticle.NewParticle(new HadCircle2() { scale2 = 0.4f }, Projectile.Center, Vector2.Zero, Color.OrangeRed, 1, 1, true, BlendState.Additive, 0);
                }
                Projectile.rotation = oRot;
                Projectile.Center = opos;
                Projectile.velocity = oVel;
                Projectile.localAI[0]++;
                return true;
            }
            
            return base.Shoot();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Books/SoulSeekerSupreme").Value;
            foreach (Vector2 pos in getSeekerPos())
            {
                Main.EntitySpriteDraw(tex, pos - Main.screenPosition, CEUtils.GetCutTexRect(tex, 6, ((int)Main.GameUpdateCount / 4) % 6, false), lightColor, 0, new Vector2(48, 65), Projectile.scale, (Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally));
            }
            return base.PreDraw(ref lightColor);
        }
        public List<Vector2> getSeekerPos()
        {
            int dist = 120;
            List<Vector2> pos = new List<Vector2>();
            int count = 3;
            for (int i = 0; i < count; i++)
            {
                float rot = MathHelper.ToRadians(360f / count) * i + seekerRot;
                pos.Add(Projectile.Center + rot.ToRotationVector2() * dist);
            }
            return pos;
        }
    }
    public class TabooVolumeBookBaseEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 4 * 60);
        }
    }
}
