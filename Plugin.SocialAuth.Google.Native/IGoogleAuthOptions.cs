using System;
namespace Plugin.SocialAuth.Google.Native
{
	public interface IGoogleAuthOptions : IAuthOptions
	{
		bool FetchProfile { get; }
		string ClientId { get; }
		string ServerClientId { get; }

		string[] Scopes { get; }

		uint RequestedPhotoDimension { get; }
	}
}
