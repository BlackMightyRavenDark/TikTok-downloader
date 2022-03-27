using Newtonsoft.Json.Linq;

namespace TikTok_downloader
{
    public interface ITikTokVideoInfoParser
    {
        TikTokVideo Parse(string jsonString);
        TikTokVideo Parse(JObject info);
    }
}
