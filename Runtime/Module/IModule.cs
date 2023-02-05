using System.Collections.Generic;

namespace QuickGameFramework.Runtime {
	public interface IModule {
		public int Priority { get; set; }
		public bool IsFrameworkModule { get; }
		
		/// <summary>
		/// 创建模块
		/// </summary>
		public void OnModuleCreate(params object[] createParam);

		/// <summary>
		/// 更新模块
		/// </summary>
		public void OnModuleUpdate(float intervalSeconds);

		/// <summary>
		/// 销毁模块
		/// </summary>
		public void OnModuleDestroy();
	}
	
	public class ModuleComparer : IComparer<IModule> {
		public int Compare(IModule x, IModule y) {
			return -(x!.Priority - y!.Priority);
		}
	}
}