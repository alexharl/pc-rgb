import { Box, Button, Fab } from '@material-ui/core';
import { FC } from 'react';

export interface ICanvasAnimateButtonProps {
  active?: boolean;
  onClick?: () => void;
  onLabel?: string;
  offLabel?: string;
}

export const CanvasActionButtonRound: FC<ICanvasAnimateButtonProps> = ({ active, onClick, onLabel, offLabel }) => {
  return (
    <Fab onClick={onClick} color={active ? 'secondary' : 'primary'} aria-label="animate">
      {active ? onLabel || 'Stop' : offLabel || 'Start'}
    </Fab>
  );
};

export const CanvasActionButton: FC<ICanvasAnimateButtonProps> = ({ active, onClick, onLabel, offLabel }) => {
  return (
    <Button onClick={onClick} color={active ? 'secondary' : 'primary'} aria-label="animate">
      {active ? onLabel || 'Stop' : offLabel || 'Start'}
    </Button>
  );
};
