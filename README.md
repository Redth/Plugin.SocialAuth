# Plugin.SocialAuth

Xamarin Social Authentication with Native SDK's and OAuth through SFSafariViewController and Android Custom Tabs.

**NOTE: This is currently a work in progress...**

Social authentication in mobile apps has evolved since the beginning of Xamarin.  Providers such as Google and Facebook offer native SDK's to help deliver a delightful user experience when authentication with their platforms.

iOS and Android have both recently provided their own extensions of the operating system browser (`SFSafariViewController` and `CustomTabs`) and now prefer (and in some cases such as Google, require) that you use them for enabling social authentication in your apps. 

Plugin.SocialAuth aims to provide you with the latest and greatest options for adding social authentication to your Xamarin apps as easily as possible.


### What's missing?

 - NuGet packages
 - Automatic handling of refreshing of access tokens in OAuth2
 - More known provider support with specific authentication implementations (Fitbit, Meetup, etc).


## Supported Providers

You can pick and choose which providers you want to use on each platform.

### iOS

| Provider        | Implementation                 | NuGet Package                  |
|-----------------|--------------------------------|--------------------------------|
| Facebook        | LoginKit Native SDK*           | Plugin.SocialAuth.Facebook.iOS |
| Google          | Sign-In Native SDK             | Plugin.SocialAuth.Google.iOS   |
| Twitter         | OAuth `SFSafariViewController` | Plugin.SocialAuth.Twitter.iOS  |
| OAuth1 & OAuth2 | OAuth `SFSafariViewController` | Plugin.Social.Auth.iOS         |


### Android

| Provider        | Implementation                        | NuGet Package                      |
|-----------------|---------------------------------------|------------------------------------|
| Facebook        | Native Android SDK*                   | Plugin.SocialAuth.Facebook.Droid   |
| Google          | Google Play services Auth SDK*        | Plugin.SocialAuth.Google.Droid     |
| Twitter         | OAuth Android Support CustomTabs SDK* | Plugin.SocialAuth.Twitter.Droid    |
| OAuth1 & OAuth2 | OAuth Android Support CustomTabs SDK* | Plugin.SocialAuth.Droid.CustomTabs |

[*] All of the Native SDK Provider options are packaged as separate NuGet packages in case you don't need them in your app (or want to use one of the OAuth providers instead of the Native SDK implementation), so that you don't incur their size or dependencies in such a case.




## How does it work? 

The core API is available in a `.NET Standard 1.4` library via NuGet.

There is a shared instance of `SocialAuthManager` which you can access to authenticate.  Here is an example of invoking the Native Facebook SDK authentication flow:

```csharp
// See if we previously logged in with any particular account
var existingAccount = SocialAuthManager.Current.AccountStore.FindAnyAccount(ProviderType.Facebook);

// Authenticate with Facebook's native SDK
var account = await SocialAuthManager.Current.AuthenticateFacebookAsync(new FacebookAuthOptions(), existingAccount?.Id);
```

In the above sample, we are first trying to look up _any_ existing Facebook account from our local `AccountStore` in order to pass the `Id` into the Authentication call.  

Plugin.SocialAuth takes care of caching authenticated accounts for you so that future Authentication calls will avoid the UI authorization flow, if the cached account is unexpired and valid.

We can just omitt the `Id` in the Authentication call if we don't care to look in the local cache first.



## Setup / Init

Before we can actually use the sample above, and call the Authentication methods, we need to do some setup.

Additionally each provider and platform may require specific application configuration.


### iOS

In your `AppDelegate`'s `FinishedLaunching` method we need to add some calls to Init and Register our providers:

```csharp
// Init the things
SocialAuth.Init();
FacebookAuthProvider.Init(SocialKeys.FACEBOOK_APP_ID;
```

Some providers like Facebook require a specific `Init` call like in the first block above, while others do not.

Next, we need to register the different provider _implementations_ we would like to associate with the provider types:

```csharp
//Register custom handlers
SocialAuthManager.Current.RegisterProvider<FacebookAuthProvider>(ProviderType.Facebook);
SocialAuthManager.Current.RegisterProvider<GoogleAuthProvider>(ProviderType.Google);
SocialAuthManager.Current.RegisterProvider<TwitterSafariServicesAuthProvider>(ProviderType.Twitter);
SocialAuthManager.Current.RegisterProvider<OAuth2SafariServicesProvider>("fitbit");
```

You can see it's possible to use a custom `providerTypeId`, but the `ProviderType` enum will contain most common providers.

Notice the generic type argument of the `RegisterProvider` call contains the _implementation_ type that will be used when the authentication call is made for the given provider type.

This means it's possible to register an `OAuth2SafariServicesProvider` as the implementation for `ProviderType.Facebook` if you'd rather use `SFSafariViewController` instead of the Native Facebook SDK.

Some providers will require relaying calls from your `AppDelegate.OpenUrl` override to `SocialAuthManager.OpenUrl`.  You should add the following code to your `AppDelegate`:

```csharp
// We need to relay open url calls to social auth
public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
{
	if (Plugin.SocialAuth.iOS.SocialAuth.OpenUrl(application, url, sourceApplication, annotation))
		return true;

	return false;
}
```

### Android
Android is very similar to iOS, but instead, you should call your `Init`'s and `RegisterProvider`'s in the `MainActivity` of your app.

Android also requires that we relay our current `Activity`'s `OnActivityResult` (if you're using Xamarin.Forms this should be in your `MainActivity`):

```csharp
// Pass OnActivityResult back into social auth
protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
	base.OnActivityResult(requestCode, resultCode, data);

	Plugin.SocialAuth.Droid.SocialAuth.OnActivityResult(requestCode, resultCode, data);
}
```

Also, in some cases where we will receive callbacks into our own application (eg: in the case of `OAuth1CustomTabsAuthProvider` and `OAuth2CustomTabsProvider`), we need to setup an `Activity` subclassing `Plugin.SocialAuth.Droid.CustomTabs.OAuthCallbackActivity` which registers itself with an `IntentFilter` handling the `DataScheme` of your Callback/Redirect URL that you are using with your OAuth provider.  Here's an example that registers the scheme `plugin.socialauth://` and assumes you're passing something like `plugin.socialauth://twitter` to your authentication provider as your callback/redirect URL:

```csharp
// We need this callback to redirect callbacks into our app to SocialAuth to handle
[Activity(NoHistory = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
[IntentFilter(new[] { Intent.ActionView },
			  Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "plugin.socialauth")]
public class SocialAuthOAuth1CallbackActivity : Plugin.SocialAuth.Droid.CustomTabs.OAuthCallbackActivity
{
}
```


## Provider Specific Setup

Each social provider will have specific guides to setting up keys/credentials and in some cases your app configuration for iOS/Android in order to make authentication work.  

### Facebook

You will need to visit [Facebook for Developers](https://developers.facebook.com/) to setup an Application.  Additionally there are instructions for setting up iOS and Android app login:

 - [Facebook Login for iOS Quickstart](https://developers.facebook.com/docs/facebook-login/ios)
 - [Facebook Login for Android](https://developers.facebook.com/docs/facebook-login/android)

NOTE: Facebook Login for iOS requires that you add specific URL schemes to your _Info.plist_ file.

NOTE: Facebook Login for Android requires that you add some specific items to your _AndroidManifest.xml_ file.  Additionally you will need to provide `.keystore` hashes in the Facebook Developer portal under your Android App settings.  Be sure to add hashes for your _debug.keystore_ as well as the _.keystore_ used for signing Play store builds.


### Google

You will need to visit [Google API Developer Console](https://console.developers.google.com/) to create an application and Credentials to use in your apps.  There are specific guides for iOS and Android to setting up your app as well:

 - [Google Sign-In for iOS](https://developers.google.com/identity/sign-in/ios/start-integrating)
 - [Google Sign-In for Android](https://developers.google.com/identity/sign-in/android/start-integrating)

NOTE: Google for iOS requires that you add some specific items to your _Info.plist_ file.  Additionally you will need to add a `GoogleService-Info.plist` in your iOS app project.

NOTE: Google for Android is a bit tricky when it comes to setting up your keys in the Google API Developer Console.  You will want to setup an Android key credential for the `ClientId` but for the `ServerClientId` you will want to setup an OAuth web application type credential.



## Caching and Token Refresh
The `SocialAuthManager` handles caching account information including tokens and in some provider cases, basic profile info, if you pass in the `Id` of the `IAccount` returned from a previous Authentication call.  Omitting this parameter will cause the cache to be skipped.

This means when you call the `AuthenticateAsync (..)` method, if there are still unexpired / valid tokens in your cache, the `Account` object will be immediately returned instead of going through the user sign in flow again.

If there is no associated account already signed in and cached, calling `AuthenticateAsync (..)` will cause the sign in flow to begin.

Both Google's and Facebook's Native SDKs additionally manage their own cache.  Plugin.SocialAuth will attempt to use these caches if its own cache is invalid.  In some cases, this means the user will not need to be asked to sign in (e.g.: Facebook can sign in automatically if you've already granted the application access with the same account on another device). 

In the case of OAuth2 providers, if there is a `refresh_token` in the cache and the `access_token` is expired, a call to refresh the access token will be attempted.  If successful, the UI authentication flow can be avoided and the new access token and refresh tokens will be cached for future use.


### Security

On both iOS and Android, account caches are stored using platform specific secure storage API's.


## License

Copyright (c) 2017 Jonathan Dick

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

