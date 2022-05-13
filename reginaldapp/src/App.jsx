import React, { useEffect, useState } from 'react';

import FixedTopDisplay from './components/FixedTopDisplay';
import MainCard from './components/MainCard';
import InfoCard from './components/InfoCard';
import InfoCardData from './data/InfoCardData';
import EndCard from './components/EndCard';

function App() {
  const [fixedTopDisplay, _setFixedTopDisplay] = useState({ isVisible: false, description: '' });
  const setFixedTopDisplay = (_description) => {
    if (_description !== fixedTopDisplay.description) {
      const isDescriptionEmpty = _description.length === 0;
      _setFixedTopDisplay({
        isVisible: !isDescriptionEmpty,
        description: isDescriptionEmpty ? 'A general productivity app for Windows' : _description,
      });
    }
  };

  const infoCards = [];
  for (let i = 0; i < InfoCardData.length; i++) {
    const infoCardDatum = InfoCardData[i];
    infoCards.push(
      <InfoCard
        key={infoCardDatum.id}
        title={infoCardDatum.title}
        description={infoCardDatum.description}
        position={infoCardDatum.position}
        image={infoCardDatum.image}
        representation={infoCardDatum.representation}
        setFixedDescription={setFixedTopDisplay}
      />,
    );
  }

  const handleScroll = () => {
    if (window.scrollY === 0) {
      setFixedTopDisplay('');
    }
  };
  useEffect(() => {
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  });

  const [downloadUrl, setDownloadUrl] = useState('');
  useEffect(() => {
    async function getLatestReleaseDownload() {
      const ep = 'https://api.github.com/repos/giosali/reginald/releases/latest';
      const response = await fetch(ep, {
        headers: {
          Accept: 'application/vnd.github.v3+json',
        },
      });
      if (response.status === 200) {
        const data = await response.json();
        setDownloadUrl(data.assets[0].browser_download_url);
        return;
      }

      const releases = 'https://github.com/giosali/reginald/releases';
      setDownloadUrl(releases);
    }

    getLatestReleaseDownload();
  }, []);

  return (
    <>
      <FixedTopDisplay
        isVisible={fixedTopDisplay.isVisible}
        description={fixedTopDisplay.description}
      />
      <div style={{ maxWidth: '1080px', margin: '0 auto' }}>
        <MainCard downloadUrl={downloadUrl} />
        {infoCards}
        <EndCard downloadUrl={downloadUrl} />
      </div>
    </>
  );
}

export default App;
