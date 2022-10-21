import React from 'react';

import PropTypes from 'prop-types';

function GitHubButton({
  href, icon, repo, type,
}) {
  let ariaLabel = 'GitHub button';
  let showCount = false;

  switch (type.toLowerCase()) {
    case 'download':
      ariaLabel = `Download ${repo} on GitHub`;
      break;
    case 'star':
      ariaLabel = `Star ${repo} on GitHub`;
      showCount = true;
      break;
    default:
      break;
  }

  return (
    <a href={href}>
      <a
        aria-label={ariaLabel}
        className="github-button"
        data-color-scheme="no-preference: dark; light: dark; dark: dark;"
        data-icon={icon}
        data-show-count={showCount}
        data-size="large"
        href={type.toLowerCase() === 'star' ? href : '#/'}
      >
        {type}
      </a>
    </a>
  );
}

GitHubButton.defaultProps = {
  icon: '',
};

GitHubButton.propTypes = {
  href: PropTypes.string.isRequired,
  icon: PropTypes.string,
  repo: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
};

export default GitHubButton;
