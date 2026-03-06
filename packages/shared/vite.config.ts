import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import path from 'path';

export default defineConfig({
  plugins: [solidPlugin()],
  build: {
    lib: {
      entry: path.resolve(__dirname, 'src/index.ts'),
      name: 'TaxiShared',
      fileName: 'index',
      formats: ['es'],
    },
    rollupOptions: {
      external: ['solid-js', 'solid-js/web'],
      // Не агрегируем модули — позволяем tree-shaking
      preserveEntrySignatures: 'exports-only',
    },
    // Отключаем минификацию для dev, включаем для prod
    minify: 'esbuild',
    target: 'esnext',
  },
  server: {
    host: true,
    allowedHosts: ['*']
  }
});
