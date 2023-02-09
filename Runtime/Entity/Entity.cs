using System;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public abstract class Entity : MonoBehaviour {
        public int ID { get; private set; }
        public string AssetName { get; private set; }

        public virtual ValueType Data { get; set; }

        private IEntityFactory _factory;


        internal void Init(int entityID, string entityAssetName,IEntityFactory factory, ValueType data = null) {
            ID = entityID;
            AssetName = entityAssetName;
            Data = data;
            _factory = factory;
            OnInit();
        }
        protected virtual void OnInit() {
            QLog.Error($"QuickGameFramework>Entity>Error>{ID}-{AssetName} 该实体未实现OnInit");
        }
        
        public void Recycle() {
            _factory.RecycleEntity(this);
        }

        public virtual void EntityUpdate(float logicTimeSpan, float realTimeSpan) { }
    }
}