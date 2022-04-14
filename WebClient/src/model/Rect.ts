import { IPoint } from './Point';
import { ISize } from './Size';

export interface IRect extends IPoint {
  size: ISize;
}
