import slugify from '../utils';

const features = [
  {
    description: 'Find and open applications with ease.',
    icon: '💾',
    key: 0,
    keyBindingsDescription: '<kbd>Enter</kbd> → Opens the selected application.<br /></br /><kbd>Alt</kbd> → Displays a prompt explaining that pressing <kbd>Enter</kbd> will run the selected application as administrator.<br /></br /><kbd>Alt</kbd> + <kbd>Enter</kbd> → Runs the selected application as administrator.',
    searchDescription: "There are two ways that your search query can match an application:<br /><br /><ul><li>The query matches the beginning of a word in the application's name.<ul><li>The query <span class='code-block'>expl</span> will match the application named <span class='code-block'>File Explorer</span></li></ul></li><li>The query matches the acronymized version of the application's name.<ul><li>The query <span class='code-block'>vsc</span> will match the application named <span class='code-block'>Visual Studio Code</span></li></ul></li></ul>",
    settingsDescription: "You can toggle applications appearing in your search results in <span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Applications</span>.",
    summaryDescription: "You can quickly browse through all of your installed applications. Specifically, Reginald searches for applications that are found in your PC's <span class='code-block'>shell:AppsFolder</span> virtual folder.<br /></br />Whenever you install or uninstall an application, Reginald will search through your applications again and retrieve an updated list of applications.",
    title: 'Application Launcher',
  },
  {
    description: 'Quickly search for folders and files and open them or their containing folder respectively.',
    icon: '📁',
    key: 1,
    keyBindingsDescription: "<kbd>Enter</kbd> → If the selected item is a <i>file</i>, the file's containing folder is opened in File Explorer; otherwise, if the selected item is a <i>folder</i>, the folder itself is opened in File Explorer.",
    searchDescription: "There is one way that your search query can match a file or folder:<br /><br /><ul><li>The query matches the beginning of a word in the name of the file/folder.</li><ul><li>The queries <span class='code-block'>world</span>, <span class='code-block'>how</span>, <span class='code-block'>you</span>, and <span class='code-block'>doing</span> will match the file <span class='code-block'>hello world_how_are123you-doing.txt</span></li></ul></ul>",
    settingsDescription: "You can toggle files and folders from appearing in your search results in <span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>File Search</span>.<br /><br />You can add files and folders for Reginald to ignore while indexing. Folders are denoted by a terminating forward slash ( / ). If it doesn't end with a forward slash, it is treated as a file.",
    summaryDescription: 'After launch, Reginald will index all files and folders in your home directory. Any files or folders that are created, deleted, or renamed in your home directory will be reflected in the index. The indexing process is heavily optimized for memory usage, meaning that Reginald will not be a memory hog because of it.<br /><br />You can begin a file search by entering an apostrophe or by pressing the space bar while there is no input.',
    title: 'File Search',
  },
  {
    description: 'Type away and make quick calculations.',
    icon: '🧮',
    key: 2,
    keyBindingsDescription: '<kbd>Enter</kbd> → Copies the result to your clipboard.<br /><br /><kbd>Alt</kbd> → Adds decimal separators to the result.',
    searchDescription: '',
    settingsDescription: "You can toggle the calculator in <span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Calculator</span>.<br /><br />You can choose which decimal separator you would like to use. Your options are a comma, a period, or the default designated by your operating system's <b>current culture</b>.",
    summaryDescription: "Reginald's built-in calculator can perform basic operations involving exponentiation, multiplication, division, addition, subtraction, and factorials. It can also handle expressions that contain parentheses. There's no keyword involved, just enter your math problem directly into the search window.",
    title: 'Calculator',
  },
  {
    description: 'Open or browse websites through keywords.',
    icon: '🌐',
    key: 3,
    keyBindingsDescription: '<kbd>Enter</kbd> → Opens the URL.<br /><br /><kbd>Alt</kbd> → Displays the alternative web query description<br /><br /><kbd>Alt</kbd> + <kbd>Enter</kbd> → Opens the alternative URL.',
    searchDescription: '',
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Web Queries</span><br /><br />You can toggle all of the default web queries at once or disable specific web queries.<br /><br /><span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Your Web Queries</span><br /><br />You can create your own web queries. You can also delete or edit them by right-clicking on a web query.",
    summaryDescription: "Web queries are keywords that allow you to open or search websites without having to manually open a web browser and type the URL you want to visit, saving you time and boosting productivty. If the default web queries aren't enough for you, you can always disable them or create your own.",
    title: 'Web Queries',
  },
  {
    description: "View and paste texts and images that you've copied.",
    icon: '📋',
    key: 4,
    keyBindingsDescription: '',
    searchDescription: '',
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Clipboard Manager</span><br /><br />You toggle the clipboard manager, change the default key binding that shows and hides the clipboard manager, and toggle the keyword responsible for emptying the clipboard manager.",
    summaryDescription: "Reginald features a clipboard manager that you can access by pressing <kbd>Shift</kbd> + <kbd>Alt</kbd> + <kbd>V</kbd>. You can view the 25 most recent text items or images that you've copied. If you copy a six-digit hexadecimal, Reginald will display the corresponding color in the clipboard manager.<br /><br />Reginald also contains the keyword <span class='code-block'>clear clipboard</span> that removes every entry from the clipboard manager.<br /><br />If you would like to delete a single clipboard item, you can do so by right-clicking on one and clicking <span class='code-block'>Delete</span>.",
    title: 'Clipboard Manager',
  },
  {
    description: 'Render text snippets through text shortcuts.',
    icon: '💨',
    key: 5,
    keyBindingDescription: '',
    searchDescription: '',
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Text Expansions</span><br /><br />You can toggle text expansions entirely and create, edit, or delete them. In order to delete a text expansion, right-click on a text expansion and click <span class='code-block'>Delete</span>.",
    summaryDescription: "Text expansions are a handy productivity tool that consists of two parts: a trigger text and a replacement text. The trigger text is what you need to type to make the replacement text appear.<br /><br />For example, let's say your text expansion is composed of <span class='code-block'>:hello</span> as a trigger and <span class='code-block'>Hello, my name is Reginald</span> as a replacement. When you fully type out <span class='code-block'>:hello</span>, that text will be automatically replaced by <span class='code-block'>Hello, my name is Reginald</span>.<br /><br />Triggers can consist of any characters including letters, numbers, special symbols, etc. Replacements can contain newlines and tabs.<br /><br />The variable <span class='code-block'>{{__cursor__}}</span> can be used to indicate where the text cursor should be placed after a text expansion is triggered. For example, if your replacement text is <span class='code-block'>youtube-dl {{__cursor__}} --hls-prefer-native --abort-on-error</span>, then <span class='code-block'>{{__cursor__}}</span> won't be printed and will be the location where the text cursor is placed.",
    title: 'Text Expansions',
  },
  {
    description: 'Swiftly open URLs in your default web browser.',
    icon: '🔗',
    key: 6,
    keyBindingDescription: '<kbd>Enter</kbd> → Launches the URL in your default web browser.',
    searchDescription: '',
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>URLs</span><br /><br />You toggle this feature.",
    summaryDescription: 'Enter a valid URL and press <kbd>Enter</kbd> to open that website in your default web browser.',
    title: 'URL Launcher',
  },
  {
    description: "Empty your PC's Recycle Bin on command.",
    icon: '🗑️',
    key: 7,
    keyBindingDescription: '<kbd>Enter</kbd> → Either asks for confirmation or proceeds to permanently delete every file in the Recycle Bin.',
    searchDescription: "The query must match the beginning of a word in <span class='code-block'>empty recycle bin</span>.",
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Recycle</span><br /><br />This keyword can be toggled.",
    summaryDescription: "The keyword associated with this feature is <span class='code-block'>empty recycle bin</span>. Before deleting every file in the Recycle Bin, this keyword will ask you for confirmation first.",
    title: 'Empty Recycle Bin',
  },
  {
    description: 'Close application windows without moving your mouse.',
    icon: '❌',
    key: 8,
    keyBindingDescription: '<kbd>Enter</kbd> → Closes all foreground windows that belong to the selected application.',
    searchDescription: "The query must match the keyword <span class='code-block'>quit</span>. Additionally, after typing <span class='code-block'>quit</span>, you can continue to type the name of a particular application.",
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Quit</span><br /><br />This keyword can be toggled.",
    summaryDescription: "The keyword associated with this feature is <span class='code-block'>quit</span>. This command will close all foreground windows that belong to a particular application. If you have unsaved work in an application, that application will ask you if you would like to save your work before closing.",
    title: 'Close Applications',
  },
  {
    description: 'Forcibly shut down applications.',
    icon: '☠️',
    key: 9,
    keyBindingDescription: '<kbd>Enter</kbd> → Forcibly terminates all foreground windows that belong to the selected application.',
    searchDescription: "The query must match the keyword <span class='code-block'>forcequit</span>. Additionally, after typing <span class='code-block'>quit</span>, you can continue to type the name of a particular application.",
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Quit</span><br /><br />This keyword can be toggled.",
    summaryDescription: "The keyword associated with this feature is <span class='code-block'>forcequit</span>. This command will forcibly terminate all windows and processes that belong to a particular application. If you have unsaved work in an application, that application will <b><i>NOT</i></b> ask you if you would like to save your work before quitting.<br /><br />This is equivalent to terminating an application or process through <span class='code-block'>Task Manager</span>.",
    title: 'Force Quit Applications',
  },
  {
    description: 'Set a timer using an intuitive text format.',
    icon: '⏲️',
    key: 10,
    keyBindingDescription: "<kbd>Enter</kbd> → Starts a timer if the keyword is <span class='code-block'>timer</span>; otherwise, nothing happens.<br /><br /><kbd>Alt</kbd> + <kbd>Enter</kbd> → Stops and removes an active timer if the keyword is <span class='code-block'>timers</span>; otherwise, nothing happens.",
    searchDescription: "The query must begin with either the keyword <span class='code-block'>timer</span> or the keyword <span class='code-block'>timers</span>.",
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Timer</span><br /><br />The <span class='code-block'>timer</span> and <span class='code-block'>timers</span> keywords can be toggled.",
    summaryDescription: "The keywords associated with this feature are <span class='code-block'>timer</span> and <span class='code-block'>timers</span>.<br /><br />The <span class='code-block'>timer</span> keyword will start a timer and push a toast notification to your desktop after the timer is up. The general format for this keyword is: <span class='code-block'>timer &lt;time&gt; &lt;message&gt;</span>. Here are some valid expressions:<br /><br /><ul><li><span class='code-block'>timer 5min Drink some water</span></li><li><span class='code-block'>timer 35m 30 seconds Take out laundry</span></li><li><span class='code-block'>timer 24hour 30 mins 1s Work on documentation</span></li></ul><br /><br />The <span class='code-block'>timers</span> keyword will display all currently active timers.",
    title: 'Timer',
  },
  {
    description: "Browse through Microsoft's settings and open them in the Windows Settings application.",
    icon: '⚙️',
    key: 11,
    keyBindingDescription: "<kbd>Enter</kbd> → Opens the related settings page in the <span class='code-block'>Settings</span> application.",
    searchDescription: '',
    settingsDescription: "<span class='code-block'>Settings</span> → <span class='code-block'>Features</span> → <span class='code-block'>Microsoft Settings</span><br /><br />This feature can be toggled.",
    summaryDescription: "This feature allows you to quickly search through Windows's settings and open them in the <span class='code-block'>Settings</span> application.",
    title: 'Microsoft Settings',
  },
];

export function getFeature(name) {
  const feature = features.find((f) => slugify(f.title) === name);
  if (feature === undefined) {
    throw new Response('', {
      status: 404,
      statusText: 'Not found',
    });
  }

  return feature;
}

export function getFeatures() {
  return features;
}
