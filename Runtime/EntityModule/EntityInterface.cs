using System;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public interface IEntityGroup {
        /// <summary>
        /// 获取实体组名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 获取实体组中实体数量。
        /// </summary>
        public int EntityCount { get; }
        
        public Transform GroupRoot { get; }

        /// <summary>
        /// 实体组中是否存在实体。
        /// </summary>
        /// <param name="entityId">实体序列编号。</param>
        /// <returns>实体组中是否存在实体。</returns>
        public bool HasEntity(int entityId);

        /// <summary>
        /// 从实体组中获取实体。
        /// </summary>
        /// <param name="entityId">实体序列编号。</param>
        /// <returns>要获取的实体。</returns>
        public Entity GetEntity(int entityId);

        /// <summary>
        /// 从实体组中获取所有实体。
        /// </summary>
        /// <returns>实体组中的所有实体。</returns>
        public Entity[] GetAllEntities();

        public void UpdateEntities(float logicTimeSpan, float realTimeSpan);
    }
}