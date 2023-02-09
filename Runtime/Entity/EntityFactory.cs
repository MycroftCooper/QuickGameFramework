using System;

namespace QuickGameFramework.Runtime {
    public interface IEntityFactory {
        public void PreLoadAsset();
        public void Init();
        public Entity CreateEntity(int entityID, string entityAssetName, ValueType data = null);
        public void RecycleEntity(Entity entity);
    }
}