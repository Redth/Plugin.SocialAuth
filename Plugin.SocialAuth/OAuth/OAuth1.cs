using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SocialAuth.OAuth
{
	public interface IOAuth1
	{
		string ConsumerKey { get; set; }
		string ConsumerSecret { get; set; }
		Uri RequestTokenUri { get; set; }
		Uri AuthorizeUrl { get; set; }
		Uri AccessTokenUrl { get; set; }
		Uri CallbackUrl { get; set; }
		OAuth1ProcessExtrasDelegate ProcessExtras { get; set; }

		Task<Uri> GetInitialUrlAsync();
		Task<IDictionary<string, string>> GetAccessTokenAsync(string token, string tokenVerifier);
	}


	public class OAuth1AuthorizeResponse
	{
		public string Token { get; set; }
		public string TokenVerifier { get; set; }
	}

	public class OAuth1 : IOAuth1
	{
		public OAuth1()
		{
		}

		public OAuth1(IOAuth1Options options)
		{
			AuthorizeUrl = options.AuthorizeUrl;
			AccessTokenUrl = options.AccessTokenUrl;
			ConsumerKey = options.ConsumerKey;
			ConsumerSecret = options.ConsumerSecret;
			RequestTokenUri = options.RequestTokenUrl;
			CallbackUrl = options.CallbackUrl;
			ProcessExtras = options.ProcessExtras;
		}

		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
		public Uri RequestTokenUri { get; set; }
		public Uri AuthorizeUrl { get; set; }
		public Uri AccessTokenUrl { get; set; }
		public Uri CallbackUrl { get; set; }
		public OAuth1ProcessExtrasDelegate ProcessExtras { get; set; }

		string token;
		string tokenSecret;

		public string Token
		{
			get { return token; }
		}

		public string TokenSecret
		{
			get { return tokenSecret; }
		}

		public async Task<Uri> GetInitialUrlAsync()
		{
			var http = new HttpClient();

			var url = RequestTokenUri;

			var nonce = new Random().Next().ToString();
			var timestamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();

			var p = new Dictionary<string, string> {
				{ "oauth_callback", CallbackUrl.AbsoluteUri },
				{ "oauth_nonce", nonce },
				{ "oauth_timestamp", timestamp },
				{ "oauth_version", "1.0" },
				{ "oauth_consumer_key", ConsumerKey },
				{ "oauth_signature_method", "HMAC-SHA1" },
			};
			p.Add("oauth_signature", HMACSHA1Signature("GET", url, p, ConsumerSecret, ""));

			var content = await new FormUrlEncodedContent(p).ReadAsStringAsync();

			var requestUrl = RequestTokenUri.AbsoluteUri.TrimEnd('?') + "?" + content;

			var resp = await http.GetStringAsync(requestUrl);

			var items = WebUtil.FormDecode(resp);

			token = items["oauth_token"];
			tokenSecret = items["oauth_token_secret"];

			var paramType = AuthorizeUrl.AbsoluteUri.IndexOf("?") >= 0 ? "&" : "?";

			return new Uri(AuthorizeUrl.AbsoluteUri + paramType + "oauth_token=" + Uri.EscapeDataString(token));
		}

		public OAuth1AuthorizeResponse ParseCallback(Uri url)
		{
			var resp = new OAuth1AuthorizeResponse();

			var r = WebUtil.FormDecode(url.Query);

			string verifier;
			if (r.TryGetValue("oauth_verifier", out verifier))
				resp.TokenVerifier = verifier;

			string t;
			if (r.TryGetValue("oauth_token", out t))
				resp.Token = t;

			return resp;
		}

		public Task<IDictionary<string, string>> GetAccessTokenAsync(OAuth1AuthorizeResponse authResponse)
		{
			return GetAccessTokenAsync(authResponse.Token, authResponse.TokenVerifier);
		}

		public async Task<IDictionary<string, string>> GetAccessTokenAsync(string token, string tokenVerifier)
		{
			var http = new HttpClient();

			var url = AccessTokenUrl;

			var nonce = new Random().Next().ToString();
			var timestamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();

			var p = new Dictionary<string, string> {
				{ "oauth_callback", CallbackUrl.AbsoluteUri },
				{ "oauth_nonce", nonce },
				{ "oauth_timestamp", timestamp },
				{ "oauth_version", "1.0" },
				{ "oauth_consumer_key", ConsumerKey },
				{ "oauth_signature_method", "HMAC-SHA1" },
				{ "oauth_token", token },
			};
			if (!string.IsNullOrEmpty(tokenVerifier))
				p.Add("oauth_verifier", tokenVerifier);

			p.Add("oauth_signature", HMACSHA1Signature("GET", url, p, ConsumerSecret, tokenSecret));

			var content = await new FormUrlEncodedContent(p).ReadAsStringAsync();

			var requestUrl = AccessTokenUrl.AbsoluteUri.TrimEnd('?') + "?" + content;

			var resp = await http.GetStringAsync(requestUrl);

			var items = WebUtil.FormDecode(resp);

			return items;
		}


		public TOAuth1Account GetAccountFromResponse<TOAuth1Account>(IDictionary<string, string> parameters)
			where TOAuth1Account : IOAuth1Account
		{
			if (parameters == null) return default(TOAuth1Account);

			if (parameters.ContainsKey("oauth_token"))
			{
				var account = (TOAuth1Account)Activator.CreateInstance(typeof(TOAuth1Account));

				foreach (var kvp in parameters)
				{
					if (kvp.Key == "oauth_token")
						account.AccessToken = kvp.Value;
					else if (kvp.Key == "oauth_token_secret")
						account.TokenSecret = kvp.Value;
					else
						account[kvp.Key] = kvp.Value;
				}

				ProcessExtras?.Invoke(parameters, account);

				return account;
			}

			return default(TOAuth1Account);
		}



		public string GetAuthorizationHeader(string method, Uri url)
		{
			var nonce = new Random().Next().ToString();
			var timestamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();

			var p = new Dictionary<string, string> {
				{ "oauth_callback", CallbackUrl.AbsoluteUri },
				{ "oauth_nonce", nonce },
				{ "oauth_timestamp", timestamp },
				{ "oauth_version", "1.0" },
				{ "oauth_consumer_key", ConsumerKey },
				{ "oauth_signature_method", "HMAC-SHA1" },
				{ "oauth_token", token },
			};
			p.Add("oauth_signature", HMACSHA1Signature(method, url, p, ConsumerSecret, tokenSecret));

			var sb = new StringBuilder();
			sb.Append("OAuth ");

			var head = "";
			foreach (var item in p)
			{
				if (item.Key.StartsWith("oauth_", StringComparison.Ordinal))
				{
					sb.Append(head);
					sb.AppendFormat("{0}=\"{1}\"", WebUtil.UrlEncodeRfc5894(item.Key), WebUtil.UrlEncodeRfc5894(item.Value));
					head = ",";
				}
			}

			return sb.ToString();
		}


		public static string HMACSHA1Signature(string method, Uri uri, IDictionary<string, string> parameters, string consumerSecret, string tokenSecret)
		{
			var b = new StringBuilder();
			b.Append(method);
			b.Append("&");
			b.Append(WebUtil.UrlEncodeRfc5894(uri.AbsoluteUri));
			b.Append("&");
			var head = "";
			foreach (var k in parameters.Keys.OrderBy(x => x))
			{
				var p = head + WebUtil.UrlEncodeRfc5894(k) + "=" + WebUtil.UrlEncodeRfc5894(parameters[k]);
				b.Append(WebUtil.UrlEncodeRfc5894(p));
				head = "&";
			}

			var key = WebUtil.UrlEncodeRfc5894(consumerSecret) + "&" + WebUtil.UrlEncodeRfc5894(tokenSecret);
			var hashAlgo = new HMACSHA1(Encoding.ASCII.GetBytes(key));
			var hash = hashAlgo.ComputeHash(Encoding.ASCII.GetBytes(b.ToString()));
			var sig = Convert.ToBase64String(hash);
			return sig;
		}
	}
}
