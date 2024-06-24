using TikTokApiLib;

namespace TikTok_downloader
{
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
}
