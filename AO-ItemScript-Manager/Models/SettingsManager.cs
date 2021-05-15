using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;

namespace AO_ItemScript_Manager
{
	/// <summary>
	/// This class handles the general settings, save/load with files
	/// </summary>
	public static class GeneralSettingsManager
	{
		public static GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();
		public static JObject SettingsJSON { get; set; }
		public static string SettingsPath = "Settings.json";

		// If the file doesn't exist, it will resort to defaults listed in the class itself
		public static void LoadGeneralSettings()
		{
			// Let's try to pull in settings, keep default if fails
			try
			{
				var fileinfo = new FileInfo(SettingsPath);
				SettingsJSON = JObject.Parse(File.ReadAllText(SettingsPath));
				GeneralSettings = JsonConvert.DeserializeObject<GeneralSettings>(SettingsJSON.ToString());
			}
			catch
			{
				// Setting defaults...
				string jsonString = JsonConvert.SerializeObject(GeneralSettings);
				GeneralSettings = JsonConvert.DeserializeObject<GeneralSettings>(jsonString);
				SettingsJSON = (JObject)JsonConvert.DeserializeObject(jsonString);
			}
		}

		// Take an incoming class and convert to json to save to disk
		// as well as implementing indentation for human-reading
		public static bool SaveSettings(string path, object item)
		{
			try { File.WriteAllText(path, JsonConvert.SerializeObject(item, Formatting.Indented)); }
			catch { return false; }
			return true;
		}

		// If the saved window settings are out of bounds (resolution change, multiple monitors change)
		// then we move into view so that it isn't off screen somewhere
		public static void MoveIntoView()
		{
			GeneralSettingsManager.GeneralSettings.WindowWidth = System.Math.Min(GeneralSettingsManager.GeneralSettings.WindowWidth, SystemParameters.VirtualScreenWidth);
			GeneralSettingsManager.GeneralSettings.WindowHeight = System.Math.Min(GeneralSettingsManager.GeneralSettings.WindowHeight, SystemParameters.VirtualScreenHeight);
			GeneralSettingsManager.GeneralSettings.WindowLeft = System.Math.Min(System.Math.Max(GeneralSettingsManager.GeneralSettings.WindowLeft, SystemParameters.VirtualScreenLeft),
				SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth - GeneralSettingsManager.GeneralSettings.WindowWidth);
			GeneralSettingsManager.GeneralSettings.WindowTop = System.Math.Min(System.Math.Max(GeneralSettingsManager.GeneralSettings.WindowTop, SystemParameters.VirtualScreenTop),
				SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight - GeneralSettingsManager.GeneralSettings.WindowHeight);
		}
	}

	/// <summary>
	/// This class stores the overall general settings for the app
	/// </summary>
	public class GeneralSettings
	{
		public string ScriptFolderLocation { get; set; } = "";
		public string GameFolderLocation { get; set; } = "";
		public double WindowTop { get; set; } = 100;
		public double WindowLeft { get; set; } = 100;
		public double WindowHeight { get; set; } = 500;
		public double WindowWidth { get; set; } = 800;

		public GeneralSettings()
		{
		}
	}
}