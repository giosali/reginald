import React from 'react';

import PropTypes from 'prop-types';

import '../stylesheets/HamburgerMenu.css';

function HamburgerMenu({ onClick }) {
  return (
    <button
      className="HamburgerMenu"
      onClick={onClick}
      type="button"
    >
      <div className="HamburgerMenu__line" />
      <div className="HamburgerMenu__line" />
      <div className="HamburgerMenu__line" />
    </button>
  );
}

HamburgerMenu.propTypes = {
  onClick: PropTypes.func.isRequired,
};

export default HamburgerMenu;
