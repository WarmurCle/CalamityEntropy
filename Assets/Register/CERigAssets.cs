using InnoVault;
using InnoVault.Rigs2D.Runtime;

namespace CalamityEntropy.Assets.Register
{
    /// <summary>
    /// 骨架两端都加载,dedServ 只解析 JSON 不取贴图,骨骼落位在服务端也成立
    /// PostSetupContent 之后才有值,宿主惰性 CreateInstance + Bind,别写进字段初始化器
    /// .rig.json 可热重载;巡游者链长随难度,走 CruiserChainRig,不在这里
    /// </summary>
    public static class CERigAssets
    {
        /// <summary>灭心者:身体 + 12 节尾链 + 尾尖,跟随链与贝塞尔链按尾巴样式切换</summary>
        [VaultLoaden("CalamityEntropy/Assets/Rigs/Apsychos")]
        public static Vault2DRig Apsychos;

        /// <summary>虚无噬菌体宿主:三层触手对 + 通往细胞的 30 节绳</summary>
        [VaultLoaden("CalamityEntropy/Assets/Rigs/Nihility")]
        public static Vault2DRig Nihility;

        /// <summary>混沌细胞:八条 12 节跟随触手带</summary>
        [VaultLoaden("CalamityEntropy/Assets/Rigs/ChaoticCell")]
        public static Vault2DRig ChaoticCell;

        /// <summary>小混沌细胞:通往母体的 30 节绳</summary>
        [VaultLoaden("CalamityEntropy/Assets/Rigs/ChaoticCellSmall")]
        public static Vault2DRig ChaoticCellSmall;

        /// <summary>流明蛾:8 帧身体件 + 两条 10 节 verlet 尾带</summary>
        [VaultLoaden("CalamityEntropy/Assets/Rigs/Luminaris")]
        public static Vault2DRig Luminaris;

        /// <summary>卫城机器:四腿步态 + 两条双节瞄准臂 + 鱼叉链带,朝右作图、靠 Mirrored 翻身</summary>
        [VaultLoaden("CalamityEntropy/Assets/Rigs/Acropolis")]
        public static Vault2DRig Acropolis;

        /// <summary>先知:四片翅骨 + 10 节尾带 + 尾环</summary>
        [VaultLoaden("CalamityEntropy/Assets/Rigs/Prophet")]
        public static Vault2DRig Prophet;
    }
}
