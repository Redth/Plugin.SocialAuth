using System;
using System.Collections.Generic;

namespace Plugin.SocialAuth
{
	public static class Registrar
	{
		public static ISecureStore SecureStore { get; private set; }

		public static void Register (ISecureStore secureStore)
		{
			SecureStore = secureStore;
		}

		static Dictionary<ProviderType, List<Type>> authProviders
			= new Dictionary<ProviderType, List<Type>> ();

		public static void Register (ProviderType providerType, Type providerImplementationType)
		{
			if (!authProviders.ContainsKey (providerType))
				authProviders.Add (providerType, new List<Type>());

			if (!authProviders[providerType].Contains (providerImplementationType))
				authProviders[providerType].Add (providerImplementationType);
		}

		public static void Unregister (ProviderType providerType, Type providerImplementationType)
		{
			if (!authProviders.ContainsKey (providerType))
				authProviders.Add (providerType, new List<Type>());

			if (authProviders[providerType].Contains (providerImplementationType))
				authProviders[providerType].Remove (providerImplementationType);
		}

		public static List<Type> Find (ProviderType providerType)
		{
			if (!authProviders.ContainsKey (providerType))
				return new List<Type> ();
			
			return authProviders[providerType];
		}
	}
}
