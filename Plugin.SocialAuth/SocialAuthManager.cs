using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.SocialAuth.OAuth;

namespace Plugin.SocialAuth
{
	public enum ProviderType
	{
		OAuth1 = 0,
		OAuth2 = 1,
		Facebook = 2,
		Google = 3,
		Twitter = 4,
		Microsoft = 5,
	}

	public interface ISocialAuthManager
	{
		Task<TAccount> AuthenticateAsync<TAccount, TOptions>(string providerTypeId, TOptions options, string accountId = null)
			where TAccount : IAccount
			where TOptions : IAuthOptions;

		Task LogoutAsync<TAccount, TOptions>(string providerTypeId, TAccount account)
			where TOptions : IAuthOptions
			where TAccount : IAccount;

		void RegisterProvider<TProviderImpl>(ProviderType providerType)
			where TProviderImpl : class;

		void RegisterProvider<TProviderImpl>(string providerTypeId)
			where TProviderImpl : class;

		void UnregisterProvider(ProviderType providerType);

		void UnregisterProvider(string providerTypeId);

		Type GetRegisteredProvider(string providerTypeId);

		IEnumerable<Type> RegisteredProviderTypes { get; }
	}

	public class SocialAuthManager : ISocialAuthManager
	{
		static SocialAuthManager current = null;
		public static SocialAuthManager Current
		{
			get
			{
				if (current == null)
					current = new SocialAuthManager();

				return current;
			}
		}

		public static string GetProviderTypeId(ProviderType providerType)
		{
			switch (providerType)
			{
				case ProviderType.Facebook:
					return "facebook";
				case ProviderType.Google:
					return "google";
				case ProviderType.Twitter:
					return "twitter";
				default:
					return providerType.ToString().ToLowerInvariant();
			}
		}

		public SocialAuthManager(IAccountStore accountStore = null)
		{
			AccountStore = accountStore ?? new AccountStore();
		}

		public IAccountStore AccountStore { get; set; }
		public ISecureStore SecureStore { get; set; }

		public Task<TAccount> AuthenticateAsync<TAccount, TOptions>(ProviderType providerType, TOptions options, string accountId = null)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			return AuthenticateAsync<TAccount, TOptions>(GetProviderTypeId(providerType), options, accountId);
		}

		public async Task<TAccount> AuthenticateAsync<TAccount, TOptions>(string providerTypeId, TOptions options, string accountId = null)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			var account = AccountStore.FindAccount(providerTypeId, accountId);

			// Check if an eccount already exists, is the right account type,
			// and has good tokens in it.
			if (account != null && account is TAccount && await account.CheckValidity())
				return (TAccount)account;

			var implType = GetRegisteredProvider(providerTypeId);

			if (implType == null)
				throw new Exception($"No Providers are registered for Provider Type: {providerTypeId}");

			var impl = GetInstance<TAccount, TOptions>(implType);

			account = await impl.AuthenticateAsync(options);

			// If the user passed in an ID to use, let's set that on the account
			if (!string.IsNullOrEmpty(accountId))
				account.Id = accountId;

			if (!string.IsNullOrEmpty(accountId ?? account.Id))
				AccountStore.SaveAccount(providerTypeId, account);

			return (TAccount)account;
		}

		public Task LogoutAsync<TAccount, TOptions>(ProviderType providerType, TAccount account)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			return LogoutAsync<TAccount, TOptions>(GetProviderTypeId(providerType), account);
		}

		public Task LogoutAsync<TAccount, TOptions>(string providerTypeId, TAccount account)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			var implType = GetRegisteredProvider(providerTypeId);

			if (implType == null)
				return Task.FromResult<object>(null);

			var impl = GetInstance<TAccount, TOptions>(implType);

			AccountStore.DeleteAccount(providerTypeId, account.Id);

			return impl.LogoutAsync();
		}

		// This is a weakly typed storage, probably not the best idea
		//internal Dictionary<Type, object> instances { get; set; } 
		//= new Dictionary<Type, object> ();

		IAuthProvider<TAccount, TOptions> GetInstance<TAccount, TOptions>(Type type)
			where TOptions : IAuthOptions
			where TAccount : IAccount
		{
			//if (instances.ContainsKey (type))
			//return (IAuthProvider<TAccount, TOptions>)instances[type];

			var obj = Activator.CreateInstance(type);

			var impl = (IAuthProvider<TAccount, TOptions>)obj;

			//instances.Add (type, impl);

			return impl;
		}

		Dictionary<string, Type> authProviders
			= new Dictionary<string, Type>();

		public void RegisterProvider<TProviderImpl>(ProviderType providerType)
			where TProviderImpl : class
		{
			RegisterProvider<TProviderImpl>(GetProviderTypeId(providerType));
		}
		public void RegisterProvider<TProviderImpl>(string providerTypeId)
			where TProviderImpl : class
		{
			if (authProviders.ContainsKey(providerTypeId))
				throw new Exception($"Another Provider is already registered for the Provider Type: {providerTypeId}");

			authProviders.Add(providerTypeId, typeof(TProviderImpl));
		}

		public void UnregisterProvider(ProviderType providerType)
		{
			UnregisterProvider(SocialAuthManager.GetProviderTypeId(providerType));
		}

		public void UnregisterProvider(string providerTypeId)
		{
			if (authProviders.ContainsKey(providerTypeId))
				authProviders.Remove(providerTypeId);
		}

		public Type GetRegisteredProvider(string providerTypeId)
		{
			if (!authProviders.ContainsKey(providerTypeId))
				return null;

			return authProviders[providerTypeId];
		}

		public IEnumerable<Type> RegisteredProviderTypes
		{
			get
			{
				return authProviders.Values;
			}
		}
	}
}
