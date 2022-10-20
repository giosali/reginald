import React from 'react';

import '../stylesheets/Home.css';
import Logo from '../images/logo.png';

function Home() {
  return (
    <div className="Home">
      <div className="hero">
        <img
          alt="Reginald logo"
          className="hero__image"
          src={Logo}
        />
        <h1 className="hero__title">Reginald</h1>
        <p className="hero__description">A general productivity application designed for Windows and inspired by Alfred</p>
      </div>
    </div>
  );
}

export default Home;
