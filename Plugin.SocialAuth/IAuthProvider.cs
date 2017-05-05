using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SocialAuth
{
	public interface IAuthProvider<TAccount, TOptions>
		where TAccount : IAccount
		where TOptions : IAuthOptions
	{
		Task<TAccount> AuthenticateAsync (TOptions options);
		Task LogoutAsync ();
	}
}
