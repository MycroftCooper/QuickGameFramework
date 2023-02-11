using System.Collections.Generic;
using System.Linq;
using YooAsset;

public class AssetLoadQueue {
    private List<AssetOperationHandle> _handles;
    private int _completeHandleNum;

    public float Progress {
        get {
            if (_handles == null || _handles.Count == 0) {
                return 0;
            }
            float totalProgress = _handles.Sum(assetOperationHandle => assetOperationHandle.Progress);
            return totalProgress / _handles.Count;
        }
    }
    
    
}
