import { IPixel } from './Pixel';
import { IRect } from './Rect';

export enum ELayerBlendMode {
  NORMAL = 1
}

export interface ILayer extends IRect {
  id: string;
  name: string;
  pixels: IPixel[];
  blendMode: ELayerBlendMode;
  layers: ILayer[];
  visible: boolean;
}
