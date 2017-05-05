using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Plugin.SocialAuth
{
	public class AccountStore : IAccountStore
	{
		public IAccount FindAccount (ProviderType provider, string id)
		{
			if (string.IsNullOrEmpty (id))
				return null;
			
			var providerId = GetProviderId (provider);

			var accounts = JsonSerializer.Deserialize<Dictionary<string, IAccount>> (Registrar.SecureStore?[providerId]);

			if (accounts?.ContainsKey(id) ?? false)
				return accounts[id];

			return null;
		}

		public IDictionary<string, IAccount> FindAccounts (ProviderType provider)
		{
			var providerId = GetProviderId (provider);

			return JsonSerializer.Deserialize<Dictionary<string, IAccount>> (Registrar.SecureStore?[providerId])
				?? new Dictionary<string, IAccount> ();
		}

		public void SaveAccount (ProviderType provider, string id, IAccount account)
		{
			var providerId = GetProviderId (provider);

			var accounts = JsonSerializer.Deserialize<Dictionary<string, IAccount>> (Registrar.SecureStore?[providerId]);

			if (accounts.ContainsKey(id))
				accounts[id] = account;
			else
				accounts.Add (id, account);

			Registrar.SecureStore[providerId] = JsonSerializer.Serialize (accounts, true);
		}

		public void DeleteAccount (ProviderType provider, string id)
		{
			var providerId = GetProviderId (provider);

			var accounts = JsonSerializer.Deserialize<Dictionary<string, IAccount>> (Registrar.SecureStore?[providerId]);

			if (accounts.ContainsKey(id))
				accounts.Remove (id);
			
			Registrar.SecureStore[providerId] = JsonSerializer.Serialize (accounts, true);
		}

		string GetProviderId (ProviderType provider)
		{
			switch (provider)
			{
				case ProviderType.Facebook:
					return "facebook";
				case ProviderType.Google:
					return "google";
				default:
					return provider.ToString().ToLowerInvariant ();
			}
		}
	}

	
}
