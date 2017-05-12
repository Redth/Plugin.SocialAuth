using System;
namespace Plugin.SocialAuth.Facebook.Native
{
	public class FacebookAuthOptions : AuthOptions, IFacebookAuthOptions
	{
		public bool WritePermissions { get; set; }

		public string[] Scopes { get; set; }

		public ImageSize RequestedPhotoSize { get; set; }
	}
}
