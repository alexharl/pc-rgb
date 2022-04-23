import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

const apiUrl = 'http://localhost:5000';

async function apiJson(path: string, options?: RequestInit) {
  const request = await fetch(apiUrl + path, options).catch(e => null);
  if (request && request.status === 200) {
    return JSON.parse(await request.text());
  }
}

const createConnection = () => {
  return new HubConnectionBuilder()
    .withUrl(apiUrl + '/canvasHub')
    .configureLogging(LogLevel.Information)
    .build();
};

let connection = createConnection();
export async function connectSignalR() {
  try {
    await connection.start();
    console.log('SignalR Connected.');
    return true;
  } catch (err) {
    return false;
  }
}

export async function getComponents() {
  return await apiJson('/canvas/components');
}

export async function getCanvas() {
  return await apiJson('/canvas');
}

export async function layerVisibility(id, visible) {
  return await apiJson(`/canvas/layer/${id}/visible/${visible ? 1 : 0}`);
}

export async function animate() {
  return await apiJson('/canvas/render', { method: 'POST' });
}

export async function step() {
  return await apiJson('/canvas/step', { method: 'POST' });
}

export async function setPixel(id, x, y) {
  return await apiJson(`/canvas/layer/${id}/draw?x=${x}&y=${y}`, { method: 'POST' });
}

export function onDisconnected(callback) {
  connection.onclose(callback);
}

export function onReconnected(callback) {
  connection.onreconnected(callback);
}

export function onReceivePixels(callback) {
  connection.on('layer', function (pixels) {
    if (pixels) {
      callback(pixels);
    }
  });
}

export function onAnimating(callback) {
  connection.on('animating', function (animating) {
    callback(animating);
  });
}
