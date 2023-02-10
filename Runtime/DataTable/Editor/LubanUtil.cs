using System.Diagnostics;
using System.IO;
using UnityEditor;

#if UNITY_EDITOR
public static class LubanUtil { 
    [MenuItem("QuickResources/LubanUtil/ImportExcel")] 
    public static void ImportExcel() { 
        CleanJson(); 
        Process proc = new Process();//new 一个Process对象 
        string targetDir = (@"Luban/");//文件目录 
 
        proc.StartInfo.WorkingDirectory = targetDir; 
        proc.StartInfo.FileName = "gen_code_json.bat";//文件名字 
 
        proc.Start(); 
        proc.WaitForExit(); 
    } 
    
    [MenuItem("QuickResources/LubanUtil/CheckExcel")] 
    public static void CheckExcel() {
        Process proc = new Process();//new 一个Process对象 
        string targetDir = (@"Luban/");//文件目录 
 
        proc.StartInfo.WorkingDirectory = targetDir; 
        proc.StartInfo.FileName = "check.bat";//文件名字 
 
        proc.Start(); 
        proc.WaitForExit(); 
    } 
    
    [MenuItem("QuickResources/LubanUtil/CleanJson")] 
    public static void CleanJson() { 
        DirectoryInfo dir = new DirectoryInfo("Assets/GameMain/DataTables/Json/");
        if (!dir.Exists) return;
        DirectoryInfo[] children = dir.GetDirectories(); 
        foreach (DirectoryInfo child in children) { 
            child.Delete(true); 
        } 
        dir.Delete(true);
    } 
} 
#endif