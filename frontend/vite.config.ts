import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import { defineConfig } from 'vite'

// The backend (.NET 8 Minimal API) has no CORS configured and is treated as
// frozen, so the dev server proxies /api to it. From the browser's point of
// view everything is same-origin: cookies (auth + session) and the XSRF
// flow work without any backend change.
// Override with: VITE_API_PROXY_TARGET=http://localhost:5099 npm run dev
const apiTarget = process.env.VITE_API_PROXY_TARGET ?? 'http://localhost:5174'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
      },
    },
  },
})
