using QuickGameFramework.Fsm;
using ProcedureOwner = QuickGameFramework.Fsm.IFsm<QuickGameFramework.Procedure.IProcedureManager>;

namespace QuickGameFramework.Procedure {
    /// <summary>
    /// 流程基类。
    /// </summary>
    public abstract class ProcedureBase : FsmState<IProcedureManager> {
        /// <summary>
        /// 状态初始化时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected internal override void OnInit(ProcedureOwner procedureOwner) {
            base.OnInit(procedureOwner);
        }

        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected internal override void OnEnter(ProcedureOwner procedureOwner) {
            base.OnEnter(procedureOwner);
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        /// <param name="intervalSeconds">流逝时间，以秒为单位。</param>
        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float intervalSeconds) {
            base.OnUpdate(procedureOwner, intervalSeconds);
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
            base.OnLeave(procedureOwner, isShutdown);
        }

        /// <summary>
        /// 状态销毁时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected internal override void OnDestroy(ProcedureOwner procedureOwner) {
            base.OnDestroy(procedureOwner);
        }
    }
}