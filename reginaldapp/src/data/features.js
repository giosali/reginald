const features = {
  0: {
    to: '/reginald/features/application-launcher',
    title: 'Application Launcher',
    icon: '💾',
    description: 'Find and open applications with ease.',
  },
  1: {
    to: '/reginald/features/file-search',
    title: 'File Search',
    icon: '📁',
    description: 'Quickly search for folders and files and open them or their containing folder respectively.',
  },
  2: {
    to: '/reginald/features/calculator',
    title: 'Calculator',
    icon: '🧮',
    description: 'Type away and make quick calculations.',
  },
  3: {
    to: '/reginald/features/web-queries',
    title: 'Web Queries',
    icon: '🌐',
    description: 'Open or browse websites through keywords.',
  },
  4: {
    to: '/reginald/features/clipboard-manager',
    title: 'Clipboard Manager',
    icon: '📋',
    description: "View and paste texts and images that you've copied.",
  },
  5: {
    to: '/reginald/features/text-expansions',
    title: 'Text Expansions',
    icon: '💨',
    description: 'Render text snippets through text shortcuts.',
  },
  6: {
    to: '/reginald/features/url-launcher',
    title: 'URL Launcher',
    icon: '🔗',
    description: 'Swiftly open URLs in your default web browser.',
  },
  7: {
    to: '/reginald/features/empty-recycle-bin',
    title: 'Empty Recycle Bin',
    icon: '🗑️',
    description: "Empty your PC's Recycle Bin on command.",
  },
  8: {
    to: '/reginald/features/close-applications',
    title: 'Close Applications',
    icon: '❌',
    description: 'Close application windows without moving your mouse.',
  },
  9: {
    to: '/reginald/features/force-quit-applications',
    title: 'Force Quit Applications',
    icon: '☠️',
    description: 'Forcibly shut down applications.',
  },
  10: {
    to: '/reginald/features/timer',
    title: 'Timer',
    icon: '⏲️',
    description: 'Set a timer using an intuitive text format.',
  },
  11: {
    to: '/reginald/features/microsoft-settings',
    title: 'Microsoft Settings',
    icon: '⚙️',
    description: "Browse through Microsoft's settings and open them in the Windows Settings application.",
  },
  12: {
    to: '/reginald/features/change-themes',
    title: 'Change Themes',
    icon: '🖼️',
    description: "Change Reginald's appearance.",
  },
};

export function getFeature() {
  return 1;
}

export function getFeatures() {
  return features;
}
