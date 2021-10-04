# Changelog

All notable changes to this project will be documented in this file.

## v0.7.0

### Added

* Added ability to close open applications by using the `quit` command

### Fixed

* Fixed issue where timer messages would become overridden by other search results
* Fixed issue where the application would crash if alphabetical characters were provided after the timer command instead of a number

* Fixed issue where fields would disappear from settings app if they were disenabled

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