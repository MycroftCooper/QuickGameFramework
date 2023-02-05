using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MycroftToolkit.QuickCode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickGameFramework.Runtime {
	public static class ModuleManager {
		private static bool _isInitialize;
		private static GameObject _driver;
		private static readonly SortedSet<IModule> Modules = new (new ModuleComparer());
		private static MonoBehaviour _behaviour;

		/// <summary>
		/// 初始化模块系统
		/// </summary>
		public static void Init() {
			if (_isInitialize) {
				QLog.Warning("QuickGameFramework>Module>模块化系统初始化失败！因为已经初始化，不能重复初始化！");
				return;
			}

			// 创建驱动器
			_isInitialize = true;
			_driver = new GameObject($"[{nameof(ModuleManager)}]");
			_behaviour = _driver.AddComponent<ModuleDriver>();
			Object.DontDestroyOnLoad(_driver);
			QLog.Log($"QuickGameFramework>Module> 模块化系统成功初始化!");
		}

		/// <summary>
		/// 销毁模块系统
		/// </summary>
		public static void Destroy() {
			if (!_isInitialize) return;
			Modules.ForEach((module)=>module.OnModuleDestroy());
			Modules.Clear();
			Object.Destroy(_driver);
			_isInitialize = false;
			QLog.Log($"QuickGameFramework>Module> 所有模块成功销毁!");
		}

		/// <summary>
		/// 更新模块系统
		/// </summary>
		internal static void Update(float intervalSeconds) {
			Modules.ForEach((module)=>module.OnModuleUpdate(intervalSeconds));
		}

		/// <summary>
		/// 获取模块
		/// </summary>
		public static T GetModule<T>() where T : class, IModule {
			var type = typeof(T);
			var targetModule = Modules.FirstOrDefault((module) => module.GetType() == type);
			if (targetModule != null) return (T)targetModule;
			QLog.Error($"QuickGameFramework>Module>获取模块失败！模块{typeof(T).Name}不存在！");
			return null;
		}

		/// <summary>
		/// 查询模块是否存在
		/// </summary>
		public static bool Contains<T>() where T : class, IModule {
			var type = typeof(T);
			return Modules.Any((module) => module.GetType() == type);
		}

		/// <summary>
		/// 创建模块
		/// </summary>
		/// <param name="createParam">附加参数</param>
		/// <param name="priority">运行时的优先级，从0开始往大数执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(int priority = -1, params System.Object[] createParam) where T : class, IModule {
			if (Contains<T>()) {
				QLog.Error($"QuickGameFramework>Module>模块<{typeof(T)}>创建失败:该模块已存在!");
				return null;
			}
			
			// 如果没有设置优先级
			if (priority < 0) {
				priority = Modules.Max.Priority + 1;
			}

			T module = Activator.CreateInstance<T>();
			module.Priority = priority;
			module.OnModuleCreate(createParam);
			Modules.Add(module);
			QLog.Log($"QuickGameFramework>Module>模块<{typeof(T)}>创建成功!优先级:{priority}");
			return module;
		}

		/// <summary>
		/// 销毁模块
		/// </summary>
		public static bool DestroyModule<T>() where T : class, IModule {
			var module = GetModule<T>();
			if (module == null) return false;
			module.OnModuleDestroy();
			Modules.Remove(module);
			return true;
		}

		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static Coroutine StartCoroutine(IEnumerator coroutine) {
			return _behaviour.StartCoroutine(coroutine);
		}

		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static Coroutine StartCoroutine(string methodName) {
			return _behaviour.StartCoroutine(methodName);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(Coroutine coroutine) {
			_behaviour.StopCoroutine(coroutine);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(string methodName) {
			_behaviour.StopCoroutine(methodName);
		}

		/// <summary>
		/// 停止所有协程
		/// </summary>
		public static void StopAllCoroutines() {
			_behaviour.StopAllCoroutines();
		}
	}
}