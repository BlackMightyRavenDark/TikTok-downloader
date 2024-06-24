using System;
using System.Collections.Generic;
using System.IO;

namespace TikTok_downloader
{
	public class Utils
	{
		public const string FILENAME_FORMAT_DEFAULT =
			"<channel_name> [<year>-<month>-<day> <hour>-<minute>-<second><GMT>] <video_title> [<video_id>]";

		public static string FormatFileName(string fmt, DownloadableItem downloadableItem)
		{
			string t = fmt.Replace("<channel_name>", downloadableItem.Video.Author.UniqueId)
				.Replace("<year>", downloadableItem.Video.DateCreation.Year.ToString())
				.Replace("<month>", downloadableItem.Video.DateCreation.Month.ToString().PadLeft(2, '0'))
				.Replace("<day>", downloadableItem.Video.DateCreation.Day.ToString().PadLeft(2, '0'))
				.Replace("<hour>", downloadableItem.Video.DateCreation.Hour.ToString().PadLeft(2, '0'))
				.Replace("<minute>", downloadableItem.Video.DateCreation.Minute.ToString().PadLeft(2, '0'))
				.Replace("<second>", downloadableItem.Video.DateCreation.Second.ToString().PadLeft(2, '0'))
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

		public static string FormatDateTime(DateTime dateTime)
		{
			string t = dateTime.ToString("yyyy.MM.dd HH:mm:ss");
			return dateTime.IsGmt() ? $"{t} GMT" : t;
		}

		public static int GetMaxLogoLength(IEnumerable<DownloadableItem> items)
		{
			int max = 0;
			foreach (DownloadableItem item in items)
			{
				string t = item.IsLogoPresent ? "С логотипом" : "Без логотипа";
				if (t.Length > max) { max = t.Length; }
			}
			return max;
		}

		public static int GetMaxSizeLength(IEnumerable<DownloadableItem> items)
		{
			int max = 0;
			foreach (DownloadableItem item in items)
			{
				string t = FormatSize(item.Size);
				if (t.Length > max) { max = t.Length; }
			}
			return max;
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
}
