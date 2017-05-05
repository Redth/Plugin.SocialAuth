using System;
namespace Plugin.SocialAuth
{
	public interface ISecureStore
	{
		string this[string key] { get; set; }
	}
}
