using System;
using SimpleJSON;
using System.IO;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public class DataTableManager {
        private const string JsonFilePath = "Assets/GameMain/DataTables/Json/";
        public cfg.Tables Tables;
        public DataTableManager() {
            try {
                Tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText(JsonFilePath + "/" + file + ".json")));
                QLog.Log("QuickGameFramework>DataTableManager>配表系统初始化成功!");
            }catch (Exception e) {
                QLog.Log($"QuickGameFramework>DataTableManager>配表系统初始化失败!\n {e.Message}");
            }
        }

        public cfg.Tables LoadTable(string tableName) {
            string pathName = JsonFilePath + "/" + tableName + ".json";
            if (File.Exists(pathName)) {
                return new cfg.Tables(_ => JSON.Parse(File.ReadAllText(JsonFilePath + "/" + tableName + ".json")));
            }
            Debug.LogError($"QuickGameFramework>DataTableManager>Error> 该文件不存在:{pathName}");
            return null;
        }
    }
}
