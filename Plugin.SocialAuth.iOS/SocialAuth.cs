using System;
using System.Linq;
using System.Reflection;
using Foundation;
using UIKit;

namespace Plugin.SocialAuth.iOS
{
	public static class SocialAuth
	{
		public static void Init()
		{
			SocialAuthManager.Current.AccountStore.SecureStore = new SecureStore();
		}

		static UIViewController overrideVc = null;

		public static void OverridePresentingViewController(UIViewController viewController)
		{
			overrideVc = viewController;
		}

		public static UIViewController PresentingViewController
		{
			get
			{
				if (overrideVc != null)
					return overrideVc;

				var window = UIApplication.SharedApplication.KeyWindow;
				var vc = window.RootViewController;
				while (vc.PresentedViewController != null)
				{
					vc = vc.PresentedViewController;
				}
				return vc;
			}
		}

		public static bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			var providers = SocialAuthManager.Current.RegisteredProviderTypes;

			foreach (var p in providers)
			{
				var openUrlMethod = p.GetMethod(nameof(OpenUrl), BindingFlags.Instance | BindingFlags.Public);

				if (openUrlMethod != null && openUrlMethod.ReturnType == typeof(bool))
				{
					var instance = Activator.CreateInstance(p);

					var r = (bool)openUrlMethod.Invoke(instance, new object[] { application, url, sourceApplication, annotation });

					if (r)
						return true;
				}
			}

			return false;
		}
	}
}
