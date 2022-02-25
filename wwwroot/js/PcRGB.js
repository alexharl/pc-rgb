const useCanvas = callback => {
  const canvasRef = React.useRef(null);

  React.useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    callback([canvas, ctx]);
  }, []);

  return canvasRef;
};

const Canvas = ({ pixels, components }) => {
  const width = 400;
  const height = 400;
  var pixelRadius = 10;
  var pixelOffset = 20;
  const [ctx, setCtx] = React.useState();
  const map = (value, x1, y1, x2, y2) => ((value - x1) * (y2 - x2)) / (y1 - x1) + x2;
  const canvasRef = useCanvas(([canvas, context]) => {
    setCtx(context);
  });
  const updateCanvas = () => {
    ctx.clearRect(0, 0, width, height);
    for (let i = 0; i < pixels.length; i++) {
      const pixel = pixels[i];

      const hue = map(pixel.color.hue, 0, 255, 360, 0) - 200;
      const saturation = map(pixel.color.saturation, 0, 255, 0, 100);
      const brightness = 100 - map(pixel.color.brightness, 0, 255, 0, 100);

      const isComponentPixel =
        components &&
        components.find(c => {
          return !!c.pixelPositions.find(p => {
            return p.y === pixel.position.y && p.x === pixel.position.x;
          });
        });
      let alpha = !!isComponentPixel ? 1 : 0.3;
      const pixelPositionX = pixel.position.x * pixelOffset + pixelRadius + 1;
      const pixelPositionY = pixel.position.y * pixelOffset + pixelRadius + 1;

      ctx.beginPath();
      ctx.arc(pixelPositionX, pixelPositionY, pixelRadius, 0, 2 * Math.PI);
      ctx.fillStyle = `hsl(${hue},${saturation}%, ${brightness}%, ${alpha})`;
      ctx.fill();
    }
  };
  React.useEffect(() => {
    if (ctx && pixels) {
      updateCanvas();
    }
  }, [pixels, ctx]);
  return React.createElement('canvas', { ref: canvasRef, width, height });
};

const api = (() => {
  const apiUrl = '';
  const apiJson = async (path, options) => {
    const request = await fetch(apiUrl + path, options);
    if (request.status == 200) {
      return JSON.parse(await request.text());
    }
  };

  const createConnection = () => {
    return new signalR.HubConnectionBuilder()
      .withUrl(apiUrl + '/canvasHub')
      .configureLogging(signalR.LogLevel.Information)
      .build();
  };

  let connection = createConnection();

  async function connectSignalR() {
    if (connection.state != 'Connected') {
      connection = createConnection();
    }
    try {
      await connection.start();
      console.log('SignalR Connected.');
      return true;
    } catch (err) {
      console.log(err);
      return false;
    }
  }

  return {
    connectSignalR,
    onReceivePixels: callback => {
      connection.on('layer', function (pixels) {
        if (pixels) {
          callback(pixels);
        }
      });
    },
    onDisconnected: callback => {
      connection.onclose(async () => {
        connection = createConnection();
        callback();
      });
    },
    getComponents: async () => await apiJson('/canvas/components'),
    getCanvas: async () => await apiJson('/canvas'),
    layerVisibility: async (id, visible) => await apiJson(`/canvas/layer/${id}/visible/${visible ? 1 : 0}`),
    animate: async () => await apiJson('/canvas/render', { method: 'POST' })
  };
})();

const LayerControlVisibilityButton = ({ layer, setLayerVisibility }) => {
  const activeStyle = 'bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded';
  const inactiveStyle = 'bg-transparent hover:bg-blue-500 text-blue-700 font-semibold hover:text-white py-2 px-4 border border-blue-500 hover:border-transparent rounded';
  return React.createElement(
    'button',
    {
      onClick: () => {
        setLayerVisibility(!layer.visible);
      },
      className: layer.visible ? activeStyle : inactiveStyle
    },
    `${layer.name}  (${layer.visible ? 'ON' : 'OFF'})`
  );
};
const LayerControl = ({ layers, setLayerVisibility }) => {
  return React.createElement(
    React.Fragment,
    {},
    layers &&
      layers.map(layer => {
        return React.createElement(LayerControlVisibilityButton, {
          key: layer.id,
          layer,
          setLayerVisibility: visible => {
            setLayerVisibility(layer.id, visible);
          }
        });
      })
  );
};

const CanvasAnimateButton = () => {
  return React.createElement(
    'button',
    {
      onClick: () => {
        api.animate();
      },
      className: 'bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded'
    },
    'Animate'
  );
};

const PcRGB = () => {
  const [connected, setConnected] = React.useState();
  const [components, setComponents] = React.useState();
  const [canvas, setCanvas] = React.useState();
  const [pixels, setPixels] = React.useState();
  const [layers, setLayers] = React.useState();

  React.useEffect(() => {
    api.getCanvas().then(canvasResult => {
      setCanvas(canvasResult);
      setPixels(canvasResult.pixels);
      setLayers(canvasResult.layers);
    });
    api.getComponents().then(componentsResult => {
      setComponents(componentsResult);
    });

    // Start the signalR connection.
    api.connectSignalR().then(isConnected => {
      setConnected(isConnected);
    });
  }, []);

  api.onReceivePixels(p => {
    setPixels(p);
  });

  api.onDisconnected(async () => {
    setConnected(false);
    return await api.connectSignalR().then(isConnected => {
      setConnected(isConnected);
      return isConnected;
    });
  });

  if (!connected) {
    return React.createElement('h3', {}, 'Connecting...');
  }

  return React.createElement(React.Fragment, {}, [
    React.createElement(Canvas, { key: 'canvas', pixels, components }),
    React.createElement(LayerControl, {
      key: 'layer-controls',
      layers,
      setLayerVisibility: (id, visible) => {
        api.layerVisibility(id, visible).then(canvasResult => {
          if (canvasResult) {
            setLayers(canvasResult.layers);
          }
        });
      }
    }),
    React.createElement(CanvasAnimateButton, { key: 'animate' })
  ]);
};

ReactDOM.render(React.createElement(PcRGB), document.getElementById('root'));
