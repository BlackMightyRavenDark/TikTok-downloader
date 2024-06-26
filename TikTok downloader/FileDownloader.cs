﻿using System;
using System.IO;
using System.Net;
using System.Text;

namespace TikTok_downloader
{
	public sealed class FileDownloader
	{
		public string Url { get; set; }
		public string UserAgent { get; set; }
		public long StreamSize { get; private set; } = 0L;
		private long _bytesTransferred = 0L;
		private long _rangeFrom = 0L;
		private long _rangeTo = 0L;
		public int ProgressUpdateInterval { get; set; } = 10;
		public bool Stopped { get; private set; } = false;
		public int LastErrorCode { get; private set; } = DOWNLOAD_ERROR_UNKNOWN;

		public const int DOWNLOAD_ERROR_UNKNOWN = -1;
		public const int DOWNLOAD_ERROR_ABORTED_BY_USER = -2;
		public const int DOWNLOAD_ERROR_INCOMPLETE_DATA_READ = -3;
		public const int DOWNLOAD_ERROR_RANGE = -4;
		public const int DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT = -5;
		public const int DOWNLOAD_ERROR_INVALID_URL = -6;

		public delegate void ConnectingDelegate(object sender, string url);
		public delegate void ConnectedDelegate(object sender, string url, int errorCode, long contentLength, ref bool abort);
		public delegate void WorkStartedDelegate(object sender, long contentLength);
		public delegate void WorkProgressDelegate(object sender, long bytesTransfered, long contentLength);
		public delegate void WorkFinishedDelegate(object sender, long bytesTransfered, long contentLength, int errorCode);
		public delegate void CancelTestDelegate(object sender, ref bool stop);
		public ConnectingDelegate Connecting;
		public ConnectedDelegate Connected;
		public WorkStartedDelegate WorkStarted;
		public WorkProgressDelegate WorkProgress;
		public WorkFinishedDelegate WorkFinished;
		public CancelTestDelegate CancelTest;

		public int Download(Stream stream)
		{          
			Stopped = false;
			LastErrorCode = DOWNLOAD_ERROR_UNKNOWN;
			_bytesTransferred = 0L;
			StreamSize = stream.Length;

			Connecting?.Invoke(this, Url);

			WebContent content = new WebContent();
			content.UserAgent = UserAgent;
			LastErrorCode = content.GetResponseStream(Url, _rangeFrom, _rangeTo);
			bool abort = false;
			Connected?.Invoke(this, Url, LastErrorCode, content.Length, ref abort);
			if ((LastErrorCode != 200 && LastErrorCode != 206) || abort)
			{
				content.Dispose();
				if (abort)
				{
					LastErrorCode = DOWNLOAD_ERROR_ABORTED_BY_USER;
				}
				return LastErrorCode;
			}

			if (content.Length == 0L)
			{
				content.Dispose();
				return DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT;
			}

			WorkStarted?.Invoke(this, content.Length);

			LastErrorCode = ContentToStream(content, stream);
			long size = content.Length;
			content.Dispose();

			WorkFinished?.Invoke(this, _bytesTransferred, size, LastErrorCode);

			return LastErrorCode;
		}

		public int DownloadString(out string resString)
		{
			resString = null;

			Stopped = false;
			LastErrorCode = DOWNLOAD_ERROR_UNKNOWN;
			_bytesTransferred = 0L;
			StreamSize = 0L;

			WebContent content = new WebContent();
			content.UserAgent = UserAgent;
			LastErrorCode = content.GetResponseStream(Url, _rangeFrom, _rangeTo);
			if (LastErrorCode != 200 && LastErrorCode != 206)
			{
				content.Dispose();
				return LastErrorCode;
			}

			if (content.Length == 0L)
			{
				content.Dispose();
				return DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT;
			}

			WorkStarted?.Invoke(this, content.Length);

			MemoryStream memoryStream = new MemoryStream();
			LastErrorCode = ContentToStream(content, memoryStream);
			if (LastErrorCode == 200)
			{
				resString = Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
			long size = content.Length;
			content.Dispose();
			memoryStream.Dispose();

			WorkFinished?.Invoke(this, _bytesTransferred, size, LastErrorCode);

			return LastErrorCode;
		}

		private int ContentToStream(WebContent content, Stream stream)
		{
			byte[] buf = new byte[4096];
			int bytesRead;
			int errorCode = 200;
			int iter = 0;
			try
			{
				do
				{
					bytesRead = content.ContentData.Read(buf, 0, buf.Length);
					if (bytesRead <= 0)
					{
						break;
					}
					stream.Write(buf, 0, bytesRead);
					_bytesTransferred += bytesRead;
					StreamSize = stream.Length;
					if (WorkProgress != null && (ProgressUpdateInterval == 0 || iter++ >= ProgressUpdateInterval || StreamSize == content.Length))
					{
						WorkProgress.Invoke(this, _bytesTransferred, content.Length);
						iter = 0;
					}
					if (CancelTest != null)
					{
						bool stop = false;
						CancelTest.Invoke(this, ref stop);
						Stopped = stop;
						if (Stopped)
						{
							break;
						}
					}
				}
				while (bytesRead > 0);
			}
			catch (Exception)
			{
				errorCode = DOWNLOAD_ERROR_UNKNOWN;
			}
			if (Stopped)
			{
				errorCode = DOWNLOAD_ERROR_ABORTED_BY_USER;
			}
			else if (errorCode == 200)
			{
				if (content.Length >= 0L && _bytesTransferred != content.Length)
				{
					LastErrorCode = DOWNLOAD_ERROR_INCOMPLETE_DATA_READ;
				}
			}
			return errorCode;
		}

		public void SetRange(long from, long to)
		{
			_rangeFrom = from;
			_rangeTo = to;
		}

		public static int GetUrlContentLength(string url, out long contentLength)
		{
			WebContent webContent = new WebContent();
			int errorCode = webContent.GetResponseStream(url);
			contentLength = errorCode == 200 ? webContent.Length : -1L;
			webContent.Dispose();
			return errorCode;
		}
	}

	public sealed class WebContent : IDisposable
	{
		public string UserAgent { get; set; }
		private HttpWebResponse webResponse = null;
		public long Length { get; private set; } = -1L;
		public Stream ContentData { get; private set; } = null;

		public void Dispose()
		{
			if (webResponse != null)
			{
				webResponse.Dispose();
				webResponse = null;
			}
			if (ContentData != null)
			{
				ContentData.Dispose();
				ContentData = null;
				Length = -1L;
			}
		}

		public int GetResponseStream(string url)
		{
			int errorCode = GetResponseStream(url, 0L, 0L);
			return errorCode;
		}

		public int GetResponseStream(string url, long rangeFrom, long rangeTo)
		{
			int errorCode = GetResponseStream(url, rangeFrom, rangeTo, out Stream stream);
			if (errorCode == 200 || errorCode == 206)
			{
				ContentData = stream;
				Length = webResponse.ContentLength;
			}
			else
			{
				ContentData = null;
				Length = -1L;
			}
			return errorCode;
		}

		public int GetResponseStream(string url, long rangeFrom, long rangeTo, out Stream stream)
		{
			stream = null;
			if (rangeTo > 0L && rangeFrom > rangeTo)
			{
				return FileDownloader.DOWNLOAD_ERROR_RANGE;
			}
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				if (rangeTo > 0L)
				{
					request.AddRange(rangeFrom, rangeTo);
				}

				request.UserAgent = UserAgent;
				webResponse = (HttpWebResponse)request.GetResponse();
				int statusCode = (int)webResponse.StatusCode;
				if (statusCode == 200 || statusCode == 206)
				{
					stream = webResponse.GetResponseStream();
				}
				return statusCode;
			}
			catch (WebException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				if (webResponse != null)
				{
					webResponse.Dispose();
					webResponse = null;
				}
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
					int statusCode = (int)httpWebResponse.StatusCode;
					return statusCode;
				}
				else
				{
					return ex.HResult;
				}
			}
			catch (NotSupportedException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				if (webResponse != null)
				{
					webResponse.Dispose();
					webResponse = null;
				}
				return FileDownloader.DOWNLOAD_ERROR_INVALID_URL;
			}
			catch (UriFormatException ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				if (webResponse != null)
				{
					webResponse.Dispose();
					webResponse = null;
				}
				return FileDownloader.DOWNLOAD_ERROR_INVALID_URL;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				if (webResponse != null)
				{
					webResponse.Dispose();
					webResponse = null;
				}
				return ex.HResult;
			}
		}
	}
}
