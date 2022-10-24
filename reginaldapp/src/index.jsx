import React from 'react';
import ReactDOM from 'react-dom/client';

import {
  createBrowserRouter,
  RouterProvider,
} from 'react-router-dom';

import './index.css';
import ApplicationLauncher from './components/ApplicationLauncher';
import Error from './components/Error';
import Features from './components/Features';
import Home from './components/Home';
import Root from './components/Root';

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
            element: <ApplicationLauncher />,
            path: 'features/application-launcher',
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
