namespace QuickGameFramework.Procedure {
    public abstract class Procedure {
        public float ProcessDuration { get; protected set; }
        internal abstract void Init(params object[] parameters);
        
        internal void Enter() {
            ProcessDuration = 0;
            OnEnter();
        }
        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        protected abstract void OnEnter();

       
        internal void Update(float intervalSeconds) {
            ProcessDuration += intervalSeconds;
            OnUpdate(intervalSeconds);
        }
        
        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        /// <param name="intervalSeconds">流逝时间，以秒为单位。</param>
        protected abstract void OnUpdate(float intervalSeconds);

        protected void Exit() {
            OnExit();
            OnDestroy();
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        protected abstract void OnExit();

        internal void Destroy() {
            OnDestroy();
        }
        
        /// <summary>
        /// 状态销毁时调用。
        /// </summary>
        protected abstract void OnDestroy();
    }
}