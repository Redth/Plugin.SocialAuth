using System;
namespace Plugin.SocialAuth.Facebook
{
	public interface IFacebookAuthOptions : IAuthOptions
	{
		bool WritePermissions { get; set; }

		ImageSize RequestedPhotoSize { get;set; }
	}
}
