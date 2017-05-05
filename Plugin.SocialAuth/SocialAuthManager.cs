using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SocialAuth
{
	public enum ProviderType
	{
		Facebook,
		Google,
	}

	public class SocialAuthManager
	{
		public SocialAuthManager ()
		{
			AccountStore = new AccountStore ();
		}

		public SocialAuthManager (IAccountStore accountStore)
		{
			AccountStore = accountStore;
		}

		public IAccountStore AccountStore { get; private set; }

		public Task<TAccount> AuthenticateAsync<TAccount, TOptions> (ProviderType providerType, TOptions options, string accountId = null)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			var account = AccountStore.FindAccount (providerType, accountId);

			// Check if an eccount already exists, is the right account type,
			// and has good tokens in it.
			if (account != null && account is TAccount && checkAccount (account))
				return Task.FromResult ((TAccount)account);
			
			var implType = Registrar.Find (providerType)?.FirstOrDefault ();

			var impl = GetInstance<TAccount, TOptions> (implType);

			return impl.AuthenticateAsync (options);
		}

		public Task LogoutAsync<TAccount, TOptions> (ProviderType providerType, TAccount account)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			var implType = Registrar.Find(providerType)?.FirstOrDefault ();

			var impl = GetInstance<TAccount, TOptions> (implType);

			AccountStore.DeleteAccount (providerType, account.Id);

			return impl.LogoutAsync ();
		}

		// This is a weakly typed storage, probably not the best idea
		Dictionary<Type, object> instances { get; set; } 
			= new Dictionary<Type, object> ();

		IAuthProvider<TAccount, TOptions> GetInstance<TAccount, TOptions> (Type type)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			if (instances.ContainsKey (type))
				return (IAuthProvider<TAccount, TOptions>)instances[type];

			var obj = Activator.CreateInstance(type);

			var impl = (IAuthProvider<TAccount, TOptions>)obj;

			instances.Add (type, impl);

			return impl;
		}

		bool checkAccount (IAccount account)
		{
			// Missing account or no tokens? login again.
			if (account == null)
				return false;

			// Missing access token AND id token, so no tokens? login again.
			if (string.IsNullOrEmpty (account.AccessToken) && string.IsNullOrEmpty (account.IdToken))
				return false;

			// Access Token has expiry date and is expired? login again.
			if (account.AccessTokenExpires.HasValue && account.AccessTokenExpires.Value.ToUniversalTime () >= DateTime.UtcNow)
				return false;

			// Id Token has expiry date and is expired? login again.
			if (account.IdTokenExpires.HasValue && account.IdTokenExpires.Value.ToUniversalTime () >= DateTime.UtcNow)
				return false;

			// Otherwise, existing tokens should be ok
			return true;
		}

	}
}
