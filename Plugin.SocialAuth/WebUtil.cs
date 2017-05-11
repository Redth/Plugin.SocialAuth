using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Plugin.SocialAuth
{
	public static class WebUtil
	{
		public static IDictionary<string, string> FormDecode(string encodedString)
		{
			var inputs = new Dictionary<string, string>();

			if (encodedString.StartsWith("?", StringComparison.OrdinalIgnoreCase)
				|| encodedString.StartsWith("#", StringComparison.OrdinalIgnoreCase))
				encodedString = encodedString.Substring(1);

			var parts = encodedString.Split('&');
			foreach (var p in parts)
			{
				var kv = p.Split('=');
				var k = Uri.UnescapeDataString(kv[0]);
				var v = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : "";
				inputs[k] = v;
			}

			return inputs;
		}


		// URL Encoding as in : http://tools.ietf.org/html/rfc5849#section-3.6
		public static string UrlEncodeRfc5894(string unencoded)
		{
			var utf8 = System.Text.Encoding.UTF8.GetBytes(unencoded);
			var sb = new System.Text.StringBuilder();

			for (var i = 0; i < utf8.Length; i++)
			{
				var v = utf8[i];
				if ((0x41 <= v && v <= 0x5A) || (0x61 <= v && v <= 0x7A) || (0x30 <= v && v <= 0x39) ||
					v == 0x2D || v == 0x2E || v == 0x5F || v == 0x7E)
				{
					sb.Append((char)v);
				}
				else
				{
					sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "%{0:X2}", v);
				}
			}

			return sb.ToString();
		}

		public static IDictionary<string, string> JsonDecode(string encodedString)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(encodedString);
		}

		public static bool CanHandleCallback(Uri expectedUrl, Uri callbackUrl)
		{
			if (!callbackUrl.Scheme.Equals(expectedUrl.Scheme, StringComparison.OrdinalIgnoreCase))
				return false;

			if (!string.IsNullOrEmpty(expectedUrl.Host))
			{
				if (!callbackUrl.Host.Equals(expectedUrl.Host, StringComparison.OrdinalIgnoreCase))
					return false;
			}

			return true;
		}
	}
}
