import { IHSB } from './HSB';
import { IPoint } from './Point';

export interface IPixel {
  position: IPoint;
  color: IHSB;
}
