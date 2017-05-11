using System;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.OS;

namespace Plugin.SocialAuth.Droid
{
	public static class SocialAuth
	{
		static ActivityLifecycleCallbackManager activityLifecycleManager;

		public static void Init(Android.App.Application app)
		{
			SocialAuthManager.Current.AccountStore.SecureStore = new SecureStore();

			if (activityLifecycleManager == null)
			{
				activityLifecycleManager = new ActivityLifecycleCallbackManager();
				app.RegisterActivityLifecycleCallbacks(activityLifecycleManager);
			}
		}

		public static void Uninit(Android.App.Application app)
		{
			if (activityLifecycleManager != null)
			{
				app.UnregisterActivityLifecycleCallbacks(activityLifecycleManager);
				activityLifecycleManager = null;
			}
		}

		public static Android.Support.V4.App.FragmentActivity CurrentActivity
		{
			get
			{
				return activityLifecycleManager?.CurrentActivity;
			}
		}

		public static bool Callback(Intent intent, Bundle state)
		{
			var providers = SocialAuthManager.Current.RegisteredProviderTypes;

			foreach (var p in providers)
			{
				var callbackMethod = p.GetMethod(nameof(Callback), BindingFlags.Instance | BindingFlags.Public);

				if (callbackMethod != null && callbackMethod.ReturnType == typeof(bool) && callbackMethod.GetParameters().Length == 2)
				{
					var instance = Activator.CreateInstance(p);

					var r = (bool)callbackMethod.Invoke(instance, new object[] { intent, state });

					if (r)
						return true;
				}
			}

			return false;
		}

		public static void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			var providers = SocialAuthManager.Current.RegisteredProviderTypes;

			foreach (var p in providers)
			{
				var onActivityResultMethod = p.GetMethod(nameof(OnActivityResult), BindingFlags.Instance | BindingFlags.Public);

				if (onActivityResultMethod != null && onActivityResultMethod.ReturnType == typeof(void) && onActivityResultMethod.GetParameters().Length == 3)
				{
					var instance = Activator.CreateInstance(p);

					onActivityResultMethod.Invoke(instance, new object[] { requestCode, resultCode, data });
				}
			}
		}
	}
}
