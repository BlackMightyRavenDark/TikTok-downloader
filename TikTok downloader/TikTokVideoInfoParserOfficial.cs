using System;
using Newtonsoft.Json.Linq;

namespace TikTok_downloader
{
    public class TikTokVideoInfoParserOfficial : ITikTokVideoInfoParser
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
            JObject jItemStruct = info.Value<JObject>("itemInfo").Value<JObject>("itemStruct");
            JObject jAuthor = jItemStruct.Value<JObject>("author");
            string userId = jAuthor.Value<string>("id");
            string uniqueId = jAuthor.Value<string>("uniqueId");
            string nickName = jAuthor.Value<string>("nickname");
            TikTokAuthor tikTokAuthor = new TikTokAuthor(nickName, uniqueId, userId);
            tikTokAuthor.AvatarImageUrl = jAuthor.Value<string>("avatarLarger");
            tikTokAuthor.Signature = jAuthor.Value<string>("signature");
            tikTokAuthor.IsVerifiedUser = jAuthor.Value<bool>("verified");

            JObject jVideo = jItemStruct.Value<JObject>("video");
            string videoTitle = jItemStruct.Value<string>("desc");
            string videoId = jItemStruct.Value<string>("id");
            TikTokVideo tikTokVideo = new TikTokVideo(videoTitle, videoId);
            tikTokVideo.Author = tikTokAuthor;
            tikTokVideo.ResolutionWidth = jVideo.Value<int>("width");
            tikTokVideo.ResolutionHeight = jVideo.Value<int>("height");
            tikTokVideo.Bitrate = jVideo.Value<int>("bitrate");
            long num = jItemStruct.Value<long>("createTime");
            tikTokVideo.DateCreation = TikTokApi.UnixTimeToDateTime(num);
            tikTokVideo.Duration = TimeSpan.FromSeconds(jVideo.Value<int>("duration"));
            tikTokVideo.VideoUrl = $"https://tiktok.com/@{tikTokAuthor.UniqueId}/video/{tikTokVideo.Id}";
            tikTokVideo.ImagePreviewUrl = jVideo.Value<string>("originCover");
            tikTokVideo.FileUrl = jVideo.Value<string>("playAddr");
            tikTokVideo.FileUrlWithoutWatermark = null;

            return tikTokVideo;
        }
    }
}
