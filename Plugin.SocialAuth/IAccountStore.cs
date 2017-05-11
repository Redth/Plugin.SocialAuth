using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Plugin.SocialAuth
{
	public interface IAccountStore
	{
		IAccount FindAccount(string providerTypeId, string id);
		IEnumerable<IAccount> FindAccounts(ProviderType providerType);
		IEnumerable<IAccount> FindAccounts(string providerTypeId);
		IAccount FindAnyAccount(ProviderType providerType);
		IAccount FindAnyAccount(string providerTypeId);
		void SaveAccount(string providerTypeId, IAccount account);
		void DeleteAccount(string providerTypeId, string id);

		ISecureStore SecureStore { get; set; }
	}

}
