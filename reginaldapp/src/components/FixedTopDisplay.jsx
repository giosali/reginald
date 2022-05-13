import React, { useEffect, useState } from 'react';

import '../stylesheets/FixedTopDisplay.css';
import PropTypes from 'prop-types';

function FixedTopDisplay({ isVisible, description }) {
  const [style, setStyle] = useState({ opacity: '0' });
  useEffect(() => {
    setStyle({
      opacity: isVisible ? '1' : '0',
    });
  }, [isVisible]);

  return (
    <div
      className="FixedTopDisplay"
      style={{ opacity: style.opacity }}
    >
      <h3 className="FixedTopDisplay__title --gradient-text">Reginald</h3>
      <p className="FixedTopDisplay__description">{description}</p>
    </div>
  );
}
FixedTopDisplay.propTypes = {
  description: PropTypes.string.isRequired,
  isVisible: PropTypes.bool.isRequired,
};

export default FixedTopDisplay;
