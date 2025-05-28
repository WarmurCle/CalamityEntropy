namespace CalamityEntropy
{
    /// <summary>
    /// 提供一个通用的资源加载、卸载途径
    /// </summary>
    internal interface ICELoader
    {
        public void LoadAsset() { }
        public void SetupData() { }
        public void LoadData() { }
        public void UnLoadData() { }

        internal void DompLoadText() { }
        internal void DompUnLoadText() { }
    }
}
