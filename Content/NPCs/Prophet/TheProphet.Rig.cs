using CalamityEntropy.Assets.Register;
using CalamityEntropy.Content.NPCs.Prophet.Core;
using InnoVault.Rigs2D.Runtime;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet
{
    /// <summary>
    /// 四片翅统一跟 rl,原内翅左片绕的是 NPC.rotation,这里并到同一个根朝向
    /// 带状件骨序是尖到根,贴图叉尾在 u = 0
    /// 相位取 rig.Time,不读 GameUpdateCount,服务端也 Step
    /// </summary>
    public partial class TheProphet
    {
        private Rig2DInstance rig;
        [Rig2DBone("wing2L", "wing2R", "wing1L", "wing1R")]
        private readonly int[] wingBones = new int[4];
        [Rig2DPiece("body")]
        private int bodyPiece;

        /// <summary>骨架是否可用(资产已加载、句柄全部命中、已完成首次求解)</summary>
        public bool RigReady => rig != null && rig.Bound && rig.Built;

        /// <summary>资产在 PostSetupContent 之后才有,首帧 AI 里建</summary>
        private void EnsureRig() {
            if (rig != null) {
                return;
            }
            Vault2DRig asset = CERigAssets.Prophet;
            if (asset == null || !asset.IsValid) {
                return;
            }
            rig = asset.CreateInstance(NPC.whoAmI);
            rig.Bind(this, OnRigBound);
        }

        /// <summary>天顶只出翅与尾,本体由旧巡洋舰自己画</summary>
        private void OnRigBound(Rig2DInstance r) {
            r.Pieces[bodyPiece].Visible = !Main.zenithWorld;
        }

        /// <summary>根取 Center + velocity,朝向取 rl,出生演出期间也跑</summary>
        private void UpdateRig() {
            EnsureRig();
            if (rig == null || !rig.Bound) {
                return;
            }
            rig.Scale = NPC.scale;
            rig.SetRoot(NPC.Center + NPC.velocity, rl);

            float rotj = FinSwing();
            rig.SetBoneLocalRotation(wingBones[0], -rotj);
            rig.SetBoneLocalRotation(wingBones[1], rotj);
            rig.SetBoneLocalRotation(wingBones[2], -ProphetDirector.Wing1RestAngle + rotj);
            rig.SetBoneLocalRotation(wingBones[3], ProphetDirector.Wing1RestAngle - rotj);
            rig.Step();
        }

        /// <summary>0~1,前 40% 升、后 60% 落,算式照搬 DrawFins</summary>
        private float FinSwing() {
            float rise = ProphetDirector.FinSwingRise;
            return finRotCounter <= rise
                ? CEUtils.GetRepeatedCosFromZeroToOne(finRotCounter / rise, 1)
                : 1 - CEUtils.GetRepeatedCosFromZeroToOne((finRotCounter - rise) / (1 - rise), 1);
        }

        /// <summary>根先写到新位置再 Snap,免得下一帧把瞬移当成甩尾</summary>
        private void SnapRig() {
            if (rig == null || !rig.Bound) {
                return;
            }
            rig.SetRoot(NPC.Center, rl);
            rig.Snap();
        }
    }
}
