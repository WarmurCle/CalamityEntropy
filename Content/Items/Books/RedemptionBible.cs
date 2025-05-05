using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class RedemptionBible : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 105;
            Item.useAnimation = Item.useTime = 36;
            Item.crit = 10;
            Item.mana = 15;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark3").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<RedemptionBibleHeld>();
        public override int SlotCount => 3;

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<UpdraftTome>()
                .AddIngredient(ItemID.HallowedBar, 6)
                .AddIngredient(ItemID.SoulofLight, 4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class RedemptionBibleHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/RedemptionBible/RedemptionBibleOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/RedemptionBible/RedemptionBiblePage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/RedemptionBible/RedemptionBibleUI";


        public override float randomShootRotMax => 0;
        public override int baseProjectileType => ModContent.ProjectileType<RedemptionSpear>();
        public float getBowShootCd()
        {
            int _shotCooldown = bookItem.useTime;

            EBookStatModifer m = getBaseModifer();
            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.getOwner().Entropy().EBookStackItems.Count); i++)
            {
                Item it = Projectile.getOwner().Entropy().EBookStackItems[i];
                if (BookMarkLoader.IsABookMark(it))
                {
                    var e = BookMarkLoader.GetEffect(it);
                    BookMarkLoader.ModifyStat(it, m);
                }
            }
            return ((float)_shotCooldown / m.attackSpeed) * 0.36f;
        }
        public float getscale()
        {
            EBookStatModifer m = getBaseModifer();
            for (int i = 0; i < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Projectile.getOwner().Entropy().EBookStackItems.Count); i++)
            {
                Item it = Projectile.getOwner().Entropy().EBookStackItems[i];
                if (BookMarkLoader.IsABookMark(it))
                {
                    var e = BookMarkLoader.GetEffect(it);
                    BookMarkLoader.ModifyStat(it, m);
                }
            }
            return m.Size;
        }
        public override void AI()
        {
            base.AI();
            Player player = Projectile.getOwner();

            if (Main.myPlayer == Projectile.owner)
            {
                int rb = ModContent.ProjectileType<RedemptionBow>();
                if (player.ownedProjectileCounts[rb] < player.ownedProjectileCounts[Projectile.type])
                {
                    ShootSingleProjectile(rb, Projectile.Center, Vector2.Zero);
                }
            }
        }
    }
    public class RedemptionBow : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.netImportant = true;
        }
        public override void ApplyHoming()
        {
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(mouse);
            writer.Write(Projectile.scale);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            mouse = reader.ReadVector2();
            Projectile.scale = reader.ReadSingle();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public Vector2 mouse = Vector2.Zero;
        public override void AI()
        {
            base.AI();
            if (Main.myPlayer == Projectile.owner)
            {
                if (mouse != Main.MouseWorld)
                {
                    mouse = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }
            }
            float dist = 1000;
            NPC target = null;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.friendly && !npc.dontTakeDamage)
                {
                    float d = Utilities.Util.getDistance(npc.Center, mouse);
                    if (d < dist)
                    {
                        target = npc;
                        dist = d;
                    }
                }
            }
            if (target != null)
            {
                Vector2 tpos = target.Center + (Projectile.Center - target.Center).normalize() * 400;
                Projectile.Center += (tpos - Projectile.Center) * 0.18f;
                Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
            }
            else
            {
                Vector2 tpos = Projectile.getOwner().Center + new Vector2(-100 * Projectile.getOwner().direction, -80);
                Projectile.Center += (tpos - Projectile.Center) * 0.16f;
                Projectile.rotation = (mouse - Projectile.Center).ToRotation();
            }
            if (this.ShooterModProjectile is RedemptionBibleHeld eb)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.scale = eb.getscale();
                    Projectile.ai[1] = Projectile.ai[0] / eb.getBowShootCd();
                }
                else
                {
                    Projectile.netUpdate = true;
                    if (Projectile.netSpam >= 10)
                    {
                        Projectile.netSpam = 9;
                    }
                }

                if (eb.active)
                {
                    Projectile.ai[0]++;
                    float shootCd = eb.getBowShootCd();
                    if (Projectile.ai[0] > shootCd)
                    {
                        Projectile.ai[0] -= shootCd;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            eb.ShootSingleProjectile(ModContent.ProjectileType<RedemptionArrow>(), Projectile.Center, Projectile.rotation.ToRotationVector2(), 0.18f, 1, 1.6f);
                        }
                    }
                }
                else
                {
                    Projectile.ai[0] = 0;
                }
            }
            if (Projectile.getOwner().ownedProjectileCounts[ModContent.ProjectileType<RedemptionBibleHeld>()] > 0)
            {
                Projectile.timeLeft = 3;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 top = Projectile.Center + new Vector2(-13, -28).RotatedBy(Projectile.rotation) * Projectile.scale;
            Vector2 but = Projectile.Center + new Vector2(-13, 28).RotatedBy(Projectile.rotation) * Projectile.scale;
            Vector2 mid = Projectile.Center + new Vector2(-13 - (MathHelper.Max(0, Projectile.ai[1] - 0.25f)) * 32, 0).RotatedBy(Projectile.rotation) * Projectile.scale;

            Utilities.Util.drawLine(top, mid, new Color(255, 255, 161), 2 * Projectile.scale, 2);
            Utilities.Util.drawLine(but, mid, new Color(255, 255, 161), 2 * Projectile.scale, 2);

            Main.EntitySpriteDraw(Projectile.getDrawData(Color.White));
            return false;
        }
    }
    public class RedemptionArrow : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 36;
        }
        public override void AI()
        {
            base.AI();
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Main.EntitySpriteDraw(Projectile.getDrawData(Color.White));
            return false;
        }
    }
}