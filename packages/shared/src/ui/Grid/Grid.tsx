import { JSX, splitProps, createMemo } from 'solid-js';
import styles from './Grid.module.css';

export type GridProps = {
  columns?: number | string; // например: 2, "repeat(3, 1fr)", "200px 1fr"
  gap?: string | number;
  minWidth?: string; // для auto-fit: если задано, то columns игнорируется, используется repeat(auto-fit, minmax(minWidth, 1fr))
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function Grid(props: GridProps) {
  const [local, others] = splitProps(props, [
    'columns', 'gap', 'minWidth', 'children', 'class'
  ]);

  const templateColumns = () => {
    if (local.minWidth) {
      return `repeat(auto-fit, minmax(${local.minWidth}, 1fr))`;
    }
    if (typeof local.columns === 'number') {
      return `repeat(${local.columns}, 1fr)`;
    }
    return local.columns || 'repeat(auto-fit, minmax(300px, 1fr))';
  };

  const style = {
    display: 'grid',
    'grid-template-columns': templateColumns(),
    gap: typeof local.gap === 'number' ? `${local.gap}px` : (local.gap || '24px'),
  };

  const classList = createMemo(() => [styles.grid, local.class].filter(Boolean).join(' '));

  return (
    <div class={classList()} style={style} {...others}>
      {local.children}
    </div>
  );
}

export type GridItemProps = {
  children?: JSX.Element;
  class?: string;
} & JSX.HTMLAttributes<HTMLDivElement>;

export function GridItem(props: GridItemProps) {
  const [local, others] = splitProps(props, ['children', 'class']);
  const classList = createMemo(() => [styles['grid-item'], local.class].filter(Boolean).join(' '));
  return (
    <div class={classList()} {...others}>
      {local.children}
    </div>
  );
}