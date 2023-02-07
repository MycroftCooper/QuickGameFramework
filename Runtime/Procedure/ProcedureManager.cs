using System;
using System.Collections.Generic;
using MycroftToolkit.QuickCode;
using QuickGameFramework.Runtime;

namespace QuickGameFramework.Procedure {
    public class ProcedureManager:IModule {
        private Dictionary<Type, Procedure> _procedures;

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        public int Priority { get => -2; set { } }
        public bool IsFrameworkModule => true;

        public void OnModuleCreate(params object[] createParam) {
            _procedures = new Dictionary<Type, Procedure>();
        }
        
        public void OnModuleUpdate(float intervalSeconds) {
            _procedures.Values.ForEach((x)=>x.Update(intervalSeconds));
        }
        
        public void OnModuleDestroy() {
            _procedures.Values.ForEach((x) => {
                x.Destroy();
            });
            _procedures.Clear();
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : Procedure {
            return _procedures.ContainsKey(typeof(T));
        }
        
        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <param name="procedureType">要检查的流程类型。</param>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure(Type procedureType) {
            return _procedures.ContainsKey(procedureType);
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public T GetProcedure<T>() where T : Procedure {
            if (HasProcedure<T>()) return (T)_procedures[typeof(T)];
            QLog.Error($"QuickGameFramework>Procedure>获取流程<{typeof(T).Name}>失败，该流程未开启！");
            return default;
        }
        
        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <param name="procedureType">要获取的流程类型。</param>
        /// <returns>要获取的流程。</returns>
        public Procedure GetProcedure(Type procedureType) {
            if (HasProcedure(procedureType)) return _procedures[procedureType];
            QLog.Error($"QuickGameFramework>Procedure>获取流程<{procedureType.Name}>失败，该流程未开启！");
            return default;
        }
        
        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        /// <param name="parameters">流程初始化参数</param>
        public void StartProcedure<T>(params object[] parameters) where T : Procedure,new() {
            if (HasProcedure<T>()) {
                QLog.Error($"QuickGameFramework>Procedure>流程<{typeof(T).Name}>开启失败，已存在相同流程！");
                return;
            }
            Procedure target = new T();
            target.Init(parameters);
            _procedures.Add(typeof(T), target);
            target.Enter();
        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <param name="procedureType">要开始的流程类型。</param>
        /// <param name="parameters">流程初始化参数</param>
        public void StartProcedure(Type procedureType, params object[] parameters) {
            if (procedureType.BaseType != typeof(Procedure)) {
                QLog.Error($"QuickGameFramework>Procedure>流程<{procedureType.Name}>开启失败，该类型不是流程<Procedure>！");
                return;
            }
            if (HasProcedure(procedureType)) {
                QLog.Error($"QuickGameFramework>Procedure>流程<{procedureType.Name}>开启失败，已存在相同流程！");
                return;
            }

            Procedure target = QuickReflect.CreateInstance<Procedure>(procedureType.FullName);
            target.Init(parameters);
            _procedures.Add(procedureType, target);
            target.Enter();
        }

        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public float GetProcedureDuration<T>() where T : Procedure {
            return GetProcedure<T>().ProcessDuration;
        }
        
        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public float GetProcedureDuration(Type procedureType) {
            return GetProcedure(procedureType).ProcessDuration;
        }
    }
}