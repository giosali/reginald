import React from 'react';

import '../stylesheets/Features.css';
import FeatureCard from '../components/FeatureCard';
import slugify from '../utils';
import { getFeatures } from '../data/features';

function Features() {
  return (
    <main className="Features">
      <div className="header">
        <h1 className="header__title">Features</h1>
        <p className="header__description">Explore everything that Reginald has to offer</p>
      </div>
      <div className="cards">
        {getFeatures().map((f) => ( // eslint-disable-line no-unused-vars
          <FeatureCard description={f.description} icon={f.icon} key={f.key} title={f.title} to={`/reginald/features/${slugify(f.title)}`} />))}
      </div>
    </main>
  );
}

export default Features;
