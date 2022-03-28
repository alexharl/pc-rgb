import { Button } from '@material-ui/core';

const LayerVisibilityButton = ({ layer, setLayerVisibility }) => {
  return <Button onClick={() => setLayerVisibility(!layer.visible)}>{`${layer.name}  (${layer.visible ? 'ON' : 'OFF'}`}</Button>;
};

export const LayerVisibilityBar = ({ layers, setLayerVisibility }) => {
  return (
    <>
      {layers &&
        layers.map(layer => {
          return <LayerVisibilityButton key={layer.id} layer={layer} setLayerVisibility={visible => setLayerVisibility(layer.id, visible)} />;
        })}
    </>
  );
};
