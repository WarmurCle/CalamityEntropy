using System;
using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Particle = CalamityEntropy.Content.Particles.Particle;

namespace CalamityEntropy.Content.Projectiles
{
    public class AzureOfFirmamentThrow : ModProjectile, IJavelin
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();
        public bool SetHandRot { get; set; }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rougeDC;
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 260;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.ArmorPenetration = 26;
            SetHandRot = true;
        }
        public float handrot = 0;
        public float handrotspeed = 0;
        public Vector2 ownerMouse = Vector2.Zero;
        
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.rotation);
            writer.Write(handrot);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.rotation = reader.ReadSingle();
            handrot = reader.ReadSingle();
        }
        public override void OnSpawn(IEntitySource source)
        {
            foreach (Projectile p in Main.projectile)
            {
                if (p.whoAmI != Projectile.whoAmI)
                {
                    if (p.ModProjectile is IJavelin jv)
                    {
                        jv.SetHandRot = false;
                    }
                }
            }
        }
        public override void PostAI()
        {
            if (Projectile.ai[0] > 10)
            {
                odp.Add(Projectile.Center + Projectile.rotation.ToRotationVector2() * 76);
                odr.Add(Projectile.rotation);
                if (odp.Count > 16)
                {
                    odp.RemoveAt(0);
                    odr.RemoveAt(0);
                }
            }
        }
        public override void AI(){
            
            if (Projectile.ai[0] == 0)
            {
                handrotspeed = -0.3f;
            }
            else if (Projectile.ai[0] < 12)
            {
                handrotspeed += 0.056f;
            }
            if (Projectile.ai[0] < 12)
            {
                
                var owner = Projectile.owner.ToPlayer();

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
                    Projectile.netUpdate = true;
                }
                if (this.SetHandRot)
                {
                    Projectile.owner.ToPlayer().heldProj = Projectile.whoAmI;
                    if (owner.direction == 1)
                    {
                        Projectile.Center = owner.MountedCenter + new Vector2(26, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2 - handrot);
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - handrot - MathHelper.Pi);
                    }
                    else
                    {
                        Projectile.Center = owner.MountedCenter + new Vector2(26, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 + handrot);
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + handrot);
                    }
                }
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Projectile.rotation);
            }
            else if (Projectile.ai[0] < 36)
            {
                handrotspeed *= 0.84f;
                var owner = Projectile.owner.ToPlayer();
                if (this.SetHandRot)
                {
                    if (owner.direction == 1)
                    {
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - handrot - MathHelper.Pi);
                    }
                    else
                    {
                        owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + handrot);

                    }
                    Projectile.owner.ToPlayer().heldProj = -1;
                }
                
            }
            if (Projectile.ai[0] > 12)
            {
                Projectile.tileCollide = true;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            handrot -= handrotspeed;
            if (Projectile.ai[0] == 10)
            {

                SoundStyle SwingSound = SoundID.Item1;
                SwingSound.Pitch = 0f;
                if (Projectile.Calamity().stealthStrike)
                {
                    SwingSound.Pitch = 1f;
                }
                
                SoundEngine.PlaySound(SwingSound, Projectile.Center);
            }

            Projectile.ai[0]++;
            
        }
        public bool sp = true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak1") { Volume = 0.3f }, Projectile.Center);
            if (sp && Projectile.Calamity().stealthStrike)
            {
                sp = false;

                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.ModProjectile is WelkinFeather)
                    {
                        p.ai[1] = target.whoAmI;
                        p.netUpdate = true;
                    }
                }
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[0] >= 12;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if(Projectile.ai[0] <= 10)
            {
                return false;
            }
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tx = TextureAssets.Projectile[Projectile.type].Value;
            float rj = 0;
            if (Projectile.ai[0] < 12)
            {
                rj = -handrot * Projectile.owner.ToPlayer().direction;
            }
            Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4 + rj, tx.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }


    }

}