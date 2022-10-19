import React from 'react';

import { BrowserRouter, Route, Routes } from 'react-router-dom';

import Home from './components/Home';
import Navigation from './components/Navigation';

function App() {
  return (
    <BrowserRouter>
      <Navigation />
      <Routes>
        <Route
          element={<Home />}
          path="/reginald"
        >
          <Route path="features" />
          <Route path="github" />
          <Route path="download" />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
