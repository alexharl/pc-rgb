import { Box, Paper } from '@material-ui/core';
import { FC, useEffect, useRef, useState } from 'react';
import { mapValueRange } from '../../helper/map';
import { IComponent } from '../../model/Model';
import { IPixel } from '../../model/Pixel';

const useCanvas = callback => {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    callback([canvas, canvas && canvas.getContext('2d')]);
  }, [callback]);

  return canvasRef;
};

export interface ICanvasProps {
  pixelWidth?: number;
  pixelHeight?: number;
  pixels?: IPixel[];
  components?: IComponent[];
  onPixelSelected?: (pixel: IPixel) => void;
}

export const Canvas: FC<ICanvasProps> = ({ pixelWidth, pixelHeight, pixels, components, onPixelSelected }) => {
  const pixelSize = 20;

  const width = (pixelWidth || 0) * pixelSize;
  const height = (pixelHeight || 0) * pixelSize;
  const [ctx, setCtx] = useState<CanvasRenderingContext2D>();

  const canvasRef = useCanvas(([canvas, context]) => {
    setCtx(context);
  });

  useEffect(() => {
    var pixelRadius = 10;
    var pixelOffset = pixelSize;
    if (ctx && pixels) {
      ctx.clearRect(0, 0, width, height);
      for (let i = 0; i < pixels.length; i++) {
        const pixel = pixels[i];

        const hue = mapValueRange(pixel.color.hue, 0, 255, 360, 0); // - 200;
        const saturation = mapValueRange(pixel.color.saturation, 0, 255, 0, 100);
        const brightness = 100 - mapValueRange(pixel.color.brightness, 0, 255, 0, 100);

        const isComponentPixel =
          components &&
          components.find(c => {
            return !!c.pixelPositions.find(p => {
              return p.y + c.rect.y === pixel.position.y && p.x + c.rect.x === pixel.position.x;
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

  const onCanvasClick = (e: React.MouseEvent<HTMLCanvasElement, MouseEvent>) => {
    if (onPixelSelected) {
      const x = Math.floor(e.clientX / pixelSize);
      const y = Math.floor(e.clientY / pixelSize);
      const foundPixel = pixels && pixels.find(p => p.position.x === x && p.position.y === y);
      if (foundPixel) {
        onPixelSelected(foundPixel);
      }
    }
  };

  return (
    <Paper elevation={3}>
      <Box sx={{ padding: 15, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <canvas ref={canvasRef} width={width} height={height} onClick={onCanvasClick} />
      </Box>
    </Paper>
  );
};
