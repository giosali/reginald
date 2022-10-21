import React, { useEffect, useState } from 'react';

import '../stylesheets/Home.css';
import GitHubButton from './GitHubButton';
import Logo from '../images/logo.png';

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
    <div className="Home">
      <div className="hero">
        <img
          alt="Reginald logo"
          className="hero__image"
          src={Logo}
        />
        <h1 className="hero__title">Reginald</h1>
        <p className="hero__description">A general productivity application designed for Windows and inspired by Alfred</p>
        <div className="hero__github-buttons">
          <GitHubButton
            href="https://github.com/giosali/reginald"
            icon="octicon-star"
            repo="giosali/reginald"
            type="Star"
          />
          <GitHubButton
            href={downloadUrl}
            repo="giosali/reginald"
            type="Download"
          />
        </div>
      </div>
    </div>
  );
}

export default Home;
