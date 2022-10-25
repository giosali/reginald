import React from 'react';

import { useLoaderData } from 'react-router-dom';

import { getFeature } from '../data/features';

export function loader({ params }) {
  return getFeature(params.featureName.toLowerCase());
}

function Feature() {
  const feature = useLoaderData();
  console.log(feature);

  return <div />;
}

export default Feature;
