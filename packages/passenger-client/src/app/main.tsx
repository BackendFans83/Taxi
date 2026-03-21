/* @refresh reload */
import { render } from 'solid-js/web';
import '@app/styles/global.css';
import App from './App';

const root = document.getElementById('root');

if (!root) {
  throw new Error('Root element not found');
}

render(() => <App />, root);
