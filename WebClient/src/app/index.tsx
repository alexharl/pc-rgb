import { Box, Container, Drawer } from '@material-ui/core';
import { useEffect, useState } from 'react';
import {
  animate,
  connectSignalR,
  getCanvas,
  getComponents,
  layerVisibility,
  onAnimating,
  onDisconnected,
  onReceivePixels,
  onReconnected,
  serial,
  setPixel,
  step
} from '../api';
import { Canvas } from '../components/canvas';
import { CanvasActionButton, CanvasActionButtonRound } from '../components/control/canvas-action-button';
import { LayerVisibilityBar } from '../components/control/layer-visibility-bar';
import { ILayer } from '../model/Layer';
import { IComponent } from '../model/Model';
import { IPixel } from '../model/Pixel';
import { IRenderer } from '../model/Renderer';

export const PcRGB = () => {
  const [connected, setConnected] = useState<boolean>(false);
  const [components, setComponents] = useState<IComponent[]>();
  const [canvas, setCanvas] = useState<IRenderer>();
  const [pixels, setPixels] = useState<IPixel[]>();
  const [serialConnected, setSerialConnected] = useState<boolean>(false);
  const [layers, setLayers] = useState<ILayer[]>();
  const [animating, setAnimating] = useState<boolean>(false);

  const loadData = () => {
    getCanvas().then(canvasResult => {
      if (canvasResult) {
        setCanvas(canvasResult);
        setAnimating(canvasResult.animating);
        setPixels(canvasResult.pixels);
        setLayers(canvasResult.layers);
        setSerialConnected(canvasResult.serialOpen);
      }
    });

    getComponents().then(componentsResult => {
      if (componentsResult) {
        setComponents(componentsResult);
      }
    });
  };

  const startup = () => {
    loadData();

    // Start the signalR connection.
    connectSignalR().then(isConnected => {
      setConnected(isConnected);
    });
  };

  useEffect(() => {
    startup();
  }, []);

  onReceivePixels(p => {
    setPixels(p);
  });

  onAnimating(a => {
    setAnimating(a);
  });

  onReconnected(async () => {
    setConnected(true);
  });

  onDisconnected(async () => {
    setConnected(false);
  });

  const handlePixelSelected = (pixel: IPixel) => {
    // TODO ist nur eine ??bergangsl??sung
    var l = layers?.find(l => l.name === 'Draw Layer');
    if (l) {
      setPixel(l.id, pixel.position.x, pixel.position.y);
    }
  };

  if (!connected) {
    return (
      <div>
        <h3>Connecting...</h3>
        <button
          onClick={() => {
            startup();
          }}
        >
          connect
        </button>
      </div>
    );
  }

  return (
    <Container>
      <Box sx={{ display: 'flex', position: 'relative', paddingTop: '25px' }}>
        <Box>
          <Canvas pixelHeight={canvas?.rect.size.height} pixelWidth={canvas?.rect.size.width} pixels={pixels} components={components} onPixelSelected={handlePixelSelected} />
        </Box>
        <Box sx={{ marginLeft: '20px' }}>
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
        </Box>
        <Box sx={{ marginLeft: '20px' }}>
          <CanvasActionButtonRound
            active={animating}
            onClick={() =>
              animate().then(canvasResult => {
                if (canvasResult) {
                  setAnimating(canvasResult.animating);
                }
              })
            }
          />
          <CanvasActionButtonRound offLabel="Step" onClick={() => step()} />
          <CanvasActionButton
            active={serialConnected}
            offLabel="Connect"
            onLabel="Connected"
            onClick={() =>
              serial().then(res => {
                setSerialConnected(res);
              })
            }
          />
        </Box>
      </Box>
    </Container>
  );
};
