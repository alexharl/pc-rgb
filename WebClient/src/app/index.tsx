import { useEffect, useState } from 'react';
import { animate, connectSignalR, getCanvas, getComponents, layerVisibility, onDisconnected, onReceivePixels } from '../api';
import { Canvas } from '../components/canvas';
import { CanvasAnimateButton } from '../components/control/canvas-animate-button';
import { LayerVisibilityBar } from '../components/control/layer-visibility-bar';

export const PcRGB = () => {
  const [connected, setConnected] = useState(false);
  const [components, setComponents] = useState();
  const [canvas, setCanvas] = useState();
  const [pixels, setPixels] = useState();
  const [layers, setLayers] = useState();

  useEffect(() => {
    getCanvas().then(canvasResult => {
      setCanvas(canvasResult);
      setPixels(canvasResult.pixels);
      setLayers(canvasResult.layers);
    });
    getComponents().then(componentsResult => {
      setComponents(componentsResult);
    });

    // Start the signalR connection.
    connectSignalR().then(isConnected => {
      setConnected(isConnected);
    });
  }, []);

  onReceivePixels(p => {
    setPixels(p);
  });

  onDisconnected(async () => {
    setConnected(false);
    return await connectSignalR().then(isConnected => {
      setConnected(isConnected);
      return isConnected;
    });
  });

  if (!connected) {
    return <h3>Connecting...</h3>;
  }

  return (
    <>
      <Canvas pixels={pixels} components={components} />
      <LayerVisibilityBar
        layers={layers}
        setLayerVisibility={(layerId, visible) => {
          layerVisibility(layerId, visible).then(canvasResult => {
            if (canvasResult) {
              setLayers(canvasResult.layers);
            }
          });
        }}
      />
      <CanvasAnimateButton toggleAnimation={() => animate()} />
    </>
  );
};
