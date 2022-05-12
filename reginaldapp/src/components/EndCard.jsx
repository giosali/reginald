import React from 'react';

import { PropTypes } from 'prop-types';

import '../stylesheets/EndCard.css';

function EndCard({ downloadUrl }) {
  const handleButtonClick = (e) => window.open(e.target.value, '_blank');

  return (
    <div className="EndCard">
      <h1 className="EndCard__title --gradient-text">Your next assistant</h1>
      <p className="EndCard__description">Reginald is free and open-source.</p>
      <button
        className="EndCard__button"
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
    </div>
  );
}
EndCard.propTypes = {
  downloadUrl: PropTypes.string.isRequired,
};

export default EndCard;
