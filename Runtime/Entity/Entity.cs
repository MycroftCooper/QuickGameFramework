using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public class Entity : MonoBehaviour {
        public int ID { get; private set; }
        public string AssetName { get; private set; }
        public EntityLogic Logic { get; private set; }

        public void Init(int entityID, string entityAssetName) {
            ID = entityID;
            AssetName = entityAssetName;
            Logic = GetComponent<EntityLogic>();
            
            try {
                Logic.OnInit();
            }
            catch (Exception exception) {
                QuickLogger.LogError($"Entity {ID}-{AssetName} OnInit with exception {exception}");
            }
        }

        public void Recycle() {
            try {
                Logic.OnRecycle();
            }
            catch (Exception exception) {
                QuickLogger.LogError($"Entity {ID}-{AssetName} OnRecycle with exception {exception}");
            }
        }

        public void EntityUpdate() {
            try {
                Logic.OnUpdate();
            }
            catch (Exception exception) {
                QuickLogger.LogError($"Entity {ID}-{AssetName} OnUpdate with exception {exception}");
            }
        }
    }
}