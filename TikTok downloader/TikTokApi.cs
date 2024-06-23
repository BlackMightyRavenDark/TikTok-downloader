using System;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;
using System.Web;
using Newtonsoft.Json.Linq;

namespace TikTok_downloader
{
	public class TikTokApi
	{
		public const string TIKTOK_API_VIDEO_INFO_URL = "https://tikwm.com/api";

		public string GetVideoInfoRequestUrl(string videoUrl, bool hd = true)
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

		public TikTokVideoDetailsResult GetVideoDetails(string videoUrl)
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

		public static TikTokVideo ParseTikTokInfo(JObject jInfo)
		{
			JObject jData = jInfo.Value<JObject>("data");
			if (jData == null) { return null; }

			string videoTitle = jData.Value<string>("title");
			string videoId = jData.Value<string>("id");
			
			TikTokVideo video = new TikTokVideo(videoTitle, videoId);
			int videoDurationSeconds = jData.Value<int>("duration");
			video.Duration = TimeSpan.FromSeconds(videoDurationSeconds);
			long videoCreationTime = jData.Value<long>("create_time");
			video.DateCreation = UnixTimeToDateTime(videoCreationTime);
			video.ImagePreviewUrl = jData.Value<string>("cover");
			if (jData.ContainsKey("hdplay"))
			{
				//Hope it's the best quality.
				video.FileSize = jData.Value<long>("hd_size");
				video.FileUrlWithoutWatermark = jData.Value<string>("hdplay");
			}
			else if (jData.ContainsKey("play"))
			{
				video.FileSize = jData.Value<long>("size");
				video.FileUrlWithoutWatermark = jData.Value<string>("play");
			}
			else if (jData.ContainsKey("wmplay"))
			{
				//Video might be unplayable.
				video.FileSize = jData.Value<long>("wm_size");
				video.FileUrlWithoutWatermark = jData.Value<string>("wmplay");
			}

			JObject jAuthor = jData.Value<JObject>("author");
			if (jAuthor != null)
			{
				string authorId = jAuthor.Value<string>("id");
				string authorUniqueId = jAuthor.Value<string>("unique_id");
				string authorNickName = jAuthor.Value<string>("nickname");
				video.Author = new TikTokAuthor(authorNickName, authorUniqueId, authorId);
				video.Author.AvatarImageUrl = jAuthor.Value<string>("avatar");
			}

			return video;
		}

		public static DateTime UnixTimeToDateTime(long unixTime)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dateTime.AddSeconds(unixTime);
		}
	}

	public class TikTokVideo
	{
		public string Title { get; private set; }
		public string Id { get; private set; }
		public long FileSize { get; set; }
		public DateTime DateCreation { get; set; }
		public TimeSpan Duration { get; set; }
		public string ImagePreviewUrl { get; set; }
		public Stream ImageData { get; set; }
		public Image ImagePreview { get; set; }
		public string VideoUrl { get; set; }
		public string FileUrlWithoutWatermark { get; set; } = null;
		public TikTokAuthor Author { get; set; }

		public TikTokVideo(string videoTitle, string videoId)
		{
			Title = videoTitle;
			Id = videoId;
		}

		~TikTokVideo()
		{
			if (ImagePreview != null)
			{
				ImagePreview.Dispose();
				ImagePreview = null;
			}
			if (ImageData != null)
			{
				ImageData.Dispose();
				ImageData = null;
			}
		}
	}

	public class TikTokAuthor
	{
		public string NickName { get; private set; }
		public string UniqueId { get; private set; }
		public string UserId { get; private set; }
		public string AvatarImageUrl { get; set; }

		public TikTokAuthor(string nickName, string uniqueId, string userId)
		{
			NickName = nickName;
			UniqueId = uniqueId;
			UserId = userId;
		}
	}

	public class TikTokVideoDetailsResult
	{
		public JObject Details { get; private set; }
		public int ErrorCode { get; private set; }

		public TikTokVideoDetailsResult(JObject details, int errorCode)
		{
			Details = details;
			ErrorCode = errorCode;
		}
	}
}
