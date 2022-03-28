using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace TikTok_downloader
{
    public static class Helper
    {
        public const string FILENAME_FORMAT_DEFAULT =
            "<channel_name> [<year>-<month>-<day> <hour>-<minute>-<second>] <video_title> [<video_id>]";

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
        public string selfDirPath;
        public string filePath;
        public string downloadingDirPath;
        public string fileNameFormat;
        public string browserExePath;
        public int menuFontSize;

        public MainConfiguration()
        {
            selfDirPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            filePath = selfDirPath + "TikTokDownloader_config.json";
        }

        public void LoadDefaults()
        {
            downloadingDirPath = null;
            fileNameFormat = Helper.FILENAME_FORMAT_DEFAULT;
            menuFontSize = 9;
            browserExePath = "firefox.exe";
        }

        public void Load()
        {
            LoadDefaults();
            if (File.Exists(filePath))
            {
                JObject json = JObject.Parse(File.ReadAllText(filePath));
                JToken jt = json.Value<JToken>("downloadingDirPath");
                if (jt != null)
                {
                    downloadingDirPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("fileNameFormat");
                if (jt != null)
                {
                    fileNameFormat = jt.Value<string>();
                    if (string.IsNullOrEmpty(fileNameFormat))
                    {
                        fileNameFormat = Helper.FILENAME_FORMAT_DEFAULT;
                    }
                }
                jt = json.Value<JToken>("menuFontSize");
                if (jt != null)
                {
                    menuFontSize = jt.Value<int>();
                    if (menuFontSize < 9)
                    {
                        menuFontSize = 9;
                    }
                    else if (menuFontSize > 16)
                    {
                        menuFontSize = 16;
                    }
                }
                jt = json.Value<JToken>("browserExePath");
                if (jt != null)
                {
                    browserExePath = jt.Value<string>();
                }
            }
        }

        public void Save()
        {
            JObject json = new JObject();
            json["downloadingDirPath"] = downloadingDirPath;
            json["fileNameFormat"] = fileNameFormat;
            json["menuFontSize"] = menuFontSize;
            json["browserExePath"] = browserExePath;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath, json.ToString());
        }
    }
}
