using CalamityEntropy.Content.Particles;
using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common;

public class EPlayerDash : ModPlayer
{
    public const int DashDown = 0;
    public const int DashUp = 1;
    public const int DashRight = 2;
    public const int DashLeft = 3;
    public const int DashCooldown = 50; public const int DashDuration = 28;
    public const float DashVelocity = 24f;
    public int DashDir = -1;
    public int DashDirLast = -1;
    public bool DashAccessoryEquipped;
    public bool velt;
    public int DashDelay = 0;
    public int DashTimer = 0;
    public override void ResetEffects()
    {
        DashAccessoryEquipped = false;

        if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
        {
            DashDir = DashDown;
        }
        else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
        {
            DashDir = DashUp;
        }
        else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
        {
            DashDir = DashRight;
        }
        else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
        {
            DashDir = DashLeft;
        }
        else
        {
            DashDir = -1;
        }
    }
    
    public override void PreUpdateMovement()
    {
        if (!Main.dedServ && CalamityKeybinds.DashHotkey.JustPressed)
        {
            if (Player.direction == 1)
            {
                DashDir = DashRight;
            }
            else
            {
                DashDir = DashLeft;
            }
            if (Player.controlLeft)
            {
                DashDir = DashLeft;
            }
            if (Player.controlRight)
            {
                DashDir = DashRight;
            }
        }
        if (!Main.dedServ && CanUseDash() && (DashDir != -1 || CalamityKeybinds.DashHotkey.JustPressed) && DashDelay == 0)
        {
            DashDirLast = DashDir;
            Player.wingTime -= 20;

            Vector2 newVelocity = Player.velocity;

            switch (DashDir)
            {
                case DashUp when Player.velocity.Y > -DashVelocity:
                case DashDown when Player.velocity.Y < DashVelocity:
                    {
                        float dashDirection = DashDir == DashDown ? 1 : -1.3f;
                        newVelocity.Y = dashDirection * DashVelocity;
                        break;
                    }
                case DashLeft when Player.velocity.X > -DashVelocity:
                case DashRight when Player.velocity.X < DashVelocity:
                    {
                        float dashDirection = DashDir == DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * DashVelocity;
                        break;
                    }
                default:
                    return;
            }

            DashDelay = DashCooldown;
            DashTimer = DashDuration;
            Player.velocity = newVelocity;
            Player.immuneTime = 30;
            for (int i = 0; i < 32; i++)
            {
                Particle p = new Particle();
                p.position = Player.Center;
                p.velocity = newVelocity;
                p.alpha = 0.36f;
                p.vd = 0.9f;
                p.velocity += new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101)) / 100;
                VoidParticles.particles.Add(p);
            }
        }

        if (DashDelay > 0)
            DashDelay--;

        if (DashTimer > 0)
        {
            Player.eocDash = DashTimer;
            Player.armorEffectDrawShadowEOCShield = true;
            if (DashDirLast == DashDown)
            {
                Player.velocity.Y = DashVelocity;
            }
            Particle p = new Particle();
            p.position = Player.Center;
            p.velocity = Vector2.Zero;
            p.alpha = 0.34f;
            p.vd = 0.9f;
            VoidParticles.particles.Add(p);
            p = new Particle();
            p.position = Player.Center - new Vector2(Player.direction * -(Player.velocity.X / 2), 0);
            p.velocity = Vector2.Zero;
            p.alpha = 0.34f;
            p.vd = 0.9f;
            VoidParticles.particles.Add(p);
            DashTimer--;
        }
    }

    private bool CanUseDash()
    {
        return DashAccessoryEquipped
               && !Player.setSolar && !Player.mount.Active
               && Player.wingTime > 20;
    }
}