using System;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace TikTok_downloader
{
    public class TikTokApi
    {
        public const string TIKTOK_DETAIL_ENDPOINT_URL = "https://m.tiktok.com/api/item/detail";
        public const string TIKTOK_AWEME_DETAIL_ENDPOINT_URL = "https://api.tiktokv.com/aweme/v1/multi/aweme/detail";

        public string GetVideoInfoRequestUrl(string videoId)
        {
            NameValueCollection query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query.Add("app_name", "tiktok_web");
            query.Add("device_platform", "web_mobile");
            query.Add("screen_width", "1567");
            query.Add("screen_height", "591");
            query.Add("focus_state", "true");
            query.Add("is_fullscreen", "false");
            query.Add("itemId", videoId);

            string url = $"{TIKTOK_DETAIL_ENDPOINT_URL}/?{query}";
            return url;
        }

        public string GetAwemeVideoInfoRequestUrl(string videoId)
        {
            string url = $"{TIKTOK_AWEME_DETAIL_ENDPOINT_URL}/?aweme_ids=%5B{videoId}%5D";
            return url;
        }

        public TikTokVideoDetailsResult GetVideoDetails(string videoId)
        {
            try
            {
                JObject jResult = new JObject();
                string url = GetVideoInfoRequestUrl(videoId);
                FileDownloader d = new FileDownloader();
                d.Url = url;
                string userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.4 Mobile/15E148 Safari/604.1";
                d.UserAgent = userAgent;
                int errorCodeOfficial = d.DownloadString(out string response);
                JObject jOfficial = null;
                if (errorCodeOfficial == 200)
                {
                    jOfficial = JObject.Parse(response);

                    //checking response
                    int responseStatusCode = jOfficial.Value<int>("statusCode");
                    if (responseStatusCode != 0)
                    {
                        jOfficial = null;
                        errorCodeOfficial = responseStatusCode;
                    }
                }
                jResult.Add(new JProperty("official", jOfficial));

                url = GetAwemeVideoInfoRequestUrl(videoId);
                d.Url = url;
                int errorCodeAweme = d.DownloadString(out string responseAweme);
                JObject jAweme = null;
                if (errorCodeAweme == 200)
                {
                    jAweme = JObject.Parse(responseAweme);

                    //checking response
                    int responseStatusCode = jAweme.Value<int>("status_code");
                    if (responseStatusCode != 0)
                    {
                        jAweme = null;
                        errorCodeAweme = responseStatusCode;
                    }
                }
                jResult.Add(new JProperty("aweme", jAweme));

                if (errorCodeOfficial == 200 || errorCodeAweme == 200)
                {
                    return new TikTokVideoDetailsResult(jResult, 200);
                }
                else
                {
                    return new TikTokVideoDetailsResult(null, 404);
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
            string infoOfficial = jInfo.Value<JToken>("official").ToString();
            TikTokVideoInfoParserOfficial parserOfficial = new TikTokVideoInfoParserOfficial();
            TikTokVideo tikTokVideoOfficial = parserOfficial.Parse(infoOfficial);

            string infoAweme = jInfo.Value<JToken>("aweme").ToString();
            TikTokVideoInfoParserAweMe parserAweMe = new TikTokVideoInfoParserAweMe();
            TikTokVideo tikTokVideoAweme = parserAweMe.Parse(infoAweme);

            if (tikTokVideoOfficial == null)
            {
                return tikTokVideoAweme;
            }
            else if (tikTokVideoAweme != null)
            {
                tikTokVideoOfficial.FileUrlWithoutWatermark = tikTokVideoAweme.FileUrlWithoutWatermark;
            }

            return tikTokVideoOfficial;
        }

        public static string ExtractVideoIdFromUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                if (!uri.Host.ToLower().EndsWith("tiktok.com"))
                {
                    return null;
                }
                int n = uri.OriginalString.IndexOf("/video/");
                if (n < 0)
                {
                    return null;
                }
                string id = uri.OriginalString.Substring(n + 7);
                return id;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return url;
            }
        }

        public static DateTime UnixTimeToDateTime(long unixTime)
        {
            DateTime dateTime = new DateTime(1970, 1, 1);
            return dateTime.AddSeconds(unixTime);
        }
    }

    public class TikTokVideo
    {
        public string Title { get; private set; }
        public string Id { get; private set; }
        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }
        public int Bitrate { get; set; }
        public DateTime DateCreation { get; set; }
        public TimeSpan Duration { get; set; }
        public string ImagePreviewUrl { get; set; }
        public Stream ImageData { get; set; }
        public Image Image;
        public string VideoUrl { get; set; }
        public string FileUrl { get; set; } = null;
        public string FileUrlWithoutWatermark { get; set; } = null;
        public TikTokAuthor Author { get; set; }

        public TikTokVideo(string videoTitle, string videoId)
        {
            Title = videoTitle;
            Id = videoId;
        }

        ~TikTokVideo()
        {
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
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
        public string Signature { get; set; }
        public bool IsVerifiedUser { get; set; }

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
