using Terraria.ModLoader;

namespace CalamityEntropy.Content.UI
{
    public class NMSPLayer : ModPlayer
    {
        public int SwingIndex;
        public int DontUseItemTime;
        public override void PostUpdate()
        {
            if (DontUseItemTime > 0)
            {
                DontUseItemTime--;
            }
        }
    }
}
