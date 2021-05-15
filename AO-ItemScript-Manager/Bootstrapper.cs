using Caliburn.Micro;
using System.Windows;

namespace AO_ItemScript_Manager
{
	public class Bootstrapper : BootstrapperBase
	{

		public Bootstrapper()
		{
			Initialize();
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			DisplayRootViewFor<ShellViewModel>();
		}
	}
}
