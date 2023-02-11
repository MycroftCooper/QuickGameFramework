using System;
using QuickGameFramework.Procedure;

namespace QuickGameFramework.Runtime {
    public class GameEntry : MonoSingleton<GameEntry> {
        public static ModuleManager ModuleMgr;
        public static ProcedureManager ProcedureMgr;
        public static CoroutineManager CoroutineMgr;
        public static DataTableManager DataTableMgr;
        public static AssetManager AssetMgr;

        void Awake() {
            IsGlobal = true;
            
            CoroutineMgr = GetComponent<CoroutineManager>();
            DataTableMgr = new DataTableManager();
            ModuleMgr = GetComponent<ModuleManager>();
            ProcedureMgr = ModuleMgr.CreateModule<ProcedureManager>();
            
            AssetMgr = new AssetManager();
            AssetMgr.Init(() => {
                ProcedureMgr.StartProcedure(Type.GetType("EnterGameProcedure"));
            });
        }
    }
}