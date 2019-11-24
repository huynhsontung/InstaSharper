# InstagramApi [InstaSharper] [Forked]
Tokenless, butthurtless private API for Instagram. Get account information, media, explore tags and user feed without any applications and other crap.

### This fork is under active development with new features coming soon

### Before posting new issues: [Demo samples](https://github.com/a-legotin/InstaSharper/tree/develop/InstaSharper.Examples), [Tests project](https://github.com/a-legotin/InstaSharper/tree/develop/InstaSharper.Tests/Endpoints) and [Wiki page](https://github.com/a-legotin/InstaSharper/wiki/How-to-use-library-features)

Note that: there is a simple [Instagram API](https://github.com/a-legotin/InstagramAPI-Web) based on web-version of Instagram. This repository based on Instagram API for mobile devices.

[![MyGet](https://img.shields.io/myget/instashaper-develop/v/InstaSharper.svg)](https://www.myget.org/feed/instashaper-develop/package/nuget/InstaSharper)

#### Current version: 1.4.0 [Stable], 2.0.0 [Under development]

## Overview
This project intends to provide all the features available in the Instagram API. It is being developed in C# for .NET Standard 2.0

#### This repository is provided for reference purposes only.

* Please note that this project is still in design and development phase; the libraries may suffer major changes even at the interface level, so do not rely on this software for production uses yet. *

## Cross-platform by design
Build with .NET Standard. Can be used on Mac, Linux, Windows.

## Easy to install
Use library as dll, reference from [myget](https://www.myget.org/feed/instashaper-develop/package/nuget/InstaSharper) or clone source code.

## Features

Currently the library supports following coverage of the following Instagram APIs:

***

- [x] Login
- [x] Logout
- [x] Create new account
- [x] Get user explore feed
- [x] Get user timeline feed
- [x] Get all user media by username
- [x] Get media by its id
- [x] Get user info by its user name
- [x] Get current user info
- [x] Get tag feed by tag value
- [x] Get current user media
- [x] Get followers list
- [x] Get followers list for currently logged in user
- [x] Get following list
- [x] Get recent following activity
- [x] Get user tags by username
- [x] Get direct mailbox
- [x] Get recent recipients
- [x] Get ranked recipients
- [x] Get inbox thread
- [x] Get recent activity
- [x] Like media
- [x] Unlike media
- [x] Follow user
- [x] Unfollow user
- [x] Set account private
- [x] Set account public
- [x] Send comment
- [x] Delete comment
- [x] Upload photo
- [x] Upload video
- [x] Get followings list
- [x] Delete media (photo/video)
- [x] Upload story (photo)
- [x] Change password
- [x] Send direct message
- [x] Search location
- [x] Get location feed
- [x] Collection create/get by id/get all/add items
- [ ] Push notification (coming soon)

## Easy to use
#### Use builder to get Insta API instance:
```c#
var api = new InstaApiBuilder()
                .UseLogger(new SomeLogger())
                .UseHttpClient(new SomeHttpClient())
                .SetUser(new UserCredentials(...You user...))
                .Build();
```
##### Note: every API method has synchronous implementation as well

#### Or load previously saved data in the builder
```c#
var api = new InstaApiBuilder()
                .LoadStateFromStream(stateStream)
                .UseLogger(new SomeLogger())
                .Build();
```

### Quick Examples
#### Login
```c#
IResult<bool> loggedIn = await api.LoginAsync();
```

#### Get user:
```c#
IResult<InstaUser> user = await api.GetUserAsync();
```

#### Get all user posts:
```c#
IResult<InstaMediaList> media = await api.GetUserMediaAsync();
```

#### Get media by its code:
```c#
IResult<InstaMedia> mediaItem = await api.GetMediaByIdAsync("1234567891234567891_123456789);
```

#### Get user timeline feed:
```c#
IResult<InstaFeed> feed = await api.GetUserFeedAsync();
```

#### Comment post:
```c#
IResult<bool> postResult = await apiInstance.CommentMediaAsync("1234567891234567891_123456789", "Hi there!");
```

##### for more samples you can look at [Examples folder](https://github.com/a-legotin/InstaSharper/tree/master/InstaSharper.Examples)


#### [Why two separate repos with same mission?](https://github.com/a-legotin/InstagramAPI-Web/wiki/Difference-between-API-Web-and-just-API-repositories)

#### [Wiki](https://github.com/a-legotin/InstagramAPI/wiki/)

## Special thanks

[ADeltaX](https://github.com/ADeltaX) for contribution

[vitalragaz](https://github.com/vitalragaz) for contribution

[n0ise9914](https://github.com/n0ise9914) for contribution

[Ramtinak](https://github.com/ramtinak) for contribution

[mgp25](https://github.com/mgp25) and his [php wrapper](https://github.com/mgp25/Instagram-API/)

# License

MIT

# Terms and conditions

- Anyone who uses this wrapper MUST follow [Instagram Policy](https://www.instagram.com/about/legal/terms/api/)
- Provided project MUST NOT be used for marketing purposes
- Support will not be provided to those who wish to use this API to send massive messages, likes, follows, and etc
- Use this API at your own risk

## Legal

This code is in no way affiliated with, authorized, maintained, sponsored or endorsed by Instagram or any of its affiliates or subsidiaries. This is an independent and unofficial API wrapper.
#### Code provided for reference purposes only.
