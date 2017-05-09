# Plugin.SocialAuth
Xamarin Plugin to make Authenticating with native social app SDK's easy.


**NOTE: This is currently a work in progress...**

There are a number of solutions for authenticating with providers such as Google and Facebook using either embedded webviews, default browsers, or *native* browser system API's like *SFSafariViewController*.


Google and Facebook both provider their own platform specific native SDK's for iOS and Android for authentication, and I prefer to use these in my own apps since they provide a great user login experience.

The primary goal of this plugin is to abstract the platform specific bits of implementing these SDK's as much as possible and make it easier to use them in your own apps.


## Supported Social Providers
 - Google
 	- iOS [Google Sign-In for iOS](https://developers.google.com/identity/sign-in/ios/)
 	- Android [Google Sign-In for Android](https://firebase.google.com/docs/auth/android/google-signin)
 - Facebook
   - iOS [Facebook SDK for iOS](https://developers.facebook.com/docs/ios/)
   - Android [Facebook SDK for Android](https://developers.facebook.com/docs/android/)
   
Each provider is completely optional and installed as its own NuGet package.  If you only want the native login experience for Google, only install those packages.

Here is a breakdown of the libraries/packages:

 - Shared
   - Plugin.SocialAuth
   - Plugin.SocialAuth.Facebook
   - Plugin.SocialAuth.Google
 - Android
   - Plugin.SocialAuth.Droid
   - Plugin.SocialAuth.Facebook.Droid
   - Plugin.SocialAuth.Google.Droid
 - iOS
   - Plugin.SocialAuth.iOS
   - Plugin.SocialAuth.Facebook.iOS
   - Plugin.SocialAuth.Google.iOS
 

## How does it work? 

There is a core API which you will program against in your shared code.  Whenever you need to access the account and its `AccessToken` or `IdToken` you simply call the `AuthenticateAsync (..)` method of the `SocialAuthManager` instance:

```csharp
var manager = new SocialAuthManager ();

var account = await manager.AuthenticateAsync<FacebookAccount, FacebookAuthOptions>
								(ProviderType.Facebook, new FacebookAuthOptions ());

var accessToken = account?.AccessToken;
```

### Caching and Token Refresh
The `SocialAuthManager` handles caching account information including tokens and basic profile info.

This means when you call the `AuthenticateAsync (..)` method, if there are still unexpired / valid tokens in your cache, the `Account` object will be immediately returned instead of going through the user sign in flow again.

If there is no associated account already signed in and cached, calling `AuthenticateAsync (..)` will cause the sign in flow to begin.

Both Google and Facebook SDK's additionally manage their own cache.  Plugin.SocialAuth will attempt to use these caches if its own cache is invalid.  In some cases, this means the user will not need to be asked to sign in (e.g.: Facebook can sign in automatically if you've already granted the application access with the same account on another device). 


## Platform / Provider initialization

To register the platform specific provider implementations, you will need to initialize them in your app.

### Android

```csharp
// Initialize the main platform specific lib
Plugin.SocialAuth.Droid.SocialAuth.Init (this.Application);

// Initialize any desired providers
Plugin.SocialAuth.Facebook.Droid.FacebookAuthProvider.Init ();
Plugin.SocialAuth.Google.Droid.GoogleAuthProvider.Init ();
```

### iOS

```csharp
// Initialize the main platform specific lib
global::Plugin.SocialAuth.iOS.SocialAuth.Init ();

// Initialize any desired providers
global::Plugin.SocialAuth.Facebook.iOS.FacebookAuthProvider.Init ("YOUR-FB-APPID");
global::Plugin.SocialAuth.Google.iOS.GoogleAuthProvider.Init ();
```

In addition, each provider may have its own lifecycle methods you need to relay to SocialAuth's provider implementation:

(DOCS ARE A WORK IN PROGRESS)
### iOS

#### Facebook


#### Google



### Android

#### Facebook


#### Google



## What about Twitter, Microsoft, etc?

Currently these aren't supported.  I am contemplating adding more generic auth providers using `SFSafariViewController` on iOS and `CustomTabs` on Android which would enable authentication flows via OAuth with these other providers.  There are other libraries you can use to achieve this for now.