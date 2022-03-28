import { Button } from '@material-ui/core';

export const CanvasAnimateButton = ({ toggleAnimation }) => {
  return <Button onClick={() => toggleAnimation()}>Animate</Button>;
};
