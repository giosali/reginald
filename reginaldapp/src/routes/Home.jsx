import React, { useEffect, useState } from 'react';

import GitHubButton from 'react-github-btn';

import '../stylesheets/Home.css';

function Home() {
  const [downloadUrl, setDownloadUrl] = useState('https://github.com/giosali/reginald/releases');

  useEffect(() => {
    const ep = 'https://api.github.com/repos/giosali/reginald/releases/latest';
    fetch(ep, {
      headers: {
        Accept: 'application/vnd.github.v3+json',
      },
    })
      .then((response) => response.json())
      .then((data) => setDownloadUrl(data.assets[0].browser_download_url))
      .catch((error) => console.log(error.message));
  }, []);

  return (
    <main className="Home">
      <div className="hero">
        <img
          alt="Reginald logo"
          className="hero__image"
          src={require('../images/logo.png')} // eslint-disable-line global-require
        />
        <h1 className="hero__title">Reginald</h1>
        <p className="hero__description">A general productivity application designed for Windows and inspired by Alfred</p>
        <div className="hero__github-buttons">
          <GitHubButton
            data-color-scheme="no-preference: dark; light: dark; dark: dark;"
            data-icon="octicon-star"
            data-show-count="true"
            data-size="large"
            href="https://github.com/giosali/reginald"
          >
            Star
          </GitHubButton>
          <GitHubButton
            data-color-scheme="no-preference: dark; light: dark; dark: dark;"
            data-size="large"
            href={downloadUrl}
          >
            Download
          </GitHubButton>
        </div>
      </div>
      <div className="divider">
        <h2 className="divider__title">
          Your
          {' '}
          <p>new</p>
          {' '}
          personal assistant
        </h2>
      </div>
      <div className="cards">
        <div className="card">
          <div className="card__body">
            <h3 className="card__title">Application launcher</h3>
            <p className="card__description">
              Reginald can help you find and launch your applications quickly and effortlessly.
              Press
              {' '}
              <kbd>Alt</kbd>
              {' '}
              +
              {' '}
              <kbd>Space</kbd>
              {' '}
              to open the Reginald search window and type away.
            </p>
          </div>
          <img
            alt="Application launcher"
            className="card__image"
            src={require('../images/application-launcher.png')} // eslint-disable-line global-require
          />
        </div>
        <div className="card card--reverse">
          <div className="card__body card__body--reverse">
            <h3 className="card__title">File search</h3>
            <p className="card__description">
              Looking for a file?
              You can instantly search through files in your user directory by pressing
              {' '}
              <kbd>&apos;</kbd>
              {' '}
              or
              {' '}
              <kbd>Space</kbd>
              {' '}
              when the search window is empty.
            </p>
          </div>
          <img
            alt="File search"
            className="card__image"
            src={require('../images/file-search.png')} // eslint-disable-line global-require
          />
        </div>
        <div className="card">
          <div className="card__body">
            <h3 className="card__title">Calculator</h3>
            <p className="card__description">
              Need to work out some quick math?
              Let Reginald take care of that for you by freely typing expressions into the search
              box.
            </p>
          </div>
          <img
            alt="Calculator"
            className="card__image"
            src={require('../images/calculator.png')} // eslint-disable-line global-require
          />
        </div>
        <div className="card card--reverse">
          <div className="card__body card__body--reverse">
            <h3 className="card__title">Web queries</h3>
            <p className="card__description">
              Never experience the hassle of having to open your web browser and open a new tab
              just to look something up again.
              With Reginald, you can quickly open websites or make web searches with ease through
              web queries.
            </p>
          </div>
          <img
            alt="Web queries"
            className="card__image"
            src={require('../images/web-queries.png')} // eslint-disable-line global-require
          />
        </div>
        <div className="card">
          <div className="card__body">
            <h3 className="card__title">Clipboard manager</h3>
            <p className="card__description">
              Reginald features a built-in clipboard manager, allowing you to quickly preview
              and access text and images that you&apos;ve previously copied.
              The clipboard manager can be called by pressing
              {' '}
              <kbd>Shift</kbd>
              {' '}
              +
              {' '}
              <kbd>Alt</kbd>
              {' '}
              +
              {' '}
              <kbd>V</kbd>
              .
            </p>
          </div>
          <img
            alt="Clipboard manager"
            className="card__image"
            src={require('../images/clipboard-manager.png')} // eslint-disable-line global-require
          />
        </div>
        <div className="card card--reverse">
          <div className="card__body card__body--reverse">
            <h3 className="card__title">Text expansions</h3>
            <p className="card__description">
              Text expansions can help you save time by preventing you from repeatedly typing
              the same text over and over.
              When the text you type matches a trigger, that text will be replaced by some
              replacement text designated by you.
            </p>
          </div>
          <div className="card__text-expansion">
            <p />
          </div>
        </div>
        <div className="card">
          <div className="card__body">
            <h3 className="card__title">URLs</h3>
            <p className="card__description">
              Got a URL on hand?
              Paste it into Reginald&apos;s search window and press Enter to open it in your default
              web browser.
            </p>
          </div>
          <img
            alt="URL"
            className="card__image"
            src={require('../images/url.png')} // eslint-disable-line global-require
          />
        </div>
      </div>
      <div className="footer">
        <h3 className="footer__title">Give Reginald a try</h3>
        <GitHubButton
          data-color-scheme="no-preference: dark; light: dark; dark: dark;"
          data-size="large"
          href={downloadUrl}
        >
          Download
        </GitHubButton>
      </div>
    </main>
  );
}

export default Home;
