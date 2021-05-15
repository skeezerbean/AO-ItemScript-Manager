using MahApps.Metro.Controls.Dialogs;
using System;
using System.Reflection;

namespace AO_ItemScript_Manager
{
	public class ShellViewModel : BaseViewModel
	{
		// Setup our public variables and such, many are saved within the general settings class, so we'll get/set from those
		public string AppTitle { get; set; } = $"AO Item/Script Manager v{Assembly.GetExecutingAssembly().GetName().Version}";

		// declare individual VMs, lets us always show the same one as we switch tabs
		public ScriptsViewModel ScriptsVM = new ScriptsViewModel(DialogCoordinator.Instance);
		public ScriptMenusViewModel ScriptMenusVM = new ScriptMenusViewModel(DialogCoordinator.Instance);
		public SettingsViewModel SettingsVM = new SettingsViewModel(DialogCoordinator.Instance);

		// This holds the values for the window position/size to be pulled from saved settings
		public double WindowTop { get { return GeneralSettingsManager.GeneralSettings.WindowTop; } set { GeneralSettingsManager.GeneralSettings.WindowTop = value; } }
		public double WindowLeft { get { return GeneralSettingsManager.GeneralSettings.WindowLeft; } set { GeneralSettingsManager.GeneralSettings.WindowLeft = value; } }
		public double WindowHeight { get { return GeneralSettingsManager.GeneralSettings.WindowHeight; } set { GeneralSettingsManager.GeneralSettings.WindowHeight = value; } }
		public double WindowWidth { get { return GeneralSettingsManager.GeneralSettings.WindowWidth; } set { GeneralSettingsManager.GeneralSettings.WindowWidth = value; } }

		public ShellViewModel()
		{
			GeneralSettingsManager.LoadGeneralSettings();
			GeneralSettingsManager.MoveIntoView();
			LoadPageScripts();
		}

		public void LoadPageScriptMenus()
		{
			ActivateItem(ScriptMenusVM);
		}

		public void LoadPageScripts()
		{
			ActivateItem(ScriptsVM);
		}

		public void LoadPageSettings()
		{
			ActivateItem(SettingsVM);
		}
	}
}