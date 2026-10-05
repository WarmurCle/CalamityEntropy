using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace CalamityEntropy.Assets.Register
{
    //这些字段在专用服务器上始终是 null,只能在绘制这类客户端路径读取
    public class AssetsRegister : ModSystem
    {
        [VaultLoaden("CalamityEntropy/Assets/Extra/Tornade_Fire")]
        public static Asset<Texture2D> FireTornado;
    }
}
