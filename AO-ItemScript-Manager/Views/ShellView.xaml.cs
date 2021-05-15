using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;

namespace AO_ItemScript_Manager
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView : MetroWindow
	{
		public ShellView()
		{
			InitializeComponent();
		}

		private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			GeneralSettingsManager.SaveSettings(GeneralSettingsManager.SettingsPath, GeneralSettingsManager.GeneralSettings);
		}

		private void HelpAbout(object sender, RoutedEventArgs e)
		{
			// for .NET Core you need to add UseShellExecute = true
			// see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
			Process.Start(new ProcessStartInfo("https://github.com/skeezerbean/AO-ItemScript-Manager/blob/main/README.md"));
			e.Handled = true;
		}
	}
}
