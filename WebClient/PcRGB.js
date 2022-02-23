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
