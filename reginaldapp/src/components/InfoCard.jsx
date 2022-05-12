import React, { useEffect, useRef } from 'react';

import { PropTypes } from 'prop-types';

import '../stylesheets/InfoCard.css';

function isVowel(char) {
  return 'aeiou'.indexOf(char) !== -1;
}

function InfoCard({
  title, description, position, setFixedDescription, image, representation,
}) {
  const componentRef = useRef();
  const handleScroll = () => {
    if (componentRef.current !== undefined) {
      const { top } = componentRef.current.getBoundingClientRect();
      if (top >= -50 && top <= 50) {
        const text = componentRef.current.querySelector('.InfoCard__info__title').textContent.toLowerCase();
        setFixedDescription(`${isVowel(text[0]) ? 'An' : 'A'} ${text} for Windows`);
      }
    }
  };
  useEffect(() => {
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  });

  let infoCardStyle;
  let imageStyle;
  switch (position) {
    case 'right':
      infoCardStyle = { flexDirection: 'row-reverse' };
      break;
    case 'center':
      infoCardStyle = { flexDirection: 'column' };
      imageStyle = { width: '576px' };
      break;
    default:
      break;
  }

  return (
    <div
      className="InfoCard"
      style={infoCardStyle}
      ref={componentRef}
    >
      <div className="InfoCard__info">
        <h2 className="InfoCard__info__title">{title}</h2>
        <p
          className="InfoCard__info__description"
          dangerouslySetInnerHTML={{ __html: description }}
        />
      </div>
      {image === undefined
        ? representation
        : (
          <img
            className="InfoCard__image"
            src={image}
            alt={title}
            style={imageStyle}
          />
        )}
    </div>
  );
}
InfoCard.propTypes = {
  title: PropTypes.string.isRequired,
  description: PropTypes.string.isRequired,
  position: PropTypes.string.isRequired,
  setFixedDescription: PropTypes.func.isRequired,
  image: PropTypes.string,
  // eslint-disable-next-line react/forbid-prop-types
  representation: PropTypes.object,
};
InfoCard.defaultProps = {
  image: undefined,
  representation: undefined,
};

export default InfoCard;
