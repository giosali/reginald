import React from 'react';

import { BrowserRouter } from 'react-router-dom';

import Navigation from './components/Navigation';

function App() {
  return (
    <BrowserRouter>
      <Navigation />
    </BrowserRouter>
  );
}

export default App;
