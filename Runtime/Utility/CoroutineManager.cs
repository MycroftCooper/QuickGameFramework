using System.Collections;
using UnityEngine;

namespace QuickGameFramework.Runtime {
    public class CoroutineManager : MonoSingleton<CoroutineManager> {
        /// <summary>
        /// 开启一个协程
        /// </summary>
        public Coroutine StartQuickCoroutine(IEnumerator coroutine) {
            return StartCoroutine(coroutine);
        }

        /// <summary>
        /// 开启一个协程
        /// </summary>
        public Coroutine StartQuickCoroutine(string methodName) {
            return StartCoroutine(methodName);
        }

        /// <summary>
        /// 停止一个协程
        /// </summary>
        public void StopQuickCoroutine(Coroutine coroutine) {
            StopCoroutine(coroutine);
        }

        /// <summary>
        /// 停止一个协程
        /// </summary>
        public void StopQuickCoroutine(string methodName) {
            StopCoroutine(methodName);
        }

        /// <summary>
        /// 停止所有协程
        /// </summary>
        public void StopAllQuickCoroutines() {
            StopAllCoroutines();
        }
    }
}