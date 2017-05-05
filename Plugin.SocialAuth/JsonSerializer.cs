using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Plugin.SocialAuth
{
	public static class JsonSerializer
	{
		public static T Deserialize<T> (string json, bool throwOnFailure = false)
		{
			try {
				var serializer = new DataContractJsonSerializer (typeof(T));

				var bytes = System.Text.Encoding.UTF8.GetBytes (json);

				using (var ms = new MemoryStream (bytes))
					return (T)serializer.ReadObject (ms);
			}
			catch (Exception ex) {
				if (throwOnFailure)
					throw ex;
			}

			return default(T);
		}

		public static string Serialize<T> (T obj, bool throwOnFailure = false)
		{
			try {
				var serializer = new DataContractJsonSerializer (typeof(T));

				using (var ms = new MemoryStream ()) {
					serializer.WriteObject (ms, obj);
					var bytes = ms.ToArray ();
					return System.Text.Encoding.UTF8.GetString (bytes, 0, bytes.Length);
				}
			} catch (Exception ex) {
				if (throwOnFailure)
					throw ex;
			}
			return null;
		}
	}
}
