using System;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public abstract class Entity : MonoBehaviour {
        public string ID { get; private set; }

        public virtual ValueType Data { get; set; }

        private IEntityFactory<Entity> _factory;


        public virtual void Init(string entityID, IEntityFactory<Entity> factory, ValueType data = null) {
            ID = entityID;
            name = ID;
            Data = data;
            _factory = factory;
        }

        public void Recycle() {
            _factory.RecycleEntity(this);
        }

        public virtual void EntityUpdate(float logicTimeSpan, float realTimeSpan) { }
    }
}