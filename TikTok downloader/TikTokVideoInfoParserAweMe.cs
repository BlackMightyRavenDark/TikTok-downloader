using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TikTok_downloader
{
    public class TikTokVideoInfoParserAweMe : ITikTokVideoInfoParser
    {
        public TikTokVideo Parse(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString) || string.IsNullOrWhiteSpace(jsonString))
            {
                return null;
            }
            JObject json = JObject.Parse(jsonString);
            if (json == null)
            {
                return null;
            }
            return Parse(json);
        }

        public TikTokVideo Parse(JObject info)
        {
            JObject jAwemeDetails = info.Value<JArray>("aweme_details")[0] as JObject;
            JObject jAuthor = jAwemeDetails.Value<JObject>("author");
            string userId = jAuthor.Value<string>("uid");
            string uniqueId = jAuthor.Value<string>("unique_id");
            string nickName = jAuthor.Value<string>("nickname");
            TikTokAuthor tikTokAuthor = new TikTokAuthor(nickName, uniqueId, userId);
            JToken jt = jAuthor.Value<JToken>("avatar_thumb");
            if (jt != null)
            {
                JArray jAvatarUrls = jt.Value<JObject>().Value<JArray>("url_list");
                tikTokAuthor.AvatarImageUrl = jAvatarUrls != null && jAvatarUrls.Count > 0 ? jAvatarUrls[jAvatarUrls.Count - 1].ToString() : null;
            }
            tikTokAuthor.Signature = jAuthor.Value<string>("signature");
            tikTokAuthor.IsVerifiedUser = jAuthor.Value<string>("custom_verify") == "Verified account";

            JObject jVideo = jAwemeDetails.Value<JObject>("video");
            string videoTitle = jAwemeDetails.Value<string>("desc");
            string videoId = jAwemeDetails.Value<JObject>("status").Value<string>("aweme_id");
            TikTokVideo tikTokVideo = new TikTokVideo(videoTitle, videoId);
            tikTokVideo.Author = tikTokAuthor;

            JObject jPlayAddr = jVideo.Value<JObject>("play_addr");
            tikTokVideo.ResolutionWidth = jPlayAddr.Value<int>("width");
            tikTokVideo.ResolutionHeight = jPlayAddr.Value<int>("height");

            JObject jBitRate = jVideo.Value<JArray>("bit_rate")[0] as JObject;
            tikTokVideo.Bitrate = jBitRate.Value<int>("bit_rate");

            long num = jAwemeDetails.Value<long>("create_time");
            tikTokVideo.DateCreation = TikTokApi.UnixTimeToDateTime(num);
            tikTokVideo.Duration = TimeSpan.FromSeconds(jVideo.Value<int>("duration") / 1000);
            tikTokVideo.VideoUrl = $"https://tiktok.com/@{tikTokAuthor.UniqueId}/video/{tikTokVideo.Id}";

            JObject jOriginCover = jVideo.Value<JObject>("origin_cover");
            if (jOriginCover != null)
            {
                JArray jCoverUrls = jOriginCover.Value<JArray>("url_list");
                tikTokVideo.ImagePreviewUrl = 
                    jCoverUrls != null && jCoverUrls.Count > 0 ? jCoverUrls[jCoverUrls.Count - 1].ToString() : null;
            }

            tikTokVideo.FileUrl = null;
            JArray jDownloadUrls = jPlayAddr.Value<JArray>("url_list");
            tikTokVideo.FileUrlWithoutWatermark = jDownloadUrls[jDownloadUrls.Count - 1].ToString();

            if (!string.IsNullOrEmpty(tikTokVideo.ImagePreviewUrl))
            {
                Stream stream = new MemoryStream();
                FileDownloader d = new FileDownloader();
                d.Url = tikTokVideo.ImagePreviewUrl;
                int errorCode = d.Download(stream);
                if (errorCode == 200)
                {
                    tikTokVideo.ImageData = stream;
                    tikTokVideo.Image = Image.FromStream(stream);
                }
                else
                {
                    stream.Dispose();
                }
            }

            return tikTokVideo;
        }
    }
}
