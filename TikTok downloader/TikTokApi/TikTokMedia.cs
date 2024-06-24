
namespace TikTokApiLib
{
	public class TikTokMedia
	{
		public TikTokMediaType Type { get; }
		public bool WithWatermark { get; }
		public long FileSize { get; }
		public string FileUrl { get; }

		public TikTokMedia(TikTokMediaType type, bool withWatermark, long fileSize, string fileUrl)
		{
			Type = type;
			WithWatermark = withWatermark;
			FileSize = fileSize;
			FileUrl = fileUrl;
		}
	}

	public enum TikTokMediaType { Video }
}
