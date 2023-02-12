using System;
using System.Collections.Generic;
using System.Linq;
using YooAsset;

namespace QuickGameFramework.Runtime {
    public class AssetLoadProgress {
        public float Progress {
            get {
                if (Handles == null || Handles.Count == 0) {
                    return 0;
                }

                float totalProgress = Handles.Sum(assetOperationHandle => assetOperationHandle.Progress);
                return totalProgress / Handles.Count;
            }
        }
        public bool IsComplete => _completeHandleNum == Handles.Count;
        public Action Completed;
        
        public List<AssetOperationHandle> Handles { get; }
        private int _completeHandleNum;

        public AssetLoadProgress() {
            Handles = new List<AssetOperationHandle>();
            _completeHandleNum = 0;
        }

        public static AssetLoadProgress operator +(AssetLoadProgress a, AssetLoadProgress b) {
            a.AddHandles(b.Handles);
            b.Destroy();
            return a;
        }
        
        public static AssetLoadProgress operator +(AssetLoadProgress a, AssetOperationHandle b) {
            a.AddHandle(b);
            return a;
        }

        public void AddHandles(List<AssetOperationHandle> handles) {
            if (handles == null || handles.Count == 0) {
                QLog.Error("QuickGameFramework>Asset> handle为空，加入加载队列失败！");
                return;
            }
            handles.ForEach(AddHandle);
        }

        public void AddHandle(AssetOperationHandle handle) {
            if (handle == null) {
                QLog.Error("QuickGameFramework>Asset> handle为空，加入加载队列失败！");
                return;
            }
            if (Handles.Contains(handle)) {
                QLog.Error("QuickGameFramework>Asset> 该handle已存在，加入加载队列失败！");
                return;
            }
            Handles.Add(handle);
            handle.Completed += OnHandleComplete;
        }

        public void OnHandleComplete(AssetOperationHandle handle) {
            _completeHandleNum += 1;
            if (IsComplete) {
                Completed?.Invoke();
            }
        }

        public void Destroy() {
            Handles.ForEach(_=>_.Completed -= OnHandleComplete);
            Handles.Clear();
            Completed = null;
        }
    }
}