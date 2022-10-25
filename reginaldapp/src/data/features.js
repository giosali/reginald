import slugify from '../utils';

const features = [
  {
    description: 'Find and open applications with ease.',
    icon: '💾',
    key: 0,
    title: 'Application Launcher',
  },
  {
    description: 'Quickly search for folders and files and open them or their containing folder respectively.',
    icon: '📁',
    key: 1,
    title: 'File Search',
  },
  {
    description: 'Type away and make quick calculations.',
    icon: '🧮',
    key: 2,
    title: 'Calculator',
  },
  {
    description: 'Open or browse websites through keywords.',
    icon: '🌐',
    key: 3,
    title: 'Web Queries',
  },
  {
    description: "View and paste texts and images that you've copied.",
    icon: '📋',
    key: 4,
    title: 'Clipboard Manager',
  },
  {
    description: 'Render text snippets through text shortcuts.',
    icon: '💨',
    key: 5,
    title: 'Text Expansions',
  },
  {
    description: 'Swiftly open URLs in your default web browser.',
    icon: '🔗',
    key: 6,
    title: 'URL Launcher',
  },
  {
    description: "Empty your PC's Recycle Bin on command.",
    icon: '🗑️',
    key: 7,
    title: 'Empty Recycle Bin',
  },
  {
    description: 'Close application windows without moving your mouse.',
    icon: '❌',
    key: 8,
    title: 'Close Applications',
  },
  {
    description: 'Forcibly shut down applications.',
    icon: '☠️',
    key: 9,
    title: 'Force Quit Applications',
  },
  {
    description: 'Set a timer using an intuitive text format.',
    icon: '⏲️',
    key: 10,
    title: 'Timer',
  },
  {
    description: "Browse through Microsoft's settings and open them in the Windows Settings application.",
    icon: '⚙️',
    key: 11,
    title: 'Microsoft Settings',
  },
  {
    description: "Change Reginald's appearance.",
    icon: '🖼️',
    key: 12,
    title: 'Themes',
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
