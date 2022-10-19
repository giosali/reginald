import React from 'react';

import { useRouteError } from 'react-router-dom';

import '../stylesheets/Error.css';

function Error() {
  const error = useRouteError();
  console.error(error);

  return (
    <div className="Error">
      <div className="Error__middle">
        <div className="Error__inner">
          <h1 className="Error__title">Oops!</h1>
          <p className="Error__message">Sorry, an unexpected error has occurred.</p>
          <p>
            <i className="Error__status">{error.statusText || error.message}</i>
          </p>
        </div>
      </div>
    </div>
  );
}

export default Error;
