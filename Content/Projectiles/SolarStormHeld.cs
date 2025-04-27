using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Utilities;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class SolarStormHeld : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.DefineSynchronousData(Common.SyncDataType.Float, "shotCounter", 0f);
        }
        public float shotCounter { get { return Projectile.GetSyncValue<float>("shotCounter"); } set { Projectile.SetSyncValue("shotCounter", value); } }
        
        int ammoID = -1;
        public float tShooted = 0;
        public float ShootNeededTime = 30;
        public float maxChargeTime = 250;
        public override void AI()
        {
            Player player = Projectile.owner.ToPlayer();
            if (player.dead)
            {
                Projectile.Kill();
            }
            if (Projectile.Entropy().IndexOfTwistedTwinShootedThisProj != -1 && !(Projectile.Entropy().IndexOfTwistedTwinShootedThisProj.ToProj().active))
            {
                Projectile.Kill();
            }
            if (player.channel)
            {
                Projectile.timeLeft = 3;
                player.itemTime = player.itemAnimation = 2;
            }
            player.heldProj = Projectile.whoAmI;
            HandleChannelMovement(player, player.MountedCenter);
            if (player.HasAmmo(player.HeldItem))
            {
                player.PickAmmo(player.HeldItem, out int projID, out float shootSpeed, out int damage, out float kb, out ammoID, true);
                shotCounter += player.GetTotalAttackSpeed(Projectile.DamageType);
                if (shotCounter < maxChargeTime && shotCounter >= tShooted + ShootNeededTime)
                {
                    tShooted += ShootNeededTime;
                    ShootNeededTime -= 1.6f;
                    if(Main.myPlayer == Projectile.owner)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int p = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + Utilities.Util.randomPointInCircle(8), Projectile.velocity.SafeNormalize(Vector2.One) * shootSpeed, projID, damage, kb, player.whoAmI);
                            p.ToProj().Update(p);
                        }
                        var snd = SoundID.DD2_BallistaTowerShot;
                        snd.MaxInstances = 5;
                        SoundEngine.PlaySound(snd, Projectile.Center);
                    }
                }
                if(shotCounter > maxChargeTime + 36)
                {
                    shotCounter = 0;
                    tShooted = 0;
                    ShootNeededTime = 30;
                }
            }
        }
        public void HandleChannelMovement(Player player, Vector2 playerRotatedPoint)
        {
            Projectile.Center = player.MountedCenter + player.gfxOffY * Vector2.UnitY;
            if (Projectile.velocity.X > 0)
            {
                player.direction = 1;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            else
            {
                player.direction = -1;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            float speed = 16f;
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = newVelocity;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.getTexture();
            Texture2D glow = this.getTextureGlow();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);

            return false;
        }
        public void DrawChargingEnergyBall(Vector2 pos, float size, float alpha)
        {
            Texture2D tex = Util.getExtraTex("a_circle");
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }

}