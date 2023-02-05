using System;
using System.Collections.Generic;
using YooAsset;

namespace QuickGameFramework.Runtime {
    public abstract class EntityFactory {
        private Dictionary<int, List<AssetOperationHandle>> _entityAssetDict;
        public abstract Entity CreateEntity(int entityID, string entityAssetName, ValueType data = null);
        public abstract void RecycleEntity(Entity entity);
    }
}