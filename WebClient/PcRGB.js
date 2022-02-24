const PcRGB = async root => {
  //////////////////
  // API          //
  //////////////////
  const apiUrl = 'http://localhost:5000';
  const apiJson = async (path, options) => {
    const request = await fetch(apiUrl + path, options);
    if (request.status == 200) {
      return JSON.parse(await request.text());
    }
  };

  const api = {
    getComponents: async () => await apiJson('/canvas/components'),
    getCanvas: async () => await apiJson('/canvas'),
    updateCanvas: async () => await apiJson('/canvas', { method: 'POST' }),
    layerVisibility: async (id, visible) => await apiJson(`/canvas/layer/${id}/visible/${visible ? 1 : 0}`),
    autoRender: async () => await apiJson('/canvas/render', { method: 'POST' })
  };

  //////////////////
  // Render       //
  //////////////////
  const map = (value, x1, y1, x2, y2) => ((value - x1) * (y2 - x2)) / (y1 - x1) + x2;

  const updatePixelNode = pixel => {
    const pixelNodes = document.getElementsByClassName(`${pixel.position.x}-${pixel.position.y}`);
    if (!!pixelNodes.length) {
      const pixelNode = pixelNodes[0];
      var h = map(pixel.color.hue, 0, 255, 360, 0) - 200;
      const isComponentPixel = !!components.find(c => {
        return !!c.pixelPositions.find(p => {
          return p.y === pixel.position.y && p.x === pixel.position.x;
        });
      });
      let a = isComponentPixel ? 1 : 0.3;
      pixelNode.style.backgroundColor = `hsl(${h},${map(pixel.color.saturation, 0, 255, 0, 100)}%, ${100 - map(pixel.color.brightness, 0, 255, 0, 100)}%, ${a})`;
    }
  };
  const createPixelNode = pixel => {
    const pixelNode = document.createElement('div');
    pixelNode.classList.add('pixel');
    pixelNode.classList.add(`${pixel.position.x}-${pixel.position.y}`);
    var h = map(pixel.color.hue, 0, 255, 360, 0) - 200;
    const isComponentPixel = !!components.find(c => {
      return !!c.pixelPositions.find(p => {
        return p.y === pixel.position.y && p.x === pixel.position.x;
      });
    });
    let a = isComponentPixel ? 1 : 0.3;
    pixelNode.style.backgroundColor = `hsl(${h},${map(pixel.color.saturation, 0, 255, 0, 100)}%, ${100 - map(pixel.color.brightness, 0, 255, 0, 100)}%, ${a})`;
    return pixelNode;
  };

  const canvasNode = document.createElement('div');
  canvasNode.classList.add('canvas');
  root.appendChild(canvasNode);

  let initial = true;
  const render = () => {
    if (initial) {
      canvasNode.innerHTML = '';
    }
    for (let row of canvas.pixels) {
      let rowNode = null;
      if (initial) {
        rowNode = document.createElement('div');
        rowNode.classList.add('row');
        canvasNode.appendChild(rowNode);
      }
      for (let pixel of row) {
        if (initial) {
          rowNode.appendChild(createPixelNode(pixel));
        } else {
          updatePixelNode(pixel);
        }
      }
    }
    if (initial) {
      initial = false;
    }
  };

  //////////////////
  // Update       //
  //////////////////
  const update = async () => {
    canvas = await api.getCanvas();
    render();
  };

  const autoRender = async () => {
    api.autoRender();
  };

  const components = await api.getComponents();

  let canvas = await api.getCanvas();
  const buttonFrame = document.createElement('div');
  buttonFrame.classList.add('button-frame');
  for (const layer of canvas.layers) {
    const btn = document.createElement('button');
    btn.innerText = layer.name + ' (' + (layer.visible ? 'ON' : 'OFF') + ')';
    console.log(layer.name);
    btn.addEventListener('click', () => {
      layer.visible = !layer.visible;
      btn.innerText = layer.name + ' (' + (layer.visible ? 'ON' : 'OFF') + ')';
      api.layerVisibility(layer.id, layer.visible);
    });
    buttonFrame.appendChild(btn);
  }
  root.appendChild(buttonFrame);

  render();

  const pcRGB = {
    api,
    components,
    canvas,
    update,
    render: autoRender
  };

  const connection = new signalR.HubConnectionBuilder()
    .withUrl(apiUrl + '/canvasHub')
    .configureLogging(signalR.LogLevel.Information)
    .build();

  connection.on('layer', function (buffer) {
    if (buffer) {
      for (let r = 0; r < buffer.length; r++) {
        for (let p = 0; p < buffer[r].length; p++) {
          updatePixelNode(buffer[r][p]);
        }
      }
    }
  });

  async function start() {
    try {
      await connection.start();
      console.log('SignalR Connected.');
    } catch (err) {
      console.log(err);
      setTimeout(start, 5000);
    }
  }

  connection.onclose(async () => {
    await start();
  });

  // Start the connection.
  start();

  return pcRGB;
};

/*

export interface Point {
  x: number;
  y: number;
}
export interface Size {
  width: number;
  height: number;
}
export interface IHsbColor {
  hue: number;
  saturation: number;
  brightness: number;
  alpha: number;
}
export interface IPixel {
  position: Point;
  color: IHsbColor;
}
export interface IPixelNodeProps {
  pixel: IPixel;
}
export const PixelNode: React.FC<IPixelNodeProps> = ({ pixel }) => {
  const map = (value: number, x1: number, y1: number, x2: number, y2: number) => ((value - x1) * (y2 - x2)) / (y1 - x1) + x2;
  const hue = map(pixel.color.hue, 0, 255, 360, 0) - 200;
  const saturation = map(pixel.color.saturation, 0, 255, 0, 100);
  const brightness = 100 - map(pixel.color.brightness, 0, 255, 0, 100);
  const alpha = 1;
  const hslValue = `hsl(${hue},${saturation}%,${brightness}%, ${alpha})`;
  return <div className="w-2 h-2" style={{ backgroundColor: hslValue }}></div>;
};
export interface IPixelRowProps {
  pixels: IPixel[];
}
export const PixelRow: React.FC<IPixelRowProps> = ({ pixels }) => {
  return (
    <div className="w-2 h-40">
      {pixels.map(pixel => (
        <PixelNode key={`pixel-${pixel.position.x}-${pixel.position.y}`} pixel={pixel} />
      ))}
    </div>
  );
};
export interface ILayer {
  id: string;
  name: string;
  blendMode: number;
  pixels: IPixel[][];
  layers: ILayer[];
  position: Point;
  size: Size;
  visible: boolean;
}
export interface ILayerProps {
  layer: ILayer;
}
export const Layer: React.FC<ILayerProps> = ({ layer }) => {
  return (
    <div className="flex">
      {layer.pixels.map((row, index) => {
        return <PixelRow key={`row-${index}`} pixels={row} />;
      })}
    </div>
  );
};
*/
