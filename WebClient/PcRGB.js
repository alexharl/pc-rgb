const PcRGB = async root => {
  //////////////////
  // API          //
  //////////////////
  const api = (() => {
    const apiUrl = 'http://localhost:5000';
    const apiJson = async (path, options) => {
      const request = await fetch(apiUrl + path, options);
      if (request.status == 200) {
        return JSON.parse(await request.text());
      }
    };

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(apiUrl + '/canvasHub')
      .configureLogging(signalR.LogLevel.Information)
      .build();

    async function connectSignalR() {
      try {
        await connection.start();
        console.log('SignalR Connected.');
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    }

    connection.onclose(async () => {
      await connectSignalR();
    });

    return {
      connectSignalR,
      onReceivePixels: callback => {
        connection.on('layer', function (pixels) {
          if (pixels) {
            callback(pixels, components);
          }
        });
      },
      getComponents: async () => await apiJson('/canvas/components'),
      getCanvas: async () => await apiJson('/canvas'),
      layerVisibility: async (id, visible) => await apiJson(`/canvas/layer/${id}/visible/${visible ? 1 : 0}`),
      animate: async () => await apiJson('/canvas/render', { method: 'POST' })
    };
  })();

  //////////////////
  // Canvas       //
  //////////////////
  const canvas = (root => {
    const canvasNode = document.createElement('canvas');
    canvasNode.classList.add('canvas');
    canvasNode.width = 400;
    canvasNode.height = 400;
    root.appendChild(canvasNode);

    var canvasPixelRadius = 10;
    var canvasPixelOffset = 20;

    const map = (value, x1, y1, x2, y2) => ((value - x1) * (y2 - x2)) / (y1 - x1) + x2;

    const update = (pixels, components) => {
      const ctx = canvasNode.getContext('2d');
      ctx.clearRect(0, 0, canvasNode.width, canvasNode.height);
      for (let i = 0; i < pixels.length; i++) {
        const pixel = pixels[i];

        const hue = map(pixel.color.hue, 0, 255, 360, 0) - 200;
        const saturation = map(pixel.color.saturation, 0, 255, 0, 100);
        const brightness = 100 - map(pixel.color.brightness, 0, 255, 0, 100);

        const isComponentPixel = components.find(c => {
          return !!c.pixelPositions.find(p => {
            return p.y === pixel.position.y && p.x === pixel.position.x;
          });
        });
        let alpha = !!isComponentPixel ? 1 : 0.3;
        const pixelPositionX = pixel.position.x * canvasPixelOffset + canvasPixelRadius + 1;
        const pixelPositionY = pixel.position.y * canvasPixelOffset + canvasPixelRadius + 1;

        ctx.beginPath();
        ctx.arc(pixelPositionX, pixelPositionY, canvasPixelRadius, 0, 2 * Math.PI);
        ctx.fillStyle = `hsl(${hue},${saturation}%, ${brightness}%, ${alpha})`;
        ctx.fill();
      }
    };

    return {
      update
    };
  })(root);

  //////////////////
  // Startup      //
  //////////////////
  let components = await api.getComponents();
  let canvasLayer = await api.getCanvas();
  canvas.update(canvasLayer.pixels, components);

  pcRGB = {
    api,
    components,
    canvas,
    render: () => api.animate()
  };

  //////////////////
  // Layer Control//
  //////////////////
  const buttonFrame = document.createElement('div');
  buttonFrame.classList.add('button-frame');
  const getLayerButtonText = layer => {
    return layer.name + ' (' + (layer.visible ? 'ON' : 'OFF') + ')';
  };
  for (const layer of canvasLayer.layers) {
    const btn = document.createElement('button');
    btn.innerText = getLayerButtonText(layer);
    btn.addEventListener('click', () => {
      layer.visible = !layer.visible;
      btn.innerText = getLayerButtonText(layer);
      api.layerVisibility(layer.id, layer.visible);
    });
    buttonFrame.appendChild(btn);
  }
  root.appendChild(buttonFrame);

  //////////////////
  // SignalR      //
  //////////////////
  api.onReceivePixels(pixels => {
    canvas.update(pixels, components);
  });

  // Start the signalR connection.
  api.connectSignalR();

  return pcRgb;
};
