using System;
namespace Plugin.SocialAuth.Droid
{
	public static class SocialAuth
	{
		static ActivityLifecycleCallbackManager activityLifecycleManager;

		public static void Init (Android.App.Application app)
		{
			Registrar.Register (new SecureStore ());

			if (activityLifecycleManager == null)
			{
				activityLifecycleManager = new ActivityLifecycleCallbackManager ();
				app.RegisterActivityLifecycleCallbacks (activityLifecycleManager);
			}
		}

		public static void Uninit (Android.App.Application app)
		{
			if (activityLifecycleManager != null)
			{
				app.UnregisterActivityLifecycleCallbacks (activityLifecycleManager);
				activityLifecycleManager = null;
			}
		}

		public static Android.Support.V4.App.FragmentActivity CurrentActivity {
			get {
				return activityLifecycleManager?.CurrentActivity;
			}
		}
	}
}
