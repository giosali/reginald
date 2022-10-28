import React from 'react';

import { useLoaderData } from 'react-router-dom';

import '../stylesheets/Feature.css';
import { getFeature } from '../data/features';

export function loader({ params }) {
  return getFeature(params.featureName.toLowerCase());
}

function Feature() {
  const feature = useLoaderData();

  return (
    <main className="Feature">
      <div className="header">
        <h1>{feature.title}</h1>
        <p className="header__description">{feature.description}</p>
        <span className="header__icon">{feature.icon}</span>
      </div>
      <div className="body">
        <div className="section">
          <h2 className="section__title">Summary</h2>
          <p
            className="section__description"
            dangerouslySetInnerHTML={{ __html: feature.summaryDescription }}
          />
        </div>
        {feature.keyBindingsDescription
          && (
            <div className="section">
              <h2 className="section__title">Key bindings</h2>
              <p
                className="section__description"
                dangerouslySetInnerHTML={{ __html: feature.keyBindingsDescription }}
              />
            </div>
          )}
        {feature.searchDescription
          && (
            <div className="section">
              <h2 className="section__title">Search</h2>
              <p
                className="section__description"
                dangerouslySetInnerHTML={{ __html: feature.searchDescription }}
              />
            </div>
          )}
        <div className="section">
          <h2 className="section__title">Settings</h2>
          <p
            className="section__description"
            dangerouslySetInnerHTML={{ __html: feature.settingsDescription }}
          />
        </div>
      </div>
    </main>
  );
}

export default Feature;
