const useCanvas = callback => {
  const canvasRef = React.useRef(null);

  React.useEffect(() => {
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    callback([canvas, ctx]);
  }, []);

  return canvasRef;
};

const Canvas = () => {
  const [position, setPosition] = React.useState({});
  const canvasRef = useCanvas(([canvas, ctx]) => {
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    const x = canvas.width;
    const y = canvas.height;
    setPosition({ x, y });
  });
  return React.createElement('canvas', { ref: canvasRef });
};

ReactDOM.render(React.createElement(Canvas), document.getElementById('root'));
