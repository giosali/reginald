import React from 'react';

import { Link } from 'react-router-dom';

import '../stylesheets/Features.css';

function Features() {
  return (
    <main className="Features">
      <div className="intro">
        <h1 className="intro__title">Features</h1>
        <p className="intro__description">Explore everything that Reginald has to offer</p>
      </div>
      <div className="cards">
        <Link
          className="card"
          to="/reginald/features/application-launcher"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Application Launcher</h3>
          <span className="card__icon">💾</span>
          <p className="card__description">Find and open applications with ease.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">File Search</h3>
          <span className="card__icon">📁</span>
          <p className="card__description">Quickly search for folders and files and open them or their containing folder respectively.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Calculator</h3>
          <span className="card__icon">🧮</span>
          <p className="card__description">Type away and make quick calculations.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Web Queries</h3>
          <span className="card__icon">🌐</span>
          <p className="card__description">Open or browse websites through keywords.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Clipboard Manager</h3>
          <span className="card__icon">📋</span>
          <p className="card__description">View and paste texts and images that you&apos;ve copied.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Text Expansions</h3>
          <span className="card__icon">💨</span>
          <p className="card__description">Render text snippets through text shortcuts.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">URL Launcher</h3>
          <span className="card__icon">🔗</span>
          <p className="card__description">Swiftly open URLs in your default web browser.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Empty Recycle Bin</h3>
          <span className="card__icon">🗑️</span>
          <p className="card__description">Empty your PC&apos;s Recycle Bin on command.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Close Applications</h3>
          <span className="card__icon">❌</span>
          <p className="card__description">Close application windows without moving your mouse.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Force Quit Applications</h3>
          <span className="card__icon">☠️</span>
          <p className="card__description">Forcibly shut down applications.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Timer</h3>
          <span className="card__icon">⏲️</span>
          <p className="card__description">Set a timer using an intuitive text format.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Microsoft Settings</h3>
          <span className="card__icon">⚙️</span>
          <p className="card__description">Browse through Microsoft&apos;s settings and open them in the Windows Settings application.</p>
        </Link>
        <Link
          className="card"
          href="#/"
        >
          <div className="card__backdrop" />
          <h3 className="card__title">Change Themes</h3>
          <span className="card__icon">🖼️</span>
          <p className="card__description">Change Reginald&apos;s appearance.</p>
        </Link>
      </div>
    </main>
  );
}

export default Features;
