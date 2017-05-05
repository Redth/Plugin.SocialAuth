using System;
namespace Plugin.SocialAuth.Google
{
	public interface IGoogleAuthOptions : IAuthOptions
	{
		bool FetchProfile { get; }
		string ClientId { get; }
		string ServerClientId { get; }

		uint RequestedPhotoDimension { get; }
	}
}
