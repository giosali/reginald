<h1 align="center">Reginald</h1>

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/reginald.png" width="150">
</p>

<p align="center">
	Reginald is an application for Windows 10 designed to make your life easier and more productive.	
</p>

<p align="center">
  <img alt="ci workflow" src="https://github.com/giosali/reginald/actions/workflows/ci.yml/badge.svg">
  <img alt="license" src="https://img.shields.io/github/license/giosali/reginald">
  <img alt="version" src="https://img.shields.io/github/v/release/giosali/reginald">
  <img alt="os" src="https://img.shields.io/badge/Windows-0078D6?style=flat&logo=windows&logoColor=white">
  <img alt="language" src="https://img.shields.io/badge/c%23-%23239120.svg?style=flat&logo=c-sharp&logoColor=white">
</p>

## Features

### Application Launcher

Reginald can help you find your applications quickly and effortlessly. Press <kbd>Alt</kbd> + <kbd>Space</kbd> to open the Reginald search window and type away.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/reginald_search_dark.png" width="750">
</p>

### Calculations

Need to work out some quick math? Let Reginald take care of that for you.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/reginald_math_dark.png" width="750">
</p>

You can also press <kbd>Enter</kbd> to copy the result to your clipboard.

### Web Searches

#### Keywords

Quickly launch websites and make web searches without the hassle of opening a new tab by using keywords.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/reginald_keyword_dark.png" width="750">
</p>

Reginald comes ready with some keywords that you can use straight out of the box:

| Name       | Keyword    | Description                 |
| ---------- | ---------- | --------------------------- |
| Amazon     | amazon     | Search Amazon for '...'     |
| DuckDuckGo | ddg        | Search DuckDuckGo for '...' |
| eBay       | ebay       | Search eBay for '...'       |
| Facebook   | fb         | Search Facebook for '...'   |
| GitHub     | git        | Search GitHub for '...'     |
| Google     | g          | Search Google for '...'     |
| IMDb       | imdb       | Search IMDb for '...'       |
| Pinterest  | pinterest  | Search Pinterest for '...'  |
| Reddit     | r/         | Go to r/                    |
| TikTok     | tiktok     | Go to _'s profile           |
| TikTok     | tiktok     | Search TikTok for '...'     |
| tumblr     | tumblr     | Go to _'s blog              |
| tumblr     | tumblr     | Search tumblr for '...'     |
| Twitch     | twitch     | Go to _'s channel           |
| Instagram  | ig         | Go to _'s profile           |
| Twitter    | twitter    | Search Twitter for '...'    |
| Twitter    | twitter    | Go to _'s profile           |
| Wikipedia  | wiki       | Search Wikipedia for '...'  |
| YouTube    | yt         | Search YouTube for '...'    |

Not satisfied with these? Create your own keywords in the `User Keywords` section under the `Utilities` header in Reginald's settings window. Additionally, you can prevent certain keywords from appearing in your search results or disable all of them at once.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/defaultkeywords.png" width="500">
</p>

#### HTTP

Got a URL on hand? Paste it into Reginald's search window and hit <kbd>Enter</kbd> to go there.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/reginald_website_dark.png" width="750">
</p>

### Special Keywords

There are certain keywords available that require an Internet connection to make requests to various APIs.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/stock.png" width="500">
</p>

These are the currently available special keywords:

| Name           | Keyword    | Description                     |
| -------------- | ---------- | ------------------------------- |
| Styvio         | stock      | Look up stock information       |
| Cloudflare     | ip         | Fetch your current IPv4 address |

### Commands

Reginald comes with keywords that sometimes require additional input for them to work. Those keywords are called `commands`.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/timer.png" width="500">
</p>

| Keyword    | Description       |
| ---------- | ----------------- |
| timer      | Set up a reminder |

### Key Bindings

Don't like using <kbd>Alt</kbd> + <kbd>Space</kbd>? No problem. Feel free to change it to something more comfortable and suitable for you through the settings panel.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/general.png" width="500">
</p>

### Dark Mode & Light Mode

Reginald uses a dark mode by default for the search window but if you prefer a lighter look, you can disable dark mode.

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/reginald_dark_light_mode.gif" width="600">
</p>

## Installation

In order to install Reginald, you can head over to the [Releases](https://github.com/giosali/reginald/releases) section of the repository, find the latest release, and click on the `Reginald-x.x.x.msi` installer file under `Assets`. Once you've downloaded the installer, open it and follow the installer's instructions. After that, you're all set to use it!

### Microsoft Defender Smartscreen Message

There's a chance you may receive the following message while attempting to install Reginald:

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/reginald/main/media/smartscreen.png" width="450">
</p>

One reason why this may occur is because the application doesn't currently have a code signing certificate. Code signing ensures that the source code hasn't been tampered with since being signed but these certificates also cost quite a bit and I'm not currently willing to invest money into it *yet*.

In order to get around the Smartscreen message, you'll need to do the following steps:

* Right-click on the `.msi` installer file
* Click on `Properties`
* Check the `Unblock` checkbox towards the bottom of the `General` tab

Once you do that, you should be able to install Reginald without issue.

## Issues and Suggestions

If you happen to have any problems or ideas to improve Reginald, please don't hesitate to [file an issue](https://github.com/giosali/reginald/issues/new)!
