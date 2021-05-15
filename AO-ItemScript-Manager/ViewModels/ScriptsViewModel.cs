using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Ookii.Dialogs.Wpf;

namespace AO_ItemScript_Manager
{
	public class ScriptsViewModel : Screen
	{
		private DateTime lastUpdate = DateTime.Now;

		// IDialogCoordinator is for metro message boxes
		private readonly IDialogCoordinator _dialogCoordinator;

		public BindableCollection<Script> ListOfScripts { get; set; } = new BindableCollection<Script>();
		public Script SelectedScript { get; set; }
		public string ColorTip { get { return "Tip - Yellow Title = \"#dede42\"  Dark Green = \"#63ad63\"  Light Green = \"#00de42\""; } }

		// read only UI display
		public string SelectedScriptContent
		{
			get { return (SelectedScript == null) ? string.Empty : SelectedScript.Contents; }
			set { SelectedScript.Contents = value; }
		}
		public FlowDocument ScriptContentPreview { get { return GetContentPreview(); } }

		// IDialogCoordinator is part of Metro, for dialog handling in the view model
		public ScriptsViewModel(IDialogCoordinator instance)
		{
			_dialogCoordinator = instance;
			InitScripts();
		}

		private async void InitScripts()
		{
			// The app tries to run all this before loading up the settings/locations that
			// may be saved, so this will help it cycle in the background a bit and then
			// load up available items in the list after settings have been read
			while (lastUpdate.AddMilliseconds(100) > DateTime.Now)
			{
				await Task.Delay(100);
			}

			// If there are proper locations in the settings, then this should now populate
			RefreshScripts();
		}

		public void RefreshScripts()
		{
			// If we have nothing or wrong location then just return
			if (GeneralSettingsManager.GeneralSettings.ScriptFolderLocation == ""
				|| !GeneralSettingsManager.GeneralSettings.ScriptFolderLocation.EndsWith("\\scripts"))
				return;

			// Check our scripts folder for files/folders and gather up these into a list
			string[] fileArray = Directory.GetFiles(GeneralSettingsManager.GeneralSettings.ScriptFolderLocation, "*.*", SearchOption.AllDirectories);
			if (fileArray.Length == 0)
				return;

			// Prep our collection for update
			ListOfScripts.Clear();

			foreach (var script in fileArray)
			{
				Script tempScript = new Script();

				// Save our name without the full path, but including immediate folder if any
				tempScript.ShortName = script.Replace(GeneralSettingsManager.GeneralSettings.ScriptFolderLocation, "");
				tempScript.File = script;

				try { tempScript.Contents = File.ReadAllText(script); }
				catch (Exception e) { MessageBox.Show($"Reading File Contents for {script}\nerror - {e.Message}"); }

				ListOfScripts.Add(tempScript);
			}
		}

		public void CreateScript()
		{
			string result = string.Empty;
			string sampleScript = "<a href=\"text://<font color=#dede42>Sample Item<br><font color=#63ad63>NODROP UNIQUE<br><font color=#00de42>Quality Level: <font color=#63ad63>Special<br><font color=#00de42>Requirements:<br>";
			sampleScript += "<font color=#00de42>Wear:<br><font color=#63ad63>  Stamina from 575<br><font color=#00de42>Loc: <font color=#63ad63>Back<br><br><font color=#00de42>Description:<br><font color=#63ad63>";
			sampleScript += "  This is the description.\">Sample Item</a>";

			if (GeneralSettingsManager.GeneralSettings.ScriptFolderLocation == "")
			{
				MessageBox.Show("The Script Folder Location is empty in settings, please set this location before creating a script.");
			}
			else
			{
				try
				{
					VistaSaveFileDialog dialog = new VistaSaveFileDialog();

					dialog.Title = "Please select a file";
					dialog.DefaultExt = "txt";
					dialog.FileName = $"{GeneralSettingsManager.GeneralSettings.ScriptFolderLocation}\\NewScript.txt";

					if ((bool)dialog.ShowDialog())
						File.WriteAllText(dialog.FileName, sampleScript);
					else // jump out if user canceled
						return;
				}

				catch { return; }
				RefreshScripts();
			}
		}

		// Take our selected script and parse into a FlowDocument
		public FlowDocument GetContentPreview()
		{
			if (SelectedScript == null)
				return new FlowDocument();

			return Parsing.TryParsingScript(SelectedScriptContent);
		}

		// Flush our selected script action list to disk, then refresh
		public void SaveScripts()
		{
			if (SelectedScript == null)
				return;

			// Take our selected script and flush it back to disk
			try { File.WriteAllText(SelectedScript.File, SelectedScript.Contents); }
			catch (Exception e) { MessageBox.Show($"Writing File {SelectedScript.File},\nerror {e.Message}"); }

			RefreshScripts();
		}
	}
}
