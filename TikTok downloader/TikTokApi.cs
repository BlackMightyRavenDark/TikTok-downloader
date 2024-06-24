using System;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace TikTok_downloader
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
			TikTokMedia media = null;
			if (jData.ContainsKey("hdplay"))
			{
				//Hope it's the best quality.
				long fileSize = jData.Value<long>("hd_size");
				string fileUrl = jData.Value<string>("hdplay");
				media = new TikTokMedia(TikTokMediaType.Video, false, fileSize, fileUrl);
			}
			else if (jData.ContainsKey("play"))
			{
				long fileSize = jData.Value<long>("size");
				string fileUrl = jData.Value<string>("play");
				media = new TikTokMedia(TikTokMediaType.Video, false, fileSize, fileUrl);
			}
			else if (jData.ContainsKey("wmplay"))
			{
				//Video might be unplayable.
				long fileSize = jData.Value<long>("wm_size");
				string fileUrl = jData.Value<string>("wmplay");
				media = new TikTokMedia(TikTokMediaType.Video, false, fileSize, fileUrl);
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
				dateCreation, duration, imagePreviewUrl, author, media);
			return video;
		}

		public static DateTime UnixTimeToDateTime(long unixTime)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dateTime.AddSeconds(unixTime);
		}
	}

	public class TikTokVideo : IDisposable
	{
		public string Title { get; }
		public string Id { get; }
		public string VideoUrl { get; }
		public DateTime DateCreation { get; }
		public TimeSpan Duration { get; }
		public string ImagePreviewUrl { get; }
		public Stream ImagePreviewData { get; private set; }
		public Image ImagePreview { get; private set; }
		public TikTokAuthor Author { get; }
		public TikTokMedia Media { get; }

		public TikTokVideo(string videoTitle, string videoId, string videoUrl,
			DateTime dateCreation, TimeSpan duration, string imagePreviewUrl,
			TikTokAuthor author, TikTokMedia media)
		{
			Title = videoTitle;
			Id = videoId;
			VideoUrl = videoUrl;
			DateCreation = dateCreation;
			Duration = duration;
			ImagePreviewUrl = imagePreviewUrl;
			Author = author;
			Media = media;
		}

		public void Dispose()
		{
			DisposeImagePreviewData();
			DisposeImagePreview();
		}

		public void DisposeImagePreview()
		{
			if (ImagePreview != null)
			{
				ImagePreview.Dispose();
				ImagePreview = null;
			}
		}

		public void DisposeImagePreviewData()
		{
			if (ImagePreviewData != null)
			{
				ImagePreviewData.Close();
				ImagePreviewData = null;
			}
		}
		
		public async Task<int> UpdateImagePreview()
		{
			DisposeImagePreview();
			DisposeImagePreviewData();

			if (string.IsNullOrEmpty(ImagePreviewUrl) || string.IsNullOrWhiteSpace(ImagePreviewUrl))
			{
				return 400;
			}

			ImagePreviewData = new MemoryStream();
			return await Task.Run(() =>
			{
				try
				{
					FileDownloader d = new FileDownloader() { Url = ImagePreviewUrl };
					int errorCode = d.Download(ImagePreviewData);
					if (errorCode == 200)
					{
						ImagePreview = Image.FromStream(ImagePreviewData);
					}
					return errorCode;
				} catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
					DisposeImagePreview();
					DisposeImagePreviewData();
					return ex.HResult;
				}
			});
		}
	}

	public class TikTokAuthor
	{
		public string NickName { get; }
		public string UniqueId { get; }
		public string UserId { get; }
		public string AvatarImageUrl { get; }

		public TikTokAuthor(string nickName, string uniqueId, string userId, string avatarImageUrl)
		{
			NickName = nickName;
			UniqueId = uniqueId;
			UserId = userId;
			AvatarImageUrl = avatarImageUrl;
		}
	}

	public class TikTokVideoDetailsResult
	{
		public JObject Details { get; }
		public int ErrorCode { get; }

		public TikTokVideoDetailsResult(JObject details, int errorCode)
		{
			Details = details;
			ErrorCode = errorCode;
		}
	}

	public enum TikTokMediaType { Video }

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
}
