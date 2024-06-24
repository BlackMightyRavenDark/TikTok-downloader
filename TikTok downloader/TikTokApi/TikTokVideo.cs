using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using TikTok_downloader;

namespace TikTokApiLib
{
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
		public List<TikTokMedia> Medias { get; }

		public TikTokVideo(string videoTitle, string videoId, string videoUrl,
			DateTime dateCreation, TimeSpan duration, string imagePreviewUrl,
			TikTokAuthor author, List<TikTokMedia> medias)
		{
			Title = videoTitle;
			Id = videoId;
			VideoUrl = videoUrl;
			DateCreation = dateCreation;
			Duration = duration;
			ImagePreviewUrl = imagePreviewUrl;
			Author = author;
			Medias = medias;
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
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
					DisposeImagePreview();
					DisposeImagePreviewData();
					return ex.HResult;
				}
			});
		}
	}
}
