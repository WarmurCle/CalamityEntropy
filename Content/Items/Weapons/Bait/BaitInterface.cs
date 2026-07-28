using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Bait
{
    public interface IBaitItem
    { }

    public abstract class BaitProj : ModProjectile
    {
        public bool IsActive = true;
        public int StickNPC = -1;
        public Vector2 StickOffset = Vector2.Zero;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StickNPC);
            writer.WriteVector2(StickOffset);
            writer.Write(IsActive);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StickNPC = reader.ReadInt32();
            StickOffset = reader.ReadVector2();
            IsActive = reader.ReadBoolean();
        }
        public float Counter { get { return Projectile.localAI[0]; } set { Projectile.localAI[0] = value; } }
        public float ActiveCounter { get { return Projectile.localAI[1]; } set { Projectile.localAI[1] = value; } }
        public int TagDamage => (int)Projectile.ai[2];
    }
}

