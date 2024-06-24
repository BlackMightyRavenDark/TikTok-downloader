using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using static TikTok_downloader.Utils;

namespace TikTok_downloader
{
	internal sealed class Configurator
	{
		public string SelfDirPath { get; set; }
		public string FilePath { get; set; }
		public string DownloadingDirPath { get; set; }
		public string FileNameFormat { get; set; }
		public string BrowserExePath { get; set; }
		public int MenuFontSize { get; set; }

		public Configurator()
		{
			SelfDirPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
			FilePath = SelfDirPath + "TikTokDownloader_config.json";
		}

		public void LoadDefaults()
		{
			DownloadingDirPath = null;
			FileNameFormat = FILENAME_FORMAT_DEFAULT;
			MenuFontSize = 9;
			BrowserExePath = "firefox.exe";
		}

		public void Load()
		{
			LoadDefaults();
			if (File.Exists(FilePath))
			{
				JObject json = JObject.Parse(File.ReadAllText(FilePath));
				JToken jt = json.Value<JToken>("downloadingDirPath");
				if (jt != null)
				{
					DownloadingDirPath = jt.Value<string>();
				}
				jt = json.Value<JToken>("fileNameFormat");
				if (jt != null)
				{
					FileNameFormat = jt.Value<string>();
					if (string.IsNullOrEmpty(FileNameFormat))
					{
						FileNameFormat = FILENAME_FORMAT_DEFAULT;
					}
				}
				jt = json.Value<JToken>("menuFontSize");
				if (jt != null)
				{
					MenuFontSize = jt.Value<int>();
					if (MenuFontSize < 9)
					{
						MenuFontSize = 9;
					}
					else if (MenuFontSize > 16)
					{
						MenuFontSize = 16;
					}
				}
				jt = json.Value<JToken>("browserExePath");
				if (jt != null)
				{
					BrowserExePath = jt.Value<string>();
				}
			}
		}

		public void Save()
		{
			JObject json = new JObject();
			json["downloadingDirPath"] = DownloadingDirPath;
			json["fileNameFormat"] = FileNameFormat;
			json["menuFontSize"] = MenuFontSize;
			json["browserExePath"] = BrowserExePath;
			if (File.Exists(FilePath))
			{
				File.Delete(FilePath);
			}
			File.WriteAllText(FilePath, json.ToString());
		}
	}
}
