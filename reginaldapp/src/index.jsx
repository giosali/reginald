import React from 'react';
import ReactDOM from 'react-dom/client';

import {
  createBrowserRouter,
  RouterProvider,
} from 'react-router-dom';

import './index.css';
import Error from './routes/Error';
import Feature, { loader as featureLoader } from './routes/Feature';
import Features from './routes/Features';
import Home from './routes/Home';
import Root from './routes/Root';

const router = createBrowserRouter([
  {
    children: [
      {
        children: [
          {
            element: <Home />,
            index: true,
          },
          {
            element: <Features />,
            path: 'features',
          },
          {
            element: <Feature />,
            loader: featureLoader,
            path: 'features/:featureName',
          },
        ],
        element: <Root />,
        path: 'reginald',
      },
    ],
    errorElement: <Error />,
    path: '/',
  },
]);

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>,
);
