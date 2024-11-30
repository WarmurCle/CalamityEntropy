using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using CalamityEntropy.Projectiles;
using CalamityEntropy.Util;
using Terraria.DataStructures;
using System.Security.Permissions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod;
using Terraria.GameContent;
using CalamityEntropy.Items;
using static Terraria.GameContent.Animations.Actions.Sprites;
namespace CalamityEntropy.Projectiles.SamsaraCasket
{
    public class ZeratosFireball0 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = NoneTypeDamageClass.Instance;
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }
        int framecounter = 3;
        int frame = 0;
        public override void AI(){
            Projectile.ArmorPenetration = HorizonssKey.getArmorPen();
            if (frame == 0 && framecounter == 3) {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            }
            framecounter--;
            if (framecounter == 0)
            {
                frame++;
                framecounter = 3;
                if(frame > 4)
                {
                    Projectile.Kill();
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (HorizonssKey.getVoidTouchLevel() > 0)
            {
                EGlobalNPC.AddVoidTouch(target, 80, HorizonssKey.getVoidTouchLevel(), 800, 16);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            lightColor = Color.White;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/SamsaraCasket/ZeratosFireball" + frame.ToString()).Value;
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale * 3, SpriteEffects.None, 0);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            
            return false;
        }

    }

}