import React from 'react';
import ReactDOM from 'react-dom/client';

import {
  createBrowserRouter,
  RouterProvider,
} from 'react-router-dom';

import './index.css';
// import App from './App';
import Error from './components/Error';
import Features from './components/Features';
import Home from './components/Home';
import Root from './components/Root';

const router = createBrowserRouter([
  {
    children: [
      {
        element: <Home />,
        path: '',
      },
      {
        path: 'features',
        element: <Features />,
      },
    ],
    element: <Root />,
    errorElement: <Error />,
    path: 'reginald',
  },
]);

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>,
);
