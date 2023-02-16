using System;
using System.Collections.Generic;
using FairyGUI;
using MycroftToolkit.QuickCode;
using UnityEngine;
using YooAsset;

namespace QuickGameFramework.Runtime {
    public class UIManager {
        private readonly ProjectAssetSetting _projectAssetSetting;
        private readonly Dictionary<string, AssetOperationHandle> _handleDict;

        public UIManager() {
            _projectAssetSetting = Resources.Load<ProjectAssetSetting>("ProjectAssetSetting");
            _handleDict = new Dictionary<string, AssetOperationHandle>();
        }

        #region UI资源加载相关
        public bool HasLoadPackage(string uiPackageName) {
            return _handleDict.ContainsKey(uiPackageName);
        }

        public void LoadPackage(string uiPackageName) {
            if (_handleDict.ContainsKey(uiPackageName)) {
                QLog.Error($"QuickGameFramework>UIManager>FUI包[{uiPackageName}]加载失败！ 该FUI包已加载，请勿重复加载！");
                return;
            }
            UIPackage.AddPackage(uiPackageName, LoadFunc);
        }

        public void ReleasePackage(string uiPackageName) {
            if (_handleDict.ContainsKey(uiPackageName)) {
                QLog.Error($"QuickGameFramework>UIManager>FUI包[{uiPackageName}]释放失败！ 该FUI包未加载！");
                return;
            }
            UIPackage.RemovePackage(uiPackageName);
            _handleDict[uiPackageName].Release();
            _handleDict.Remove(uiPackageName);
        }

        public void ReleaseAllPackage() {
            _handleDict.ForEach(_ => {
                UIPackage.RemovePackage(_.Key);
                _.Value.Release();
            });
            _handleDict.Clear();
        }

        // FairyGUI对接YooAssets加载UI包的方法
        private object LoadFunc(string name, string extension, Type type, out DestroyMethod method) {
            method = DestroyMethod.None;
            string location = _projectAssetSetting.uiResPath + name;
            var assetPackage = YooAssets.GetAssetsPackage(_projectAssetSetting.uiAssetsPackageName);
            var handle = assetPackage.LoadAssetSync(location, type);
            _handleDict.Add(name, handle);
            return handle.AssetObject;
        }
        #endregion

        public GComponent CreateFUIComponent(string pkgName, string componentName) {
            GComponent view = UIPackage.CreateObject("","").asCom;
            return view;
        }
        
    }
}