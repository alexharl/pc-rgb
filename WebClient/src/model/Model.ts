import { IPoint } from './Point';

export interface IComponent {
  name: string;
  id: number;
  pixelPositions: IPoint[];
}
