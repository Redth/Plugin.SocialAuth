using System;
namespace Plugin.SocialAuth
{
	public abstract class AuthOptions : IAuthOptions
	{
		public string[] Scopes { get; set; }
	}
}
