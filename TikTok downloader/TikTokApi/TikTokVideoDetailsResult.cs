using Newtonsoft.Json.Linq;

namespace TikTokApiLib
{
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
}
