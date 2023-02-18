using FairyGUI;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public static class FguiExtensions {
        public static GObject GameObjectToGObject(this GameObject gameObject) {
            DisplayObjectInfo info = gameObject.GetComponent<DisplayObjectInfo>();
            if (info == null) {
                return null;
            }
            GObject obj = GRoot.inst.DisplayObjectToGObject(info.displayObject);
            return obj;
        }

        public static GameObject GObjectToGameObject(this GObject gObject) {
            return gObject.displayObject.gameObject;
        }
    }
}