using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Pets
{
    public class LavaPancake : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<ProfPetG1>();
            Item.buffType = ModContent.BuffType<GuardiansBuff>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(Item.buffType, 3600);
            }
            return true;
        }

    }
    public class GuardiansBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<ProfPetG1>());
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<ProfPetG2>());
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<ProfPetG3>());
        }
    }
    public abstract class ProfanedGuardianPet : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public int counter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 42;
            Projectile.height = 42;
        }
        public virtual string TextureName => "";
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            if (Main.gameMenu)
            {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Pets/Prof/" + TextureName + "1").Value;
                Main.EntitySpriteDraw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            List<Texture2D> list = new List<Texture2D>();
            for (int i = 1; i <= texs; i++)
            {
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Pets/Prof/" + TextureName + i.ToString()).Value);
            }
            Texture2D tx = list[(counter / 4) % list.Count];

            if (Projectile.direction == -1)
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            }
            else
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);

            }

            return false;

        }
        public virtual int texs => 5;
        public virtual float MS => 0.1f;
        public virtual Vector2 posOffset => new Vector2(-30, -40);
        void MoveToTarget(Vector2 targetPos)
        {
            Projectile.velocity = (targetPos + posOffset * new Vector2(Projectile.getOwner().direction, 1) - Projectile.Center) * MS;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false;
            return true;
        }

        public override void AI()
        {
            counter++;
            Player player = Main.player[Projectile.owner];
            MoveToTarget(player.Center);
            if (!player.dead && (player.HasBuff(this.Buff) || player.HasBuff(ModContent.BuffType<ProfNGuardBuff>())))
            {
                Projectile.timeLeft = 2;
            }
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
            Projectile.rotation = Projectile.velocity.X * 0.01f;
        }

        public virtual int Buff => ModContent.BuffType<GuardiansBuff>();
    }

    public class ProfPetG1 : ProfanedGuardianPet
    {
        public override string TextureName => "A";
        public override float MS => 0.1f;
        public override Vector2 posOffset => new Vector2(-50, -20);
    }
    public class ProfPetG2 : ProfanedGuardianPet
    {
        public override string TextureName => "B";
        public override float MS => 0.08f;
        public override Vector2 posOffset => new Vector2(-70, -20);
    }
    public class ProfPetG3 : ProfanedGuardianPet
    {
        public override string TextureName => "C";
        public override float MS => 0.06f;
        public override Vector2 posOffset => new Vector2(-90, -20);
    }
}