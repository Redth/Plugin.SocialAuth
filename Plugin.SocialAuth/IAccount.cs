using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SocialAuth
{

	public interface IAccount : IDictionary<string, string>
	{
		string Id { get; set; }
		string Name { get; set; }

		Task<bool> CheckValidity();
	}
}
