import React from 'react';

import { PropTypes } from 'prop-types';

import '../stylesheets/MainCard.css';
import Logo from '../images/logo.png';

function MainCard({ downloadUrl }) {
  const handleButtonClick = (e) => window.open(e.target.value, '_blank');
  return (
    <div className="MainCard">
      <img
        className="MainCard__logo"
        src={Logo}
        alt="logo"
      />
      <h1 className="MainCard__header">Reginald</h1>
      <h2 className="MainCard__subheader --gradient-text">A general productivity app for Windows</h2>
      <div className="MainCard__buttons">
        <button
          className="MainCard__buttons__download"
          type="button"
          value={downloadUrl}
          onClick={handleButtonClick}
        >
          Download for
          <i
            className="fa fa-windows"
            aria-hidden="true"
          />
        </button>
        <button
          className="MainCard__buttons__source"
          type="button"
          value="https://github.com/giosali/reginald"
          onClick={handleButtonClick}
        >
          View source code
          <i
            className="fa fa-github"
            aria-hidden="true"
          />
        </button>
      </div>
      <p className="MainCard__note">Available for Windows 10 and Windows 11</p>
    </div>
  );
}
MainCard.propTypes = {
  downloadUrl: PropTypes.string.isRequired,
};

export default MainCard;
