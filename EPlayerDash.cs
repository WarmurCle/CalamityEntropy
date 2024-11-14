using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityEntropy;
using CalamityMod;

public class EPlayerDash : ModPlayer
{
    // These indicate what direction is what in the timer arrays used
    public const int DashDown = 0;
    public const int DashUp = 1;
    public const int DashRight = 2;
    public const int DashLeft = 3;

    public const int DashCooldown = 50; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
    public const int DashDuration = 28; // Duration of the dash afterimage effect in frames

    public const float DashVelocity = 24f;

    // The direction the player has double tapped.  Defaults to -1 for no dash double tap
    public int DashDir = -1;

    // The fields related to the dash accessory
    public bool DashAccessoryEquipped;
    public bool velt;
    public int DashDelay = 0; // frames remaining till we can dash again
    public int DashTimer = 0; // frames remaining in the dash

    public override void ResetEffects()
    {
        // Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
        DashAccessoryEquipped = false;

        // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
        // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
        // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
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
    public int DashDirLast = -1;
    // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
    // If they double tapped this frame, they'll move fast this frame
    public override void PreUpdateMovement()
    {
        // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
        if (CalamityKeybinds.DashHotkey.JustPressed)
        {
            if (Player.direction == 1) {
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
        if (CanUseDash() && (DashDir != -1 || CalamityKeybinds.DashHotkey.JustPressed) && DashDelay == 0)
        {
            DashDirLast = DashDir;
            Player.wingTime -= 20;
            
            Vector2 newVelocity = Player.velocity;

            switch (DashDir)
            {
                // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                case DashUp when Player.velocity.Y > -DashVelocity:
                case DashDown when Player.velocity.Y < DashVelocity:
                    {
                        // Y-velocity is set here
                        // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
                        // This adjustment is roughly 1.3x the intended dash velocity
                        float dashDirection = DashDir == DashDown ? 1 : -1.3f;
                        newVelocity.Y = dashDirection * DashVelocity;
                        break;
                    }
                case DashLeft when Player.velocity.X > -DashVelocity:
                case DashRight when Player.velocity.X < DashVelocity:
                    {
                        // X-velocity is set here
                        float dashDirection = DashDir == DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * DashVelocity;
                        break;
                    }
                default:
                    return; // not moving fast enough, so don't start our dash
            }

            // start our dash
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
        { // dash is active
          // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
          // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
          // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
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
            // count down frames remaining
            DashTimer--;
        }
    }

    private bool CanUseDash()
    {
        return DashAccessoryEquipped
            && !Player.setSolar // player isn't wearing solar armor
            && !Player.mount.Active
            && Player.wingTime > 20; // player isn't mounted, since dashes on a mount look weird
            
    }
}