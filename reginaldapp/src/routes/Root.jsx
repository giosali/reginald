import React, { useState } from 'react';

import { Link, Outlet } from 'react-router-dom';

import '../stylesheets/Root.css';
import HamburgerMenu from '../components/HamburgerMenu';

function Root() {
  const [isHamburgerMenuActive, setIsHamburgerMenuActive] = useState(false);
  const handleClick = () => {
    setIsHamburgerMenuActive(!isHamburgerMenuActive);
  };

  return (
    <div className="Root">
      <nav className="nav">
        <ul className="nav__list">
          <li className="nav__item">
            <Link className="nav__link" to="/reginald">
              <img
                alt="Reginald logo"
                className="nav__logo"
                src={require('../images/logo.png')} // eslint-disable-line global-require
              />
            </Link>
          </li>
          <li className="nav__item">
            <HamburgerMenu className="nav__hamburger-menu" onClick={handleClick} />
            <button
              aria-label="close"
              className={isHamburgerMenuActive ? 'nav__hamburger-menu-close nav__hamburger-menu-close--active' : 'nav__hamburger-menu-close'}
              onClick={handleClick}
              type="button"
            />
            <div className={isHamburgerMenuActive ? 'nav__hamburger-menu-links nav__hamburger-menu-links--active' : 'nav__hamburger-menu-links'}>
              <Link className="nav__link" to="/reginald/features">Features</Link>
              <a
                className="nav__link"
                href="https://github.com/giosali/reginald"
                rel="noreferrer"
                target="_blank"
              >
                GitHub
              </a>
            </div>
          </li>
        </ul>
      </nav>
      <Outlet />
    </div>
  );
}

export default Root;
