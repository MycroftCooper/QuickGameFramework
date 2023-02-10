using SimpleJSON;
using System.IO;
using QuickGameFramework.Runtime;
using UnityEngine;

namespace OasisProject3D {
    public class DataTableManager : Singleton<DataTableManager> {
        private const string JsonFilePath = "Assets/GameMain/DataTables/Json/";
        public cfg.Tables Tables;
        public DataTableManager() {
            Tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText(JsonFilePath + "/" + file + ".json")));
        }

        public cfg.Tables LoadTable(string tableName) {
            string pathName = JsonFilePath + "/" + tableName + ".json";
            if (File.Exists(pathName)) {
                return new cfg.Tables(file => JSON.Parse(File.ReadAllText(JsonFilePath + "/" + tableName + ".json")));
            }
            Debug.LogError($"DataTableManager>Error> 该文件不存在:{pathName}");
            return null;
        }
    }
}
