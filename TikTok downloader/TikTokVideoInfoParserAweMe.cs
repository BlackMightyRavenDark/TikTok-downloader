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
            string userId = jAuthor.Value<string>("id");
            string uniqueId = jAuthor.Value<string>("uniqueId");
            string nickName = jAuthor.Value<string>("nickname");
            TikTokAuthor tikTokAuthor = new TikTokAuthor(nickName, uniqueId, userId);
            tikTokAuthor.AvatarImageUrl = jAuthor.Value<string>("avatarLarger");
            tikTokAuthor.Signature = jAuthor.Value<string>("signature");
            tikTokAuthor.IsVerifiedUser = jAuthor.Value<bool>("verified");

            JObject jVideo = jAwemeDetails.Value<JObject>("video");
            string videoTitle = jAwemeDetails.Value<string>("desc");
            string videoId = jAwemeDetails.Value<string>("id");
            TikTokVideo tikTokVideo = new TikTokVideo(videoTitle, videoId);
            tikTokVideo.Author = tikTokAuthor;
            tikTokVideo.ResolutionWidth = jVideo.Value<int>("width");
            tikTokVideo.ResolutionHeight = jVideo.Value<int>("height");
            tikTokVideo.Bitrate = jVideo.Value<int>("bitrate");
            long num = jAwemeDetails.Value<long>("createTime");
            tikTokVideo.DateCreation = TikTokApi.UnixTimeToDateTime(num);
            tikTokVideo.Duration = TimeSpan.FromSeconds(jVideo.Value<int>("duration"));
            tikTokVideo.VideoUrl = $"https://tiktok.com/@{tikTokAuthor.UniqueId}/video/{tikTokVideo.Id}";
            tikTokVideo.ImagePreviewUrl = jVideo.Value<string>("originCover");
            tikTokVideo.FileUrl = null;
            JArray jUrls = jVideo.Value<JObject>("play_addr").Value<JArray>("url_list");
            tikTokVideo.FileUrlWithoutWatermark = jUrls[jUrls.Count - 1].ToString();

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

            return tikTokVideo;
        }
    }
}
