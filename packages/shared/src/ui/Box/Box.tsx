import { JSX, splitProps } from 'solid-js';
import { Paper, PaperProps } from '../Paper/Paper';

export type BoxProps = PaperProps; // Box использует те же пропсы, что и Paper

export function Box(props: BoxProps) {
  const [local, others] = splitProps(props, ['class', 'children']);
  // По умолчанию elevation='sm', padding='md', без обводки
  return (
    <Paper elevation="sm" padding="md" outlined={false} class={local.class} {...others}>
      {local.children}
    </Paper>
  );
}