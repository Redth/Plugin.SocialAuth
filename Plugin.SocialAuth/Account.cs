using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Plugin.SocialAuth
{

	[DataContract (Name="account")]
	public class Account : IAccount
	{
		[DataMember(Name="id")]
		public string Id { get; set; }
		[DataMember (Name="name")]
		public string Name { get;set; }

		[DataMember (Name="accessToken")]
		public string AccessToken { get; set; }
		[DataMember (Name="refreshToken")]
		public string RefreshToken { get;set; }
		[DataMember (Name="idToken")]
		public string IdToken { get; set; }

		[DataMember (Name="accessTokenExpires")]
		public DateTime? AccessTokenExpires { get; set; }
		[DataMember (Name="refreshTokenExpires")]
		public DateTime? RefreshTokenExpires { get; set; }
		[DataMember (Name="idTokenExpires")]
		public DateTime? IdTokenExpires { get; set; }
	}
}
