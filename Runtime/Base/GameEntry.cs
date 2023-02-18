using System;
using QuickGameFramework.Procedure;

namespace QuickGameFramework.Runtime {
    public class GameEntry : MonoSingleton<GameEntry> {
        public static ModuleManager ModuleMgr;
        public static ProcedureManager ProcedureMgr;
        public static CoroutineManager CoroutineMgr;
        public static DataTableManager DataTableMgr;
        public static AssetManager AssetMgr;
        public static UIManager UIMgr;
        public static ConfigManager ConfigMgr;

        private void Awake() {
            IsGlobal = true;
            
            CoroutineMgr = GetComponent<CoroutineManager>();
            ConfigMgr = new ConfigManager();
            DataTableMgr = new DataTableManager();
            ModuleMgr = GetComponent<ModuleManager>();
            ProcedureMgr = ModuleMgr.CreateModule<ProcedureManager>();
            
            AssetMgr = new AssetManager();
            AssetMgr.Init(() => {
                ProcedureMgr.StartProcedure(Type.GetType("EnterGameProcedure"));
                UIMgr = new UIManager();
            });
        }
    }
}