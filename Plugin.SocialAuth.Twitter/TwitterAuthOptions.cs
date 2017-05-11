using System;
namespace Plugin.SocialAuth.Twitter
{
	public class TwitterAuthOptions : OAuth.OAuth1Options, OAuth.IOAuth1Options
	{
		public TwitterAuthOptions() : base()
		{
			AuthorizeUrl = new Uri("https://api.twitter.com/oauth/authorize");
			RequestTokenUrl = new Uri("https://api.twitter.com/oauth/request_token");
			AccessTokenUrl = new Uri("https://api.twitter.com/oauth/access_token");
		}
	}
}
