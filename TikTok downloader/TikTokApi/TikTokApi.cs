using System;
using System.Collections.Specialized;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TikTok_downloader;

namespace TikTokApiLib
{
	public static class TikTokApi
	{
		public const string TIKTOK_API_VIDEO_INFO_URL = "https://tikwm.com/api";

		public static string GetVideoInfoRequestUrl(string videoUrl, bool hd = true)
		{
			NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
			query.Add("url", HttpUtility.UrlEncode(videoUrl));
			if (hd)
			{
				query.Add("hd", "1");
			}

			string url = $"{TIKTOK_API_VIDEO_INFO_URL}/?{query}";
			return url;
		}

		public static TikTokVideoDetailsResult GetVideoDetails(string videoUrl)
		{
			try
			{
				JObject jResult = new JObject();
				FileDownloader d = new FileDownloader();
				d.Url = GetVideoInfoRequestUrl(videoUrl);
				string userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.4 Mobile/15E148 Safari/604.1";
				d.UserAgent = userAgent;
				int errorCode = d.DownloadString(out string response);
				if (errorCode == 200)
				{
					JObject json = JObject.Parse(response);
					return new TikTokVideoDetailsResult(json, 200);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			return new TikTokVideoDetailsResult(null, 400);
		}

		public static TikTokVideo ParseTikTokInfo(JObject jInfo, string videoUrl = null)
		{
			JObject jData = jInfo.Value<JObject>("data");
			if (jData == null) { return null; }

			string videoTitle = jData.Value<string>("title");
			string videoId = jData.Value<string>("id");
			int videoDurationSeconds = jData.Value<int>("duration");
			TimeSpan duration = TimeSpan.FromSeconds(videoDurationSeconds);
			long videoCreationTime = jData.Value<long>("create_time");
			DateTime dateCreation = UnixTimeToDateTime(videoCreationTime);
			string imagePreviewUrl = jData.Value<string>("cover");
			List<TikTokMedia> medias = new List<TikTokMedia>();
			if (jData.ContainsKey("hdplay"))
			{
				//Hope it's the best quality.
				long fileSize = jData.Value<long>("hd_size");
				string fileUrl = jData.Value<string>("hdplay");
				medias.Add(new TikTokMedia(TikTokMediaType.Video, false, fileSize, fileUrl));
			}
			if (jData.ContainsKey("play"))
			{
				long fileSize = jData.Value<long>("size");
				string fileUrl = jData.Value<string>("play");
				medias.Add(new TikTokMedia(TikTokMediaType.Video, false, fileSize, fileUrl));
			}
			if (jData.ContainsKey("wmplay"))
			{
				//Video might be unplayable.
				long fileSize = jData.Value<long>("wm_size");
				string fileUrl = jData.Value<string>("wmplay");
				medias.Add(new TikTokMedia(TikTokMediaType.Video, true, fileSize, fileUrl));
			}

			JObject jAuthor = jData.Value<JObject>("author");
			TikTokAuthor author = null;
			if (jAuthor != null)
			{
				string authorId = jAuthor.Value<string>("id");
				string authorUniqueId = jAuthor.Value<string>("unique_id");
				string authorNickName = jAuthor.Value<string>("nickname");
				string authorAvatarImageUrl = jAuthor.Value<string>("avatar");
				author = new TikTokAuthor(authorNickName, authorUniqueId, authorId, authorAvatarImageUrl);

				videoUrl = $"https://www.tiktok.com/@{authorUniqueId}/video/{videoId}";
			}

			TikTokVideo video = new TikTokVideo(videoTitle, videoId, videoUrl,
				dateCreation, duration, imagePreviewUrl, author, medias);
			return video;
		}

		public static DateTime UnixTimeToDateTime(long unixTime)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dateTime.AddSeconds(unixTime);
		}
	}
}
