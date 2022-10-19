import React from 'react';

import { BrowserRouter, Route, Routes } from 'react-router-dom';

import Navigation from './components/Navigation';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route
          element={<Navigation />}
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
