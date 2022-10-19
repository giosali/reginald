import React from 'react';

import { Link, Outlet } from 'react-router-dom';

import '../stylesheets/Root.css';
import Logo from '../images/logo.png';

function Root() {
  return (
    <div className="Root">
      <nav className="nav">
        <ul className="nav__list">
          <li className="nav__item">
            <Link className="nav__link" to="/reginald">
              <img
                alt="Reginald logo"
                className="nav__logo"
                src={Logo}
              />
            </Link>
          </li>
          <li className="nav__item">
            <Link className="nav__link" to="/reginald/features">Features</Link>
          </li>
          <li className="nav__item">
            <Link className="nav__link" to="/reginald/github">GitHub</Link>
          </li>
          <li className="nav__item">
            <Link className="nav__link" to="/reginald/download">Download</Link>
          </li>
        </ul>
      </nav>
      <Outlet />
    </div>
  );
}

export default Root;
