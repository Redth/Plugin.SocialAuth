using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace Plugin.SocialAuth.iOS
{
	public interface IAuthProvideriOS<TAccount, TOptions> : IAuthProvider<TAccount, TOptions>
		where TAccount : IAccount
		where TOptions : IAuthOptions
	{
		bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation);
	}
}
