using System;
using System.Collections.Generic;
using YooAsset;

namespace QuickGameFramework.Runtime {
    public interface IEntityFactory<out T> where T : Entity {
        public List<AssetOperationHandle> PreLoadAsset(Action callBack = null);
        public void Init();
        public T CreateEntity(string entityID, object data = null);
        public void RecycleEntity(Entity entity);
    }
}