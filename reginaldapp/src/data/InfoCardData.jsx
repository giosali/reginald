import React from 'react';

import HelloTextExpansion from '../components/HelloTextExpansion';
import ApplicationLauncherImage from '../images/applicationlauncher.png';
import CalculatorImage from '../images/calculator.png';
import ClipboardManagersImage from '../images/clipboardmanagers.png';
import WindowsImage from '../images/windows.png';

const InfoCardData = [
  {
    id: '1',
    title: 'Application launcher',
    description: "Don't be troubled by Windows's built-in slow and unreliable search results any longer. With <b>Reginald</b>, quickly launching and opening applications on your PC are a few keystrokes away.<br/><br/>Get started by pressing <kbd>Alt</kbd> + <kbd/>Space</kbd>. If you don't like this key combination, no worries. You can always set your own key gesture in the settings.",
    position: 'left',
    image: ApplicationLauncherImage,
  },
  {
    id: '2',
    title: 'Calculator',
    description: 'Need to work out some quick math? Let <b>Reginald</b> take care of that for you. Enter your problems into the search bar and hit <kbd>Enter</kbd> to save the result to your clipboard.',
    position: 'right',
    image: CalculatorImage,
  },
  {
    id: '3',
    title: 'Clipboard manager',
    description: "<b>Reginald</b> features a clipboard manager with a familiar design, allowing you to quickly preview and access text and images that you've previously copied. The clipboard manager can be intuitively called by pressing <kbd>Shift</kbd> + <kbd>Alt</kbd> + <kbd>V</kbd> or be bound to another key gesture in the settings.",
    position: 'center',
    image: ClipboardManagersImage,
  },
  {
    id: '4',
    title: 'Web tool',
    description: "Quickly launch websites and make web searches without the hassle of opening a new tab by using some of <b>Reginald</b>'s built-in keywords. Keywords make browsing and searching the web faster, boosting productivity and efficiency through their ease of use.<br/><br/>Find the keywords lacking? No worries, you can create and add your own through the settings panel.",
    position: 'right',
    image: WindowsImage,
  },
  {
    id: '5',
    title: 'Text expander',
    description: "Text expansions are keyboard inputs (<b><i>triggers</i></b>) that transform into larger text (<b><i>replacements</i></b>). For example, you could have a text expansion that prints out '<b><i>My name is Reginald</i></b>' each time you type '<b><i>$hello</i></b>'. They make life easier and save you time wasted when typing out the same boilerplate text over and over.<br/><br/><b>Reginald</b> provides built-in functionality for creating text expansions and an intuitive manager for adding or deleting them. On top of that, <b>Reginald</b> offers an option to place your cursor after a text expansion is triggered, giving both your fingers and your mouse a rest.",
    position: 'center',
    representation: <HelloTextExpansion />,
  },
];

export default InfoCardData;
