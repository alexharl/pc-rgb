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
    updateCanvas: async () => await apiJson('/canvas', { method: 'POST' })
  };

  //////////////////
  // Render       //
  //////////////////
  const map = (value, x1, y1, x2, y2) => ((value - x1) * (y2 - x2)) / (y1 - x1) + x2;

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

  const render = () => {
    canvasNode.innerHTML = '';
    for (let row of canvas.pixels) {
      const rowNode = document.createElement('div');
      rowNode.classList.add('row');
      canvasNode.appendChild(rowNode);
      for (let pixel of row) {
        rowNode.appendChild(createPixelNode(pixel));
      }
    }
  };

  //////////////////
  // Update       //
  //////////////////
  const update = async () => {
    canvas = await api.updateCanvas();
    render();
  };

  const components = await api.getComponents();
  let canvas = await api.getCanvas();
  console.log(components);
  render();

  const pcRGB = {
    api,
    components,
    canvas,
    update
  };

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
