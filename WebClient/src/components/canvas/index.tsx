import { useEffect, useRef, useState } from 'react';
import { mapValueRange } from '../../helper/map';

const useCanvas = callback => {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    callback([canvas, canvas && canvas.getContext('2d')]);
  }, [callback]);

  return canvasRef;
};

export const Canvas = ({ pixels, components }) => {
  const width = 400;
  const height = 400;
  const [ctx, setCtx] = useState<CanvasRenderingContext2D>();

  const canvasRef = useCanvas(([canvas, context]) => {
    setCtx(context);
  });

  useEffect(() => {
    var pixelRadius = 10;
    var pixelOffset = 20;
    if (ctx && pixels) {
      ctx.clearRect(0, 0, width, height);
      for (let i = 0; i < pixels.length; i++) {
        const pixel = pixels[i];

        const hue = mapValueRange(pixel.color.hue, 0, 255, 360, 0) - 200;
        const saturation = mapValueRange(pixel.color.saturation, 0, 255, 0, 100);
        const brightness = 100 - mapValueRange(pixel.color.brightness, 0, 255, 0, 100);

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
    }
  }, [pixels, ctx, components]);

  return <canvas ref={canvasRef} width={width} height={height} />;
};
