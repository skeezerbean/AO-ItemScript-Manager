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
	public class ScriptMenusViewModel : Screen
	{
		private DateTime lastUpdate = DateTime.Now;
		private string MenuFolder { get { return $"{GeneralSettingsManager.GeneralSettings.GameFolderLocation}\\cd_image\\text\\help"; } }

		// IDialogCoordinator is for metro message boxes
		private readonly IDialogCoordinator _dialogCoordinator;

		public BindableCollection<Script> ListOfMenus { get; set; } = new BindableCollection<Script>();
		public Script SelectedMenu { get; set; }

		// read only UI display
		public string SelectedMenuContent
		{
			get { return (SelectedMenu == null) ? string.Empty : SelectedMenu.Contents; }
			set { SelectedMenu.Contents = value; }
		}
		public FlowDocument MenuContentPreview { get { return GetContentPreview(); } }

		// IDialogCoordinator is part of Metro, for dialog handling in the view model
		public ScriptMenusViewModel(IDialogCoordinator instance)
		{
			_dialogCoordinator = instance;
			InitMenus();
		}

		private async void InitMenus()
		{
			// The app tries to run all this before loading up the settings/locations that
			// may be saved, so this will help it cycle in the background a bit and then
			// load up available items in the list after settings have been read
			while (lastUpdate.AddMilliseconds(100) > DateTime.Now)
			{
				await Task.Delay(100);
			}

			// If there are proper locations in the settings, then this should now populate
			RefreshMenus();
		}

		public void RefreshMenus()
		{
			// If we have nothing or wrong location then just return
			if (GeneralSettingsManager.GeneralSettings.GameFolderLocation == ""
				|| !Directory.Exists(MenuFolder))
				return;

			// Check our scripts folder for files/folders and gather up these into a list
			string[] fileArray = Directory.GetFiles(MenuFolder, "*.txt", SearchOption.AllDirectories);
			if (fileArray.Length == 0)
				return;

			// Prep our collection for update
			ListOfMenus.Clear();

			foreach (var script in fileArray)
			{
				Script tempScript = new Script();

				// Save our name without the full path, but including immediate folder if any
				tempScript.ShortName = script.Replace(MenuFolder, "");
				tempScript.File = script;

				try { tempScript.Contents = File.ReadAllText(script); }
				catch (Exception e) { MessageBox.Show($"Reading File Contents for {script}\nerror - {e.Message}"); }

				ListOfMenus.Add(tempScript);
			}
		}

		public void CreateMenu()
		{
			string result = string.Empty;

			string sampleMenu = "<font color=#00CC00>Sample Script Menu!\n";
			sampleMenu += "<a href=\"chatcmd://script1.txt\">First Script</a>\n";
			sampleMenu += "<a href=\"chatcmd://script2.txt\">Second Script</a>";

			if (GeneralSettingsManager.GeneralSettings.GameFolderLocation == "")
			{
				MessageBox.Show("The Game Folder Location is empty in settings, please set this location before creating a menu.");
			}
			else
			{
				try
				{
					VistaSaveFileDialog dialog = new VistaSaveFileDialog();

					dialog.Title = "Please select a file";
					dialog.DefaultExt = "txt";
					dialog.FileName = $"{MenuFolder}\\NewMenu.txt";

					if ((bool)dialog.ShowDialog())
						File.WriteAllText(dialog.FileName, sampleMenu);
					else // jump out if user canceled
						return;
				}

				catch { return; }
				RefreshMenus();
			}
		}

		// Take our selected script and parse into a FlowDocument
		public FlowDocument GetContentPreview()
		{
			if (SelectedMenu == null)
				return new FlowDocument();

			return Parsing.TryParsingMenu(SelectedMenuContent);
		}

		// Flush our selected script action list to disk, then refresh
		public void SaveMenu()
		{
			if (SelectedMenu == null)
				return;

			// Take our selected script and flush it back to disk
			try { File.WriteAllText(SelectedMenu.File, SelectedMenu.Contents); }
			catch (Exception e) { MessageBox.Show($"Writing File {SelectedMenu.File},\nerror {e.Message}"); }

			RefreshMenus();
		}
	}
}
