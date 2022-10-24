import React from 'react';

import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';

import '../stylesheets/FeatureCard.css';

function FeatureCard({
  description, icon, title, to,
}) {
  return (
    <Link
      className="FeatureCard"
      to={to}
    >
      <div className="FeatureCard__backdrop" />
      <h3 className="FeatureCard__title">{title}</h3>
      <span className="FeatureCard__icon">{icon}</span>
      <p className="FeatureCard__description">{description}</p>
    </Link>
  );
}

FeatureCard.propTypes = {
  description: PropTypes.string.isRequired,
  icon: PropTypes.string.isRequired,
  title: PropTypes.string.isRequired,
  to: PropTypes.string.isRequired,
};

export default FeatureCard;
