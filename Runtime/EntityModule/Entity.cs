using System;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public abstract class Entity : MonoBehaviour {
        public int ID { get; private set; }
        public string AssetName { get; private set; }
        public IEntityGroup Group { get; private set; }
        
        public virtual ValueType Data { get; set; }

        protected EntityFactory Factory;


        internal void Init(int entityID, string entityAssetName, IEntityGroup entityGroup = null, ValueType data = null) {
            ID = entityID;
            AssetName = entityAssetName;
            Group = entityGroup;
            Data = data;
            OnInit();
        }
        protected virtual void OnInit() {
            QuickLogger.LogError($"QuickGameFramework>Entity>Error>{ID}-{AssetName} 该实体未实现OnInit");
        }
        
        public void Recycle() {
            Factory.RecycleEntity(this);
        }

        public virtual void EntityUpdate(float logicTimeSpan, float realTimeSpan) { }
    }
}