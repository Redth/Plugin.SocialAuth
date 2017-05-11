using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Plugin.SocialAuth
{
	public abstract class Account : Dictionary<string, string>, IAccount
	{
		public string Id
		{
			get { return GetStringValue("id"); }
			set { SetStringValue("id", value); }
		}

		public string Name
		{
			get { return GetStringValue("name"); }
			set { SetStringValue("name", value); }
		}

		public abstract Task<bool> CheckValidity();

		protected string GetStringValue(string key, string defaultValue = null)
		{
			string r;
			if (!TryGetValue(key, out r))
				r = defaultValue;
			return r;
		}

		protected void SetStringValue(string key, string value = null)
		{
			if (ContainsKey(key))
				this[key] = value;
			else
				Add(key, value);
		}

		protected DateTime? GetDateTimeValue(string key, DateTime? defaultValue = null)
		{
			DateTime r;

			var str = GetStringValue(key);

			if (string.IsNullOrEmpty(str))
				return defaultValue;

			if (DateTime.TryParse(str, out r))
				return r;

			return defaultValue;
		}

		protected void SetDateTimeValue(string key, DateTime? value = null)
		{
			if (!value.HasValue)
				SetStringValue(key);
			else
			{
				var t = value.Value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				var s = (int)t.TotalSeconds;
				SetStringValue(key, s.ToString());
			}
		}

		protected long? GetLongValue(string key, long? defaultValue = null)
		{
			long r;

			var str = GetStringValue(key);

			if (string.IsNullOrEmpty(str))
				return defaultValue;

			if (long.TryParse(str, out r))
				return r;

			return defaultValue;
		}

		protected void SetLongValue(string key, long? value = null)
		{
			if (!value.HasValue)
				SetStringValue(key);
			else
				SetStringValue(key, value.Value.ToString());
		}

		protected bool? GetBoolValue(string key, bool? defaultValue = null)
		{
			bool r;

			var str = GetStringValue(key);

			if (string.IsNullOrEmpty(str))
				return defaultValue;

			if (bool.TryParse(str, out r))
				return r;

			return defaultValue;
		}

		protected void SetBoolValue(string key, bool? value = null)
		{
			if (!value.HasValue)
				SetStringValue(key);
			else
				SetStringValue(key, value.Value.ToString());
		}
	}
}
