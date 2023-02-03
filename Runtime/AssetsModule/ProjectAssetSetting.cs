using UnityEngine;
using YooAsset;

namespace QuickGameFramework.Runtime {
    [CreateAssetMenu(fileName = "ProjectAssetSetting", menuName = "YooAsset/Create Project Asset Setting")]
    public class ProjectAssetSetting : ScriptableObject {
        public EPlayMode playMode;
        public string defaultPackageName = "DefaultPackage";
        public string gameVersion = "v1.0";

        public string hostServerIP;
        public string backupHostServerIP;
    }
}