using System.Collections.Generic;
using System.Linq;
using MycroftToolkit.QuickCode;
using Sirenix.OdinInspector;
using UnityEngine;

namespace QuickGameFramework.Runtime {
	public class ModuleManager:MonoBehaviour {
		[ShowInInspector] private readonly SortedSet<IModule> _modules = new SortedSet<IModule>(new ModuleComparer());
		private void Awake() {
			QLog.Log($"QuickGameFramework>Module> 模块化系统成功初始化!");
		}

		/// <summary>
		/// 销毁模块系统
		/// </summary>
		public void DestroyAllModule() {
			_modules.ForEach((module)=>module.OnModuleDestroy());
			_modules.Clear();
			Destroy(this);
			QLog.Log($"QuickGameFramework>Module> 所有模块成功销毁!");
		}

		/// <summary>
		/// 更新模块系统
		/// </summary>
		private void Update() {
			_modules.ForEach((module)=>module.OnModuleUpdate(Time.deltaTime));
		}

		/// <summary>
		/// 获取模块
		/// </summary>
		public T GetModule<T>() where T : class, IModule {
			var type = typeof(T);
			var targetModule = _modules.FirstOrDefault((module) => module.GetType() == type);
			if (targetModule != null) return (T)targetModule;
			QLog.Error($"QuickGameFramework>Module>获取模块失败！模块{typeof(T).Name}不存在！");
			return null;
		}

		/// <summary>
		/// 查询模块是否存在
		/// </summary>
		public bool Contains<T>() where T : class, IModule {
			var type = typeof(T);
			return _modules.Any((module) => module.GetType() == type);
		}

		/// <summary>
		/// 创建模块
		/// </summary>
		/// <param name="createParam">附加参数</param>
		/// <param name="priority">运行时的优先级，从0开始往大数执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public T CreateModule<T>(int priority = -1, params System.Object[] createParam) where T : class, IModule, new() {
			if (Contains<T>()) {
				QLog.Error($"QuickGameFramework>Module>模块<{typeof(T)}>创建失败:该模块已存在!");
				return null;
			}
			
			T module = new T();
			// 如果没有设置优先级
			if (!module.IsFrameworkModule && priority < 0) {
				priority = _modules.Count > 0 ? _modules.Max.Priority + 1 : 0;
			}
			module.Priority = priority;
			
			module.OnModuleCreate(createParam);
			_modules.Add(module);
			QLog.Log($"QuickGameFramework>Module>模块<{typeof(T)}>创建成功!优先级:{priority}");
			return module;
		}

		/// <summary>
		/// 销毁模块
		/// </summary>
		public bool DestroyModule<T>() where T : class, IModule {
			var module = GetModule<T>();
			if (module == null) return false;
			module.OnModuleDestroy();
			_modules.Remove(module);
			return true;
		}
	}
}