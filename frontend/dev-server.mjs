#!/usr/bin/env node
/* Tiny dev server for the legacy-stack storefront (Node built-ins only).
 * - Serves static files from this directory.
 * - Proxies /api/* to the .NET 8 backend (same-origin cookies + XSRF,
 *   so no backend CORS change is needed).
 *
 * Usage:  node dev-server.mjs [port]
 * Backend: VITE_API_PROXY_TARGET or API_PROXY_TARGET, default http://localhost:5174
 */
import http from 'node:http';
import https from 'node:https';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const dir = path.dirname(fileURLToPath(import.meta.url));
const port = parseInt(process.argv[2] || process.env.PORT || '5173', 10);
const target = (process.env.VITE_API_PROXY_TARGET || process.env.API_PROXY_TARGET || 'http://localhost:5174').replace(/\/$/, '');

const MIME = {
  '.html': 'text/html; charset=utf-8', '.css': 'text/css; charset=utf-8',
  '.js': 'text/javascript; charset=utf-8', '.mjs': 'text/javascript; charset=utf-8',
  '.json': 'application/json; charset=utf-8', '.png': 'image/png',
  '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.gif': 'image/gif',
  '.svg': 'image/svg+xml', '.ico': 'image/x-icon', '.woff': 'font/woff',
  '.woff2': 'font/woff2', '.ttf': 'font/ttf', '.eot': 'application/vnd.ms-fontobject',
};

function serveFile(req, res, filePath) {
  fs.stat(filePath, (err, st) => {
    if (err || !st.isFile()) {
      res.writeHead(404, { 'Content-Type': 'text/plain' });
      res.end('Not found');
      return;
    }
    const ext = path.extname(filePath).toLowerCase();
    res.writeHead(200, { 'Content-Type': MIME[ext] || 'application/octet-stream' });
    fs.createReadStream(filePath).pipe(res);
  });
}

function proxy(req, res) {
  const url = new URL(target + req.url);
  const lib = url.protocol === 'https:' ? https : http;
  const proxyReq = lib.request(
    { hostname: url.hostname, port: url.port, path: url.pathname + url.search, method: req.method, headers: { ...req.headers, host: url.host } },
    (proxyRes) => { res.writeHead(proxyRes.statusCode, proxyRes.headers); proxyRes.pipe(res); }
  );
  proxyReq.on('error', () => { res.writeHead(502, { 'Content-Type': 'text/plain' }); res.end('Bad gateway: backend unreachable at ' + target); });
  req.pipe(proxyReq);
}

const server = http.createServer((req, res) => {
  if (req.url.startsWith('/api/') || req.url === '/api') return proxy(req, res);
  let pathname = decodeURIComponent(new URL(req.url, 'http://x').pathname);
  if (pathname.endsWith('/')) pathname += 'index.html';
  const filePath = path.normalize(path.join(dir, pathname));
  if (!filePath.startsWith(dir)) { res.writeHead(403); res.end('Forbidden'); return; }
  serveFile(req, res, filePath);
});

server.listen(port, () => console.log(`Storefront dev server: http://localhost:${port}  (proxy /api -> ${target})`));
