# Changelog

All notable changes to this project will be documented in this file.

## [v1.0.1] - 2022-10-30

### Fixed

* Blurry application icons due to the Windows 11 October update.
* Fixed application crashing due to %APPDATA% directory not being created before being used.

## [v1.0.0] - 2022-10-13

### Added

* Ability to search through files and folders on your computer.
* Search and clipboard manager windows can become transparent by toggling <kbd>Ctrl</kbd> + <kbd>T</kbd>.
* `Clear clipboard` keyword to empty Reginald's clipboard manager.
* `forcequit` keyword to forcibly close applications.
* Optional decimal separators for the calculator keyword.
* `Features` section in settings.
* Search results limit.
* Option to run applications as administrator by pressing <kbd>Alt</kbd> + <kbd>Enter</kbd>.

### Changed

* Application icon.
* Move most settings under the `Features` section.
* Improve performance when dealing with large text selections in clipboard manager.
* Clipboard items can be deleted by right-clicking and clicking `Delete`.
* Build compression setting from 'for size' to 'for speed'.

### Fixed

* **Major** performance improvements.
* Clipboard manager will now remove the last item when the limit is reached.
* Search results clipping outside the search window.

## v0.11.0

### Added

* Added a built-in **clipboard manager**
  * The clipboard manager can be accessed by pressing <kbd>Shift</kbd> + <kbd>Alt</kbd> + <kbd>V</kbd>
* The installer now contains an option to run the application after installation
* Added original acrylic material

### Changed

* The main search box window is now a popup window
* **Changed appearance of the settings section** of the application
  * Removed third-party UI libraries
* Improved handling of URLs
* Multiple instances of the application are now prevented from concurrently running
* Change appearance of the scroll bar

### Fixed

* Fixed bug where certain math expressions would crash the application
* Fixed bug where the <kbd>Alt</kbd> key would be programmatically stuck
* Fixed regex parsing bug where certain symbols would crash the application
* Fixed bug where the application wouldn't run on startup after updating
* Fixed bug where pressing <kbd>Tab</kbd> on a keyword would crash the application

## v0.10.0

### Added

* Added ability to have Reginald launch on system startup
  * This setting can be toggled in either:
    * the menu of the system tray icon
    * the `General` settings page in the settings portion of the application
* Added toggle button for text expansions
* Added support for uppercase letters and symbols in text expansion triggers
  * You are no longer restrained to using a dollar symbol for your triggers
    * If you have been using a version of Reginald prior to `v0.10.0`, please review your triggers and manually prefix a dollar symbol (or any other symbol if you wish) to them
* Added ability to view current timers and cancel them
  * Type `timers` to view a list of currently active timers and see how much time is left
    * You can press `Alt + Enter` on a selected timer to cancel it

### Fixed

* Fixed issue where certain URLs would not be picked by the search box
* Fixed issue where poorly constructed URLs would be picked by the search box
* Fixed issue where timer keyword would incorrectly parse the input text

## v0.9.1

### Changed

* Changed interface for adding user keywords
* `Search Box Appearance` has been removed from the menu sidebar
* UI has been made more consistent in the settings portion of the application
* Changed icons for a few of the default keywords
* Refactored a majority of the codebase
  * Adding new features will be much more seamless and quicker

## v0.9.0

### Added

* Added a new and exciting feature: text expansions
  * You can configure these within the settings panel of the app under `Expansions`

## v0.8.0

### Added

* Added ability to to disable the application through the system tray icon
* Added fluent support for Windows 11
* Added new theme: "Black Cherry"

## v0.7.0

### Added

* Added ability to close open applications by using the `quit` command

### Fixed

* Fixed issue where timer messages would become overridden by other search results
* Fixed issue where the application would crash if alphabetical characters were provided after the timer command instead of a number
* Fixed issue where fields would disappear from settings app if they were disabled

## v0.6.1

### Fixed

* Fixed issue where search results would become highlighted if the cursor was already over the area of the search result *again*

## v0.6.0

### Added

* Themes have been properly added to the settings window, paving the way for more proprietary and user-generated themes

### Changed

* Search box window should now open significantly more quickly than before

### Fixed

* Fixed issue where a search result would become highlighted if the cursor is already over the area of the search result when it appears

### Removed

* Removed ability to toggle the light/dark modes and the border around the search box window

## v0.5.0

### Added

* Added ability to open settings pages from search bar

### Fixed

* Fixed issue where Styvio API would cause application to crash if certain values are empty

## v0.4.3

### Fixed

* Fixed issue where calculations involving a single, lone decimal ('.') would cause the application to crash
* Fixed issue where apostrophes would cause the application to crash

## v0.4.2

### Added

* Added acrylic blur behind search box window

## v0.4.1

### Fixed

* Fixed issue where entering parentheses into the search box would crash the application

## v0.4.0

### Added

* Added a new utility: empty recycle bin

### Changed

* Changed UI again due to major issues with the previous UI library
* Made text selectable in special keyword search results
* Updated developer info

### Fixed

* Fixed issue where restart button wouldn't reopen the application
* Fixed issue where search box would not close after using the timer command
* Fixed issue where the system tray icon would no longer function after a period of time
* Fixed several responsiveness issues

## v0.3.1

### Changed

* Changed minimum supported version of Windows 10 to Build 18362 (version: 1903; codename: 19H1; released: May 21, 2019; marketing name: May 2019 Update)

### Fixed

* Fixed issue where new tabs opened in the web browser would lose focus after being opened

## v0.3.0

### Added

* Added the `timer` keyword which allows users to create notifications that will be displayed after a set amount of time
* Added ability for user to change the highlight color of search results to the system highlight color
* Added new tabs in the side navigation menu
* Added ability for user to change key binding for opening the search box window

### Changed

* Massively revamped the UI

## v0.2.1

### Changed

* Changed keyword search results from appearing if only part of the keyword was entered
* Update version number in NotifyIcon tooltip
* URIs without schemes will break if the top-level domain contains a space and doesn't contain a forward slash
* Changed minimum supported version of Windows 10 to Build 17763 (version: 1809; codename: Redstone 5; released: November 13, 2018; marketing name: October 2018 Update)

## v0.2.0

### Added

* Added ability to enable a border around the search box
* Added ability to calculate factorials
* Added special keywords to search results

### Fixed

* Fixed startup time of search box window

## v0.1.0

* First release