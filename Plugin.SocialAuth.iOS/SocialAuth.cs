using System;
using UIKit;

namespace Plugin.SocialAuth.iOS
{
	public static class SocialAuth
	{
		public static void Init ()
		{
			
		}

		static UIViewController overrideVc = null;

		public static void OverridePresentingViewController (UIViewController viewController)
		{
			overrideVc = viewController;
		}


		public static UIViewController PresentingViewController
		{
			get {
				if (overrideVc != null)
					return overrideVc;

				var window = UIApplication.SharedApplication.KeyWindow;
				var vc = window.RootViewController;
				while (vc.PresentedViewController != null) {
					vc = vc.PresentedViewController;
				}
				return vc;
			}
		}
	}
}
