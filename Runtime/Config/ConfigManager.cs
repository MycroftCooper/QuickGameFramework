using UnityEngine;

namespace QuickGameFramework.Runtime {
    public class ConfigManager {
        public ProjectAssetSetting ProjectAssetSetting;

        public ConfigManager() {
            ProjectAssetSetting = Resources.Load<ProjectAssetSetting>("ProjectAssetSetting");
            if (ProjectAssetSetting == null) {
                QLog.Error($"QuickGameFramework>Asset>项目资源设置缺失！加载失败!\n"+
                           "请在<Resources目录>下增加<ProjectAssetSetting>\n");
                return;
            }
            
            
        }
    }
}