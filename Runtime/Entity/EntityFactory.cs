using System;

namespace QuickGameFramework.Runtime {
    public abstract class EntityFactory {
        public abstract Entity CreateEntity(int entityID, string entityAssetName, IEntityGroup entityGroup = null, ValueType data = null);
        public abstract void RecycleEntity(Entity entity);
    }
}