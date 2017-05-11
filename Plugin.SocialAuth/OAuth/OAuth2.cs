using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Plugin.SocialAuth.OAuth
{
	public interface IOAuth2
	{
		string ClientId { get; set; }
		string ClientSecret { get; set; }
		string Scope { get; set; }
		Uri AuthorizeUrl { get; set; }
		Uri AccessTokenUrl { get; set; }
		Uri RedirectUrl { get; set; }

		bool SendAccessTokenRequestAuthHeader { get; set; }

		Task<Uri> GetInitialUrlAsync();
		Task<IDictionary<string, string>> GetAccessTokenAsync(string code);
	}

	public class OAuth2 : IOAuth2
	{
		public OAuth2()
		{
			var chars = new char[16];
			var rand = new Random();
			for (var i = 0; i < chars.Length; i++)
				chars[i] = (char)rand.Next((int)'a', (int)'z' + 1);
			requestState = new string(chars);
		}

		public OAuth2(IOAuth2Options options) : this()
		{
			AuthorizeUrl = options.AuthorizeUrl;
			AccessTokenUrl = options.AccessTokenUrl;
			ClientId = options.ClientId;
			ClientSecret = options.ClientSecret;
			Scope = options.Scope;
			RedirectUrl = options.RedirectUrl;
			SendAccessTokenRequestAuthHeader = options.SendAccessTokenRequestAuthHeader;
			ProcessExtras = options.ProcessExtras;
		}

		string requestState;

		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Scope { get; set; }
		public Uri AuthorizeUrl { get; set; }
		public Uri AccessTokenUrl { get; set; }
		public Uri RedirectUrl { get; set; }
		public bool SendAccessTokenRequestAuthHeader { get; set; } = false;
		public OAuth2ProcessExtrasDelegate ProcessExtras { get; set; }

		public Task<Uri> GetInitialUrlAsync()
		{
			var url = new Uri(string.Format(
				"{0}?client_id={1}&redirect_uri={2}&response_type={3}&scope={4}&state={5}",
				AuthorizeUrl.AbsoluteUri,
				Uri.EscapeDataString(ClientId),
				Uri.EscapeDataString(RedirectUrl.AbsoluteUri),
				AccessTokenUrl == null ? "token" : "code",
				Uri.EscapeDataString(Scope),
				Uri.EscapeDataString(requestState)));

			return Task.FromResult(url);
		}

		public async Task<IDictionary<string, string>> GetAccessTokenAsync(string code)
		{
			var queryValues = new Dictionary<string, string> {
				{ "grant_type", "authorization_code" },
				{ "code", code },
				{ "redirect_uri", RedirectUrl.AbsoluteUri },
				{ "client_id", ClientId },
			};
			if (!string.IsNullOrEmpty(ClientSecret))
				queryValues["client_secret"] = ClientSecret;

			var http = new HttpClient();

			var content = new FormUrlEncodedContent(queryValues);

			var req = new HttpRequestMessage(HttpMethod.Post, AccessTokenUrl)
			{
				Content = new FormUrlEncodedContent(queryValues)
			};

			if (SendAccessTokenRequestAuthHeader)
			{

				var unencodedAuth = ClientId + ":" + ClientSecret;
				var encAuth = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(unencodedAuth));

				req.Headers.Authorization = new AuthenticationHeaderValue("Basic", encAuth);
			}

			var resp = await http.SendAsync(req);

			var text = await resp.Content.ReadAsStringAsync();

			// Parse the response
			var data = text.Contains("{") ? WebUtil.JsonDecode(text) : WebUtil.FormDecode(text);

			if (data.ContainsKey("error"))
				throw new Exception("Error authenticating: " + data["error"]);
			if (data.ContainsKey("errors"))
				throw new Exception("Error authenticating: " + data["errors"]);
			if (data.ContainsKey("access_token"))
				return data;

			throw new Exception("Expected access_token in access token response, but did not receive one.");
		}

		public TOAuth2Account GetAccountFromResponse<TOAuth2Account>(IDictionary<string, string> parameters)
			where TOAuth2Account : IOAuth2Account
		{
			if (parameters == null) return default(TOAuth2Account);

			var account = (TOAuth2Account)Activator.CreateInstance(typeof(TOAuth2Account));

			foreach (var kvp in parameters)
			{
				if (kvp.Key == "access_token")
					account.AccessToken = kvp.Value;
				else if (kvp.Key == "expires_in")
				{
					int seconds = -1;
					if (int.TryParse(kvp.Value, out seconds))
						account.AccessTokenExpires = DateTime.UtcNow.AddSeconds(seconds);
				}
				else if (kvp.Key == "refresh_token")
					account.RefreshToken = kvp.Value;
				else if (kvp.Key == "token_type")
					account.TokenType = kvp.Value;
				else if (kvp.Key == "scope")
					account.Scope = kvp.Value;
				else
					account[kvp.Key] = kvp.Value;
			}

			return account;
		}

		public async Task<TOAuth2Account> ParseCallbackAsync<TOAuth2Account>(Uri url)
			where TOAuth2Account : IOAuth2Account
		{
			var query = WebUtil.FormDecode(url.Query);
			var fragment = WebUtil.FormDecode(url.Fragment);

			// Validate the state in the server response matches our request
			var state = query?["state"];
			if ((requestState ?? "") != (state ?? ""))
				throw new Exception("Invalid state returned from server.");

			var account = default(TOAuth2Account);

			if (fragment?.ContainsKey("access_token") ?? false)
			{
				account = GetAccountFromResponse<TOAuth2Account>(fragment);

				ProcessExtras?.Invoke(fragment, account);
			}
			else if (AccessTokenUrl != null)
			{
				if (query.ContainsKey("code"))
				{
					var code = query["code"];

					var items = await GetAccessTokenAsync(code);

					account = GetAccountFromResponse<TOAuth2Account>(items);

					ProcessExtras?.Invoke(items, account);
				}
			}

			return account;
		}
	}
}
