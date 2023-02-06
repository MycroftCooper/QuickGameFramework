using QuickGameFramework.Procedure;
using QuickGameFramework.Runtime;

public class GameEntry : MonoSingleton<GameEntry> {
    public static ModuleManager ModuleManager;
    public static ProcedureManager ProcedureManager;
    public static CoroutineManager CoroutineManager;
    public static AssetManager AssetManager;
    void Awake() {
        IsGlobal = true;
        ModuleManager = transform.Find("ModuleManager").GetComponent<ModuleManager>();
        CoroutineManager = transform.Find("CoroutineManager").GetComponent<CoroutineManager>();
        ProcedureManager = ModuleManager.CreateModule<ProcedureManager>();
        AssetManager = new AssetManager();
    }
}
