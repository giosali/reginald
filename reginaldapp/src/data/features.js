const features = [
  {
    description: 'Find and open applications with ease.',
    icon: '💾',
    title: 'Application Launcher',
  },
  {
    description: 'Quickly search for folders and files and open them or their containing folder respectively.',
    icon: '📁',
    title: 'File Search',
  },
  {
    description: 'Type away and make quick calculations.',
    icon: '🧮',
    title: 'Calculator',
  },
  {
    description: 'Open or browse websites through keywords.',
    icon: '🌐',
    title: 'Web Queries',
  },
  {
    description: "View and paste texts and images that you've copied.",
    icon: '📋',
    title: 'Clipboard Manager',
  },
  {
    description: 'Render text snippets through text shortcuts.',
    icon: '💨',
    title: 'Text Expansions',
  },
  {
    description: 'Swiftly open URLs in your default web browser.',
    icon: '🔗',
    title: 'URL Launcher',
  },
  {
    description: "Empty your PC's Recycle Bin on command.",
    icon: '🗑️',
    title: 'Empty Recycle Bin',
  },
  {
    description: 'Close application windows without moving your mouse.',
    icon: '❌',
    title: 'Close Applications',
  },
  {
    description: 'Forcibly shut down applications.',
    icon: '☠️',
    title: 'Force Quit Applications',
  },
  {
    description: 'Set a timer using an intuitive text format.',
    icon: '⏲️',
    title: 'Timer',
  },
  {
    description: "Browse through Microsoft's settings and open them in the Windows Settings application.",
    icon: '⚙️',
    title: 'Microsoft Settings',
  },
  {
    description: "Change Reginald's appearance.",
    icon: '🖼️',
    title: 'Themes',
  },
];

export function getFeature(name) {
  const feature = features.find((f) => f.title === name);
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
