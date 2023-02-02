using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QuickGameFramework.Runtime {
	public static class ModuleManager {
		private class ModuleInfo {
			public int Priority { private set; get; }
			public IModule Module { private set; get; }
			public ModuleInfo(IModule module, int priority) {
				Module = module;
				Priority = priority;
			}
		}

		private static bool _isInitialize;
		private static GameObject _driver;
		private static readonly List<ModuleInfo> ModuleInfos = new List<ModuleInfo>(100);
		private static MonoBehaviour _behaviour;
		private static bool _isDirty;

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
			DestroyAll();
			_isInitialize = false;
			if (_driver == null) {
				QLog.Log($"QuickGameFramework>Module> 所有模块成功销毁!");
				return;
			}
			Object.Destroy(_driver);
		}

		/// <summary>
		/// 更新模块系统
		/// </summary>
		internal static void Update() {
			// 如果需要重新排序
			if (_isDirty) {
				_isDirty = false;
				ModuleInfos.Sort((left, right) => {
					if (left.Priority == right.Priority)
						return 0;
					if (left.Priority > right.Priority)
						return -1;
					return 1;
				});
			}

			// 轮询所有模块
			foreach (var moduleInfo in ModuleInfos) {
				moduleInfo.Module.OnModuleUpdate();
			}
		}

		/// <summary>
		/// 获取模块
		/// </summary>
		public static T GetModule<T>() where T : class, IModule {
			var type = typeof(T);
			var targetModule = ModuleInfos.Find((moduleInfo) => moduleInfo.Module.GetType() == type);
			if (targetModule != null) return (T)targetModule.Module;
			QLog.Error($"QuickGameFramework>Module>获取模块失败！模块{typeof(T).Name}不存在！");
			return null;
		}

		/// <summary>
		/// 查询模块是否存在
		/// </summary>
		public static bool Contains<T>() where T : class, IModule {
			var type = typeof(T);
			return ModuleInfos.Any((moduleInfo) => moduleInfo.Module.GetType() == type);
		}

		/// <summary>
		/// 创建模块
		/// </summary>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(int priority = 0) where T : class, IModule {
			return CreateModule<T>(priority, null);
		}

		/// <summary>
		/// 创建模块
		/// </summary>
		/// <param name="createParam">附加参数</param>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public static T CreateModule<T>(int priority = 0, params System.Object[] createParam) where T : class, IModule {
			if (priority < 0)
				throw new Exception("The priority can not be negative");

			if (Contains<T>())
				throw new Exception($"Module is already existed : {typeof(T)}");

			// 如果没有设置优先级
			if (priority == 0) {
				int minPriority = GetMinPriority();
				priority = --minPriority;
			}

			T module = Activator.CreateInstance<T>();
			ModuleInfo moduleInfo = new ModuleInfo(module, priority);
			moduleInfo.Module.OnModuleCreate(createParam);
			ModuleInfos.Add(moduleInfo);
			_isDirty = true;
			return module;
		}

		/// <summary>
		/// 销毁模块
		/// </summary>
		public static bool DestroyModule<T>() where T : class, IModule {
			var type = typeof(T);
			for (int i = 0; i < ModuleInfos.Count; i++) {
				if (ModuleInfos[i].Module.GetType() != type) continue;
				ModuleInfos[i].Module.OnModuleDestroy();
				ModuleInfos.RemoveAt(i);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 开启一个协程
		/// </summary>
		public static Coroutine StartCoroutine(IEnumerator coroutine) {
			return _behaviour.StartCoroutine(coroutine);
		}

		public static Coroutine StartCoroutine(string methodName) {
			return _behaviour.StartCoroutine(methodName);
		}

		/// <summary>
		/// 停止一个协程
		/// </summary>
		public static void StopCoroutine(Coroutine coroutine) {
			_behaviour.StopCoroutine(coroutine);
		}

		public static void StopCoroutine(string methodName) {
			_behaviour.StopCoroutine(methodName);
		}

		/// <summary>
		/// 停止所有协程
		/// </summary>
		public static void StopAllCoroutines() {
			_behaviour.StopAllCoroutines();
		}

		private static int GetMinPriority() {
			return ModuleInfos.Select(t => t.Priority).Prepend(0).Min();
		}

		private static void DestroyAll() {
			foreach (var t in ModuleInfos) {
				t.Module.OnModuleDestroy();
			}

			ModuleInfos.Clear();
		}
	}
}