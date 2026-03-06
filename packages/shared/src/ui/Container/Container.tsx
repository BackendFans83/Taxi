import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Container.module.css';

export type ContainerProps = {
  maxWidth?: string | number;
  padding?: string | number;
  centered?: boolean;
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function Container(props: ContainerProps) {
  const [local, others] = splitProps(props, [
    'maxWidth', 'padding', 'centered', 'children', 'class'
  ]);

  const style = {
    'max-width': typeof local.maxWidth === 'number' ? `${local.maxWidth}px` : local.maxWidth,
    padding: typeof local.padding === 'number' ? `${local.padding}px` : local.padding,
    margin: local.centered !== false ? '0 auto' : undefined,
  };

  const classList = createMemo(() => [styles.container, local.class].filter(Boolean).join(' '));

  return (
    <div class={classList()} style={style} {...others}>
      {local.children}
    </div>
  );
}

Container.defaultProps = {
  maxWidth: '1400px',
  padding: '24px',
  centered: true,
};