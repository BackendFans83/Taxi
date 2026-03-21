import { lazy } from 'solid-js';
import { Route } from '@solidjs/router';

import { ROUTES } from './links';

const HomePage = lazy(() => import('@pages/HomePage'));
const ProfilePage = lazy(() => import('@pages/ProfilePage'));
const LoginPage = lazy(() => import('@pages/LoginPage'));
const RegisterPage = lazy(() => import('@pages/RegisterPage'));

export const routes = (
  <>
    <Route path={ROUTES.HOME} component={HomePage} />
    <Route path={ROUTES.LOGIN} component={LoginPage} />
    <Route path={ROUTES.REGISTER} component={RegisterPage} />
    <Route path={ROUTES.PROFILE} component={ProfilePage} />
  </>
);
