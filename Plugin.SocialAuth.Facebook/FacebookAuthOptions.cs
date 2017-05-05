using System;
namespace Plugin.SocialAuth.Facebook
{
	public class FacebookAuthOptions : AuthOptions, IFacebookAuthOptions
	{
		public bool WritePermissions { get;set; }

		public ImageSize RequestedPhotoSize { get;set; }
	}
}
