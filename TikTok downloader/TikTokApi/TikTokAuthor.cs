
namespace TikTokApiLib
{
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
}
