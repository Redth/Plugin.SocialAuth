using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SocialAuth
{
	public interface IAccount
	{
		string Id { get; set; }
		string Name { get; set; }

		string AccessToken { get; set; }
		string RefreshToken { get; set; }
		string IdToken { get; set; }

		DateTime? AccessTokenExpires { get; set; }
		DateTime? RefreshTokenExpires { get; set; }
		DateTime? IdTokenExpires { get; set; }
	}
}
