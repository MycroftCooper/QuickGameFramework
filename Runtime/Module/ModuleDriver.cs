using UnityEngine;

namespace QuickGameFramework.Runtime {
	internal class ModuleDriver : MonoBehaviour {
		void Update() {
			ModuleManager.Update(Time.deltaTime);
		}
	}
}