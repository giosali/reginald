import React from 'react';

import { Link, Outlet } from 'react-router-dom';

import '../stylesheets/Root.css';
import Logo from '../images/logo.png';

function Root() {
  return (
    <div className="Root">
      <nav className="nav">
        <ul className="nav__list">
          <Link className="nav__item" to="/reginald">
            <img
              alt="Reginald logo"
              className="nav__logo"
              src={Logo}
            />
          </Link>
          <Link className="nav__item" to="/reginald/features">Features</Link>
          <Link className="nav__item" to="/reginald/github">GitHub</Link>
          <Link className="nav__item" to="/reginald/download">Download</Link>
        </ul>
      </nav>
      <Outlet />
    </div>
  );
}

export default Root;
