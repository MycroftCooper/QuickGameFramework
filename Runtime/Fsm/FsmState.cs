using System;

namespace QuickGameFramework.Fsm {
    /// <summary>
    /// 有限状态机状态基类。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    public abstract class FsmState<T> where T : class {
        /// <summary>
        /// 初始化有限状态机状态基类的新实例。
        /// </summary>
        protected FsmState() { }

        /// <summary>
        /// 有限状态机状态初始化时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnInit(IFsm<T> fsm) { }

        /// <summary>
        /// 有限状态机状态进入时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnEnter(IFsm<T> fsm) { }

        /// <summary>
        /// 有限状态机状态轮询时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="intervalSeconds">流逝时间，以秒为单位。</param>
        protected internal virtual void OnUpdate(IFsm<T> fsm, float intervalSeconds) { }

        /// <summary>
        /// 有限状态机状态离开时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发。</param>
        protected internal virtual void OnLeave(IFsm<T> fsm, bool isShutdown) { }

        /// <summary>
        /// 有限状态机状态销毁时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnDestroy(IFsm<T> fsm) { }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        /// <param name="fsm">有限状态机引用。</param>
        protected void ChangeState<TState>(IFsm<T> fsm) where TState : FsmState<T> {
            Fsm<T> fsmImplement = (Fsm<T>)fsm;
            if (fsmImplement == null) {
                throw new Exception("QuickGameFramework>FSM>有限状态机无效！");
            }

            fsmImplement.ChangeState<TState>();
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="stateType">要切换到的有限状态机状态类型。</param>
        protected void ChangeState(IFsm<T> fsm, Type stateType) {
            Fsm<T> fsmImplement = (Fsm<T>)fsm;
            if (fsmImplement == null) {
                throw new Exception("QuickGameFramework>FSM>有限状态机无效！");
            }

            if (stateType == null) {
                throw new Exception("QuickGameFramework>FSM>状态类型是无效的！");
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType)) {
                throw new Exception($"QuickGameFramework>FSM>状态类型 '{stateType.FullName}' 无效！");
            }

            fsmImplement.ChangeState(stateType);
        }
    }
}
