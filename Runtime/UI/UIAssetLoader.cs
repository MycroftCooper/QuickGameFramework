using System;
using System.Collections.Generic;
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
        private bool _isLoad;
        private List<AssetOperationHandle> _fguiAssetHandles;
        private string _fuiPackageName;

        private void Awake() {
            if (_isLoad) return;
            
            _fuiPackageName = gameObject.GetComponent<UIPanel>().packageName;
            _fguiAssetHandles = new List<AssetOperationHandle>();
            UIPackage.AddPackage(_fuiPackageName, LoadFunc);
            _isLoad = true;
        }

        private void OnDestroy() {
            _fguiAssetHandles.ForEach(_=>_.Release());
            GameEntry.AssetMgr.UnloadAssets();

            var package = UIPackage.GetByName(_fuiPackageName);
            if (package == null) return;
            UIPackage.RemovePackage(_fuiPackageName);
            _fguiAssetHandles.Clear();
            _isLoad = false;
        }
        
        private object LoadFunc(string packageName, string extension, Type type, out DestroyMethod method) {
            method = DestroyMethod.None;
            string location = GameEntry.ConfigMgr.ProjectAssetSetting.uiResPath + packageName;
            var assetPackage = YooAssets.GetAssetsPackage(GameEntry.ConfigMgr.ProjectAssetSetting.uiAssetsPackageName);
            var handle = assetPackage.LoadAssetSync(location, type);
            handle.Completed += AssetManager.LogLoadSuccess;
            _fguiAssetHandles.Add(handle);
            return handle.AssetObject;
        }
    }
}