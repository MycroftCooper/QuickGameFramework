using System;
using FairyGUI;
using UnityEngine;
using YooAsset;

namespace QuickGameFramework.Runtime {
    /// <summary>
    /// UI资源加载组件
    /// 如果UI的资源不大，可以短时间内加载完，不需要预加载，就只需要挂这个脚本
    /// 如果UI的资源很大，加载会卡顿，需要预加载，就挂这个脚本然后在预加载流程里调UIManager的Preload,并记得释放
    /// </summary>
    public class UIAssetLoader : MonoBehaviour {
        private AssetOperationHandle _fguiAssetHandle;
        private string _fuiPackageName;

        private void Awake() {
            _fuiPackageName = gameObject.GetComponent<UIPanel>().packageName;
            UIPackage.AddPackage(_fuiPackageName, LoadFunc);
        }

        private void OnDestroy() {
            _fguiAssetHandle.Release();
            GameEntry.AssetMgr.UnloadAssets();
            if (!_fguiAssetHandle.IsValid) {
                UIPackage.RemovePackage(_fuiPackageName);
            }
        }
        
        private object LoadFunc(string packageName, string extension, Type type, out DestroyMethod method) {
            method = DestroyMethod.None;
            string location = GameEntry.ConfigMgr.ProjectAssetSetting.uiResPath + packageName;
            var assetPackage = YooAssets.GetAssetsPackage(GameEntry.ConfigMgr.ProjectAssetSetting.uiAssetsPackageName);
            var handle = assetPackage.LoadAssetSync(location, type);
            _fguiAssetHandle = handle;
            return handle.AssetObject;
        }
    }
}