
namespace QuickGameFramework.Runtime {
	public interface IModule {
		/// <summary>
		/// 创建模块
		/// </summary>
		public void OnModuleCreate(params object[] createParam);

		/// <summary>
		/// 更新模块
		/// </summary>
		public void OnModuleUpdate();

		/// <summary>
		/// 销毁模块
		/// </summary>
		public void OnModuleDestroy();
	}
}