using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickGameFramework.Runtime {
    public class Singleton<T> where T : Singleton<T>, new() {
        private static T _instance;
        private static readonly object Obj = new object();
        public static T Instance {
            get {
                lock (Obj)
                {
                    return _instance ??= new T();
                }
            }
        }
    }

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        private static readonly object Lock = new object();
        /// <summary>
        /// 程序是否正在退出
        /// </summary>
        protected static bool ApplicationIsQuitting { get; private set; }
        /// <summary>
        /// 是否为全局单例
        /// </summary>
        private static bool _dontDestroy;
        protected static bool IsGlobal {
            get => _dontDestroy;
            set {
                if (value == _dontDestroy || !Application.isPlaying) return;
                if (value)
                    DontDestroyOnLoad(Instance.gameObject);
                else
                    SceneManager.MoveGameObjectToScene(Instance.gameObject, SceneManager.GetActiveScene());
                _dontDestroy = value;
            }
        }

        static MonoSingleton() {
            ApplicationIsQuitting = false;
        }

        public static T Instance {
            get {
                if (ApplicationIsQuitting) {
                    if (Debug.isDebugBuild) {
                        QLog.Error($"QuickGameFramework>Singleton>已进入退出游戏过程，单例<{typeof(T)}>创建失败！");
                    }
                    return null;
                }

                lock (Lock) {
                    if (_instance != null) return _instance;
                    
                    // 先在场景中找寻
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1) {
                        if (Debug.isDebugBuild) {
                            QLog.Error($"QuickGameFramework>Singleton> 单例 {typeof(T).Name}创建失败，该单例已存在，一个场景不应当有多个相同单例！");
                        }
                        return _instance;
                    }

                    // 场景中找不到就创建新物体挂载
                    if (_instance != null) return _instance;
                    
                    GameObject singletonObj = new GameObject();
                    _instance = singletonObj.AddComponent<T>();
                    singletonObj.name = typeof(T).Name;

                    if (IsGlobal && Application.isPlaying) {
                        DontDestroyOnLoad(singletonObj);
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// 当工程运行结束，在退出时，不允许访问单例
        /// </summary>
        public void OnApplicationQuit() {
            ApplicationIsQuitting = true;
        }

        private void OnDestroy() {
            _instance = null;
        }
    }
}
