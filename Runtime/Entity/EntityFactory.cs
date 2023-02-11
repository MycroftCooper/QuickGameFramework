using System;

namespace QuickGameFramework.Runtime {
    public interface IEntityFactory<out T>where T:Entity {
        public void PreLoadAsset(Action callBack = null);
        public void Init();
        public T CreateEntity(string entityID, object data = null);
        public void RecycleEntity(Entity entity);
    }
}