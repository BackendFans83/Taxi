import { Router } from '@solidjs/router';
import { Suspense } from 'solid-js';
import type { ParentComponent } from 'solid-js';
import { routes } from '@routes/routes';

const AppRoot: ParentComponent = (props) => {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      {props.children}
    </Suspense>
  );
};

export const RouterProvider = () => {
  return (
    <Router root={AppRoot}>
      {routes}
    </Router>
  );
};
