using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;

namespace AO_ItemScript_Manager
{
	public class SettingsViewModel : Screen
	{
		// IDialogCoordinator is for metro message boxes
		private readonly IDialogCoordinator _dialogCoordinator;

		public string ScriptFolderLocation { get { return GeneralSettingsManager.GeneralSettings.ScriptFolderLocation; } set { GeneralSettingsManager.GeneralSettings.ScriptFolderLocation = value; } }
		public string GameFolderLocation { get { return GeneralSettingsManager.GeneralSettings.GameFolderLocation; } set { GeneralSettingsManager.GeneralSettings.GameFolderLocation = value; } }

		// IDialogCoordinator is part of Metro, for dialog handling in the view model
		public SettingsViewModel(IDialogCoordinator instance)
		{
			_dialogCoordinator = instance;
		}

		// From hitting the SPP Browse button in settings tab
		public void ScriptFolderBrowse()
		{
			// Start in our current user\AppData\Local folder
			string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			baseFolder += "\\Funcom\\Anarchy Online";

			// See if we can find a single scripts folder, but if there are multiple installations
			// then the user will need to choose which to manage
			string[] d = Directory.GetDirectories(baseFolder, "scripts", SearchOption.AllDirectories);

			// means we found only 1 match, otherwise it will default to AppData\Local\Funcom\Anarchy Online
			// to choose a specific folder
			if (d.Length == 1)
				baseFolder = d[0];

			// If it's empty, then it was cancelled and we keep the old setting
			string tmp = BrowseFolder(baseFolder);
			if (tmp != string.Empty)
			{
				ScriptFolderLocation = tmp;
				GeneralSettingsManager.SaveSettings(GeneralSettingsManager.SettingsPath, GeneralSettingsManager.GeneralSettings);
			}
		}

		// From hitting the SPP Browse button in settings tab
		public void GameFolderBrowse()
		{
			// If it's empty, then it was cancelled and we keep the old setting
			string tmp = BrowseFolder(@"C:\");
			if (tmp != string.Empty)
			{
				GameFolderLocation = tmp;
				GeneralSettingsManager.SaveSettings(GeneralSettingsManager.SettingsPath, GeneralSettingsManager.GeneralSettings);
			}
		}

		// Method to browse to a folder
		public string BrowseFolder(string baseFolder)
		{
			string result = string.Empty;
			try
			{
				VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
				dialog.Description = "Please select a folder.";
				dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.
				dialog.SelectedPath = baseFolder; // place to start search
				if ((bool)dialog.ShowDialog())
					result = dialog.SelectedPath;
			}
			catch { return string.Empty; }

			return result;
		}
	}
}