using System;
using System.Collections.Generic;

namespace CalamityEntropy.Core.Hooks
{
    /// <summary>订阅和退订各一个无参 lambda,少写一边编译不过;Unload 统一调 UndoAll</summary>
    internal static class CEDetourRegistry
    {
        //模块用 GetUninitializedObject 创建,实例字段初始化器不执行,所以登记表必须是静态的
        private static readonly List<Action> UndoActions = new List<Action>();

        /// <summary>已登记的钩子数,供加载期自查用</summary>
        public static int Count => UndoActions.Count;

        /// <summary>方法组写进 lambda 才对着 On_* 的具名委托转换,当参数传会被推断成 Action</summary>
        public static void Add(Action subscribe, Action unsubscribe) {
            subscribe();
            UndoActions.Add(unsubscribe);
        }

        /// <summary>按登记的逆序全部退订。可重复调用</summary>
        public static void UndoAll() {
            for (int i = UndoActions.Count - 1; i >= 0; i--) {
                UndoActions[i]();
            }
            UndoActions.Clear();
        }
    }
}
