using System;
namespace Plugin.SocialAuth.Google
{
	public class GoogleAuthOptions : AuthOptions, IGoogleAuthOptions
	{
		public bool FetchProfile { get; set; }
		public string ClientId { get; set; }
		public string ServerClientId { get; set; }

		public string[] Scopes { get; set; }
		public uint RequestedPhotoDimension { get; set; } = 256;
	}
}
