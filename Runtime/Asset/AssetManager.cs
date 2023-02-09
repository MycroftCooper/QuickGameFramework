using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MycroftToolkit.QuickCode;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace QuickGameFramework.Runtime {
	public class AssetManager {
		private ProjectAssetSetting _projectAssetSetting;
		private Dictionary<string, AssetsPackage> _packages;

		public void Init() {
			_projectAssetSetting = Resources.Load<ProjectAssetSetting>("ProjectAssetSetting");
			if (_projectAssetSetting == null) {
				QLog.Error($"QuickGameFramework>Asset>项目资源设置缺失！加载失败!\n"+
				           "请在<Resources目录>下增加<ProjectAssetSetting>\n");
				return;
			}

			_packages = new Dictionary<string, AssetsPackage>();
			YooAssets.Initialize();
			GameEntry.CoroutineManager.StartCoroutine(InitPackage());
		}

		#region 资源加载API
		public AssetOperationHandle LoadAssetAsync<T>(string path, Action<T> callback ,string packageName = null) where T : Object {
			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				QLog.Error($"QuickGameFramework>Asset>资源<{path}>异步加载失败!");
				return null;
			}
			var handle = package.LoadAssetAsync<T>(path);
			if (callback != null) {
				handle.Completed += _=> callback((T)handle.AssetObject);
			}
			handle.Completed += _ => { QLog.Log($"QuickGameFramework>Asset>资源<{path}>异步加载成功!"); };
			return handle;
		}

		public AssetOperationHandle[] LoadAssetsAsyncByTag<T>(string tag, Action<T> callback, string packageName = null)
			where T : Object {
			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				QLog.Error($"QuickGameFramework>Asset>Tag:<{tag}>相关资源异步加载失败!");
				return null;
			}

			AssetInfo[] infos = package.GetAssetInfos(tag);
			if (infos == null || infos.Length == 0) {
				QLog.Error($"QuickGameFramework>Asset>Tag:<{tag}>相关资源异步加载失败!tag不存在或该tag下无资源!");
				return null;
			}

			var output = new List<AssetOperationHandle>();
			foreach (var info in infos) {
				if (info.AssetType != typeof(T)) {
					continue;
				}
				var path = info.Address;
				var handle = package.LoadAssetAsync<T>(path);
				if (callback != null) {
					handle.Completed += _=> callback((T)handle.AssetObject);
				}
				handle.Completed += _ => { QLog.Log($"QuickGameFramework>Asset>资源<{path}>异步加载成功!"); };
				output.Add(handle);
			}
			return output.ToArray();
		}

		public AssetOperationHandle LoadAssetSync<T>(out T asset ,string path, string packageName = null) where T : Object {
			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				QLog.Error($"QuickGameFramework>Asset>资源<{path}>同步加载失败!");
				asset = null;
				return null;
			}
			var handle = package.LoadAssetSync<T>(path);
			asset =(T) package.LoadAssetSync<T>(path).AssetObject;
			QLog.Log($"QuickGameFramework>Asset>资源<{path}>同步加载成功!");
			return handle;
		}
		
		public AssetOperationHandle[] LoadAssetsSyncByTag<T>(string tag, Action<T> callback, string packageName = null)
			where T : Object {
			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				QLog.Error($"QuickGameFramework>Asset>Tag:<{tag}>相关资源同步加载失败!");
				return null;
			}

			AssetInfo[] infos = package.GetAssetInfos(tag);
			if (infos == null || infos.Length == 0) {
				QLog.Error($"QuickGameFramework>Asset>Tag:<{tag}>相关资源同步加载失败!tag不存在或该tag下无资源!");
				return null;
			}

			var output = new List<AssetOperationHandle>();
			foreach (var info in infos) {
				if (info.AssetType != typeof(T)) {
					continue;
				}
				var path = info.Address;
				var handle = package.LoadAssetSync<T>(path);
				if (callback != null) {
					handle.Completed += _=> callback((T)handle.AssetObject);
				}
				handle.Completed += _ => { QLog.Log($"QuickGameFramework>Asset>资源<{path}>异步加载成功!"); };
				output.Add(handle);
			}
			return output.ToArray();
		}

		public SceneOperationHandle LoadSceneAsync(string path, string packageName = null, LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true) {
			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				QLog.Error($"QuickGameFramework>Asset>场景<{path}>！异步加载失败!");
				return null;
			}
			SceneOperationHandle handle = package.LoadSceneAsync(path, sceneMode, activateOnLoad);
			handle.Completed += _ => { QLog.Log($"QuickGameFramework>Asset>场景<{path}>！异步加载成功!"); };
			return handle;
		}

		public AssetOperationHandle LoadAndInitPrefabAsync(string path, (Transform parent, Vector3 pos, Quaternion rotation) gameObjectInfo ,Action<GameObject> callback = null ,string packageName = null) {
			AssetOperationHandle handle = LoadAssetAsync<GameObject>(path, null, packageName);
			if (handle == null) {
				QLog.Error($"QuickGameFramework>Asset>预制体<{path}>！异步加载失败!");
				return null;
			}
			handle.Completed += (x) => {
				var target = x.InstantiateSync(gameObjectInfo.pos, gameObjectInfo.rotation, gameObjectInfo.parent);
				callback?.Invoke(target);
				QLog.Log($"QuickGameFramework>Asset>预制体<{path}>！异步加载并实例化成功!");
			};
			return handle;
		}
		#endregion
		
		public bool ReleaseAsset(AssetOperationHandle handle) {
			if (handle == null) {
				QLog.Error($"QuickGameFramework>Asset>资源释放失败，句柄为空!");
				return false;
			}
			handle.Release();
			return true;
		}

		public void UnloadAssets(string packageName = null, bool isForce = false) {
			if (string.IsNullOrEmpty(packageName)) {
				_packages.Values.ForEach((x) => {
					if (isForce) {
						x.ForceUnloadAllAssets();
						QLog.Log($"QuickGameFramework>Asset>资源包{packageName}所有资源强制卸载成功");
					}
					else {
						x.UnloadUnusedAssets();
						QLog.Log($"QuickGameFramework>Asset>资源包{packageName}未使用的资源卸载成功");
					}
				});
				UpdatePackageDict();
				return;
			}

			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				QLog.Error($"QuickGameFramework>Asset>资源包{packageName}卸载资源失败，可能未加载或不存在!");
				return;
			}

			if (isForce) {
				package.ForceUnloadAllAssets();
				QLog.Log($"QuickGameFramework>Asset>资源包{packageName}所有资源强制卸载成功");
			}
			else {
				package.UnloadUnusedAssets();
				QLog.Log($"QuickGameFramework>Asset>资源包{packageName}未使用的资源卸载成功");
			}

			UpdatePackageDict();
		}

		
		private void UpdatePackageDict() {
			List<string> needRemovePackages = _packages.Keys.Where(
				packageName => !YooAssets.HasAssetsPackage(packageName)
				).ToList();
			foreach (var packageName in needRemovePackages) {
				_packages.Remove(packageName);
			}
		}

		private bool GetAssetPackage(string packageName, out AssetsPackage package) {
			if (string.IsNullOrEmpty(packageName)) {
				packageName = _projectAssetSetting.defaultPackageName;
			}
			if (_packages.ContainsKey(packageName)) {
				package = _packages[packageName];
				return true;
			}
			package = YooAssets.TryGetAssetsPackage(packageName);
			if (package != null) {
				if (!_packages.ContainsKey(packageName)) {
					_packages.Add(packageName, package);
				}
				return true;
			}
			QLog.Warning($"QuickGameFramework>Asset>资源包<{packageName}>不存在!");
			return false;
		}
		
		private IEnumerator InitPackage() {
			var defaultPackageName = _projectAssetSetting.defaultPackageName;
			var playMode = _projectAssetSetting.playMode;

			// 创建默认的资源包
			var package = YooAssets.TryGetAssetsPackage(defaultPackageName);
			if (package == null) {
				package = YooAssets.CreateAssetsPackage(defaultPackageName);
				YooAssets.SetDefaultAssetsPackage(package);
			}
			_packages.Add(_projectAssetSetting.defaultPackageName, package);

			InitializationOperation initializationOperation = null;
			switch (playMode) {
				// 编辑器下的模拟模式
				case EPlayMode.EditorSimulateMode: {
					var createParameters = new EditorSimulateModeParameters {
						SimulatePatchManifestPath = EditorSimulateModeHelper.SimulateBuild(defaultPackageName)
					};
					initializationOperation = package.InitializeAsync(createParameters);
					break;
				}

				// 单机运行模式
				case EPlayMode.OfflinePlayMode: {
					var createParameters = new OfflinePlayModeParameters();
					// createParameters.DecryptionServices 可提供资源包加密类
					initializationOperation = package.InitializeAsync(createParameters);
					break;
				}

				// 联机运行模式
				case EPlayMode.HostPlayMode: {
					var createParameters = new HostPlayModeParameters {
						// DecryptionServices = 可提供资源包加密类
						// QueryServices = new GameQueryServices(); 内置文件查询服务类
						DefaultHostServer = GetHostServerURL(true),
						FallbackHostServer = GetHostServerURL(false)
					};
					initializationOperation = package.InitializeAsync(createParameters);
					break;
				}
			}

			yield return initializationOperation;

			if (initializationOperation !=null && package.InitializeStatus != EOperationStatus.Succeed) {
				QLog.Error($"QuickGameFramework>Asset>初始资源包<{defaultPackageName}>以<{playMode}模式>加载失败!\n"+
				              $"{initializationOperation.Error}");
			} else {
				QLog.Log($"QuickGameFramework>Asset>初始资源包<{defaultPackageName}>以<{playMode}模式>加载成功!");
			}
		}
		
		private string GetHostServerURL(bool isBackup) {
			var hostServerIP = isBackup? _projectAssetSetting.backupHostServerIP : _projectAssetSetting.hostServerIP;
			var gameVersion = _projectAssetSetting.gameVersion;
#if UNITY_EDITOR
			return UnityEditor.EditorUserBuildSettings.activeBuildTarget switch {
				UnityEditor.BuildTarget.Android => $"{hostServerIP}/CDN/Android/{gameVersion}",
				UnityEditor.BuildTarget.iOS => $"{hostServerIP}/CDN/IPhone/{gameVersion}",
				UnityEditor.BuildTarget.WebGL => $"{hostServerIP}/CDN/WebGL/{gameVersion}",
				_ => $"{hostServerIP}/CDN/PC/{gameVersion}"
			};
#else
			return Application.platform switch {
				RuntimePlatform.Android => $"{hostServerIP}/CDN/Android/{gameVersion}",
				RuntimePlatform.IPhonePlayer => $"{hostServerIP}/CDN/IPhone/{gameVersion}",
				RuntimePlatform.WebGLPlayer => $"{hostServerIP}/CDN/WebGL/{gameVersion}",
				_ => $"{hostServerIP}/CDN/PC/{gameVersion}"
			};
#endif
		}
	}
}