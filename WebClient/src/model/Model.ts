import { ILayer } from './Layer';
import { IPoint } from './Point';

export interface IComponent extends ILayer {
  hardwareId: number;
  pixelPositions: IPoint[];
}
