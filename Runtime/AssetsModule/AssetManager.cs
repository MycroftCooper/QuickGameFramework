using System;
using System.Collections;
using MycroftToolkit.QuickCode;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace QuickGameFramework.Runtime {
	public class AssetManager : Singleton<AssetManager> {
		private ProjectAssetSetting _projectAssetSetting;
		private AssetsPackage _defaultPackage;

		public void Init() {
			_projectAssetSetting = Resources.Load<ProjectAssetSetting>("ProjectAssetSetting");
			if (_projectAssetSetting == null) {
				QLog.Error($"QuickGameFramework>Asset>项目资源设置缺失！加载失败!\n"+
				              "请在<Resources目录>下增加<ProjectAssetSetting>\n");
				return;
			}

			ModuleManager.StartCoroutine(InitPackage());
		}

		public void LoadAssetAsync<T>(string path, Action<AssetOperationHandle> callback ,string packageName = null) where T : Object {
			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				return;
			}
			AssetOperationHandle handle = package.LoadAssetAsync<T>(path);
			handle.Completed += callback;
		}
		
		public T LoadAssetSync<T>(string path, string packageName = null) where T : Object {
			if (!GetAssetPackage(packageName, out AssetsPackage package)) {
				return default;
			}
			return (T) package.LoadAssetSync<T>(path).AssetObject;
		}

		public void ReleaseAsset() {
			
		}

		public void UnloadAssets() {
			
		}

		private bool GetAssetPackage(string packageName, out AssetsPackage package) {
			package = string.IsNullOrEmpty(packageName) ? _defaultPackage : YooAssets.TryGetAssetsPackage(packageName);
			if (package != null) return true;
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
			_defaultPackage = package;

			InitializationOperation initializationOperation = null;
			switch (playMode) {
				// 编辑器下的模拟模式
				case EPlayMode.EditorSimulateMode: {
					var createParameters = new EditorSimulateModeParameters();
					createParameters.SimulatePatchManifestPath =
						EditorSimulateModeHelper.SimulateBuild(defaultPackageName);
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
					var createParameters = new HostPlayModeParameters();
					// createParameters.DecryptionServices 可提供资源包加密类
					// createParameters.QueryServices = new GameQueryServices(); 内置文件查询服务类
					createParameters.DefaultHostServer = GetHostServerURL(true);
					createParameters.FallbackHostServer = GetHostServerURL(false);
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