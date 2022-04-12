import { Box, Fab } from '@material-ui/core';
import { FC } from 'react';

export interface ICanvasAnimateButtonProps {
  animating: boolean;
  toggleAnimation: () => void;
}

export const CanvasAnimateButton: FC<ICanvasAnimateButtonProps> = ({ animating, toggleAnimation }) => {
  return (
    <Fab onClick={() => toggleAnimation()} color={animating ? 'secondary' : 'primary'} aria-label="animate">
      {animating ? 'Stop' : 'Start'}
    </Fab>
  );
};
