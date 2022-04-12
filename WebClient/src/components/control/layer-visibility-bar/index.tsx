import { Box, Button, FormControlLabel, FormGroup, FormLabel, Grid, Paper, Switch, Typography } from '@material-ui/core';
import { FC } from 'react';
import { ILayer } from '../../../model/Layer';

export interface ILayerVisibilityButtonProps {
  layer: ILayer;
  setLayerVisibility: (visible: boolean) => void;
}

const LayerVisibilityButton: FC<ILayerVisibilityButtonProps> = ({ layer, setLayerVisibility }) => {
  // return <Button variant="contained" onClick={() => setLayerVisibility(!layer.visible)}>{`${layer.name}  (${layer.visible ? 'ON' : 'OFF'}`}</Button>;
  return <FormControlLabel control={<Switch checked={layer.visible} onChange={(e, checked) => setLayerVisibility(checked)} name={layer.name} />} label={layer.name} />;
};

export interface ILayerVisibilityBarProps {
  layers?: ILayer[];
  setLayerVisibility: (id: string, visible: boolean) => void;
}

export const LayerVisibilityBar: FC<ILayerVisibilityBarProps> = ({ layers, setLayerVisibility }) => {
  return (
    <Paper>
      <Box sx={{ display: 'flex', flexDirection: 'column', padding: '20px' }}>
        <FormLabel component="legend">Layers</FormLabel>
        <FormGroup>
          {layers &&
            layers.map(layer => {
              return (
                <Box>
                  <LayerVisibilityButton key={layer.id} layer={layer} setLayerVisibility={visible => setLayerVisibility(layer.id, visible)} />
                </Box>
              );
            })}
        </FormGroup>
      </Box>
    </Paper>
  );
};
