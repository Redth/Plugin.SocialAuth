using System;
namespace Plugin.SocialAuth.Facebook
{
	public interface IFacebookAuthOptions : IAuthOptions
	{
		bool WritePermissions { get; set; }

		string[] Scopes { get; set; }

		ImageSize RequestedPhotoSize { get; set; }
	}
}
