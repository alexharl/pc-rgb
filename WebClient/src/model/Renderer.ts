import { ILayer } from './Layer';

export interface IRenderer extends ILayer {
  frameTime: number;
  animating: boolean;
}
