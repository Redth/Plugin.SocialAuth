using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Plugin.SocialAuth
{
	public interface IAccountStore
	{
		IAccount FindAccount (ProviderType provider, string id);
		IDictionary<string, IAccount> FindAccounts (ProviderType provider);
		void SaveAccount (ProviderType provider, string id, IAccount account);
		void DeleteAccount (ProviderType provider, string id);
	}
	
}
