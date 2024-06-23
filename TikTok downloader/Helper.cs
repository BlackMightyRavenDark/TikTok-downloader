using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TikTok_downloader
{
	public static class Helper
	{
		public const string FILENAME_FORMAT_DEFAULT =
			"<channel_name> [<year>-<month>-<day> <hour>-<minute>-<second><GMT>] <video_title> [<video_id>]";

		public static string LeadZero(int n)
		{
			return n < 10 ? $"0{n}" : n.ToString();
		}

		public static string FormatFileName(string fmt, DownloadableItem downloadableItem)
		{
			string t = fmt.Replace("<channel_name>", downloadableItem.Video.Author.UniqueId)
				.Replace("<year>", LeadZero(downloadableItem.Video.DateCreation.Year))
				.Replace("<month>", LeadZero(downloadableItem.Video.DateCreation.Month))
				.Replace("<day>", LeadZero(downloadableItem.Video.DateCreation.Day))
				.Replace("<hour>", LeadZero(downloadableItem.Video.DateCreation.Hour))
				.Replace("<minute>", LeadZero(downloadableItem.Video.DateCreation.Minute))
				.Replace("<second>", LeadZero(downloadableItem.Video.DateCreation.Second))
				.Replace("<GMT>", " GMT")
				.Replace("<video_id>", downloadableItem.Video.Id);
			if (!string.IsNullOrEmpty(downloadableItem.Video.Title) && !string.IsNullOrWhiteSpace(downloadableItem.Video.Title))
			{
				t = t.Replace("<video_title>", downloadableItem.Video.Title);
			}
			else
			{
				t = t.Replace("<video_title>", "\u227A" + "Untitled" + "\u227B");
			}
			if (!downloadableItem.IsLogoPresent)
			{
				t += "_no_watermark";
			}
			return t;
		}

		public static string FixFileName(string fn)
		{
			return fn.Replace("\\", "\u29F9").Replace("|", "\u2758").Replace("/", "\u2044")
				.Replace("?", "\u2753").Replace(":", "\uFE55").Replace("<", "\u227A").Replace(">", "\u227B")
				.Replace("\"", "\u201C").Replace("*", "\uFE61").Replace("^", "\u2303").Replace("\n", " ");
		}

		public static string GetNumberedFileName(string filePath)
		{
			if (File.Exists(filePath))
			{
				int n = filePath.LastIndexOf(".");
				string part1 = filePath.Substring(0, n);
				string ext = filePath.Substring(n);
				string newFilePath;
				int i = 2;
				do
				{
					newFilePath = $"{part1}_{i++}{ext}";
				} while (File.Exists(newFilePath));
				return newFilePath;
			}
			return filePath;
		}

		public static Rectangle ResizeTo(this Rectangle source, Size newSize)
		{
			float aspectSource = source.Height / (float)source.Width;
			float aspectDest = newSize.Height / (float)newSize.Width;
			int w = newSize.Width;
			int h = newSize.Height;
			if (aspectSource > aspectDest)
			{
				w = (int)(newSize.Height / aspectSource);
			}
			else if (aspectSource < aspectDest)
			{
				h = (int)(newSize.Width * aspectSource);
			}
			return new Rectangle(0, 0, w, h);
		}

		public static Rectangle CenterIn(this Rectangle source, Rectangle dest)
		{
			int x = dest.Width / 2 - source.Width / 2;
			int y = dest.Height / 2 - source.Height / 2;
			return new Rectangle(x, y, source.Width, source.Height);
		}

		public static bool IsGmt(this DateTime dateTime)
		{
			return dateTime.Kind == DateTimeKind.Utc;
		}

		public static string FormatDateTime(DateTime dateTime)
        {
			string t = dateTime.ToString("yyyy.MM.dd HH:mm:ss");
			return dateTime.IsGmt() ? $"{t} GMT" : t;
        }

		public static string FormatSize(long n)
		{
			const int KB = 1000;
			const int MB = 1000000;
			const int GB = 1000000000;
			const long TB = 1000000000000;
			long b = n % KB;
			long kb = (n % MB) / KB;
			long mb = (n % GB) / MB;
			long gb = (n % TB) / GB;

			if (n >= 0 && n < KB)
				return string.Format("{0} B", b);
			if (n >= KB && n < MB)
				return string.Format("{0},{1:D3} KB", kb, b);
			if (n >= MB && n < GB)
				return string.Format("{0},{1:D3} MB", mb, kb);
			if (n >= GB && n < TB)
				return string.Format("{0},{1:D3},{2:D3} GB", gb, mb, kb);

			return string.Format("{0} {1:D3} {2:D3} {3:D3} bytes", gb, mb, kb, b);
		}
	}

	public class DownloadableItem
	{
		public string Url { get; private set; }
		public long Size { get; private set; }
		public bool IsLogoPresent { get; private set; }
		public TikTokVideo Video { get; private set; }

		public DownloadableItem(string url, long size, bool withLogo, TikTokVideo video)
		{
			Url = url;
			Size = size;
			IsLogoPresent = withLogo;
			Video = video;
		}
	}

	public sealed class MainConfiguration
	{
		public string SelfDirPath { get; set; }
		public string FilePath { get; set; }
		public string DownloadingDirPath { get; set; }
		public string FileNameFormat { get; set; }
		public string BrowserExePath { get; set; }
		public int MenuFontSize { get; set; }

		public MainConfiguration()
		{
			SelfDirPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
			FilePath = SelfDirPath + "TikTokDownloader_config.json";
		}

		public void LoadDefaults()
		{
			DownloadingDirPath = null;
			FileNameFormat = Helper.FILENAME_FORMAT_DEFAULT;
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
						FileNameFormat = Helper.FILENAME_FORMAT_DEFAULT;
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
